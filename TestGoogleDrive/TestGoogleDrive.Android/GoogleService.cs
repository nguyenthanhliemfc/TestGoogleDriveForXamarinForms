using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using TestGoogleDrive.Droid;


using Android.Util;
using System.Threading.Tasks;
using Java.IO;

using Android.Gms.Common;


using Android.Gms.Auth;
using Java.Lang;
using Android.Gms.Common.Apis;
using Android.Gms.Auth.Api.SignIn;

using Android.Gms.Drive;
using Android.Gms.Auth.Api;
using Android.Support.V4.App;


namespace TestGoogleDrive.Droid
{
    public class GoogleService : Java.Lang.Object,IGoogleService,
        GoogleApiClient.IConnectionCallbacks,
        GoogleApiClient.IOnConnectionFailedListener,
        IResultCallback

    {
        public static GoogleService Current; 
        Context mContext;
        FragmentActivity mFragmentActivity;

        public GoogleApiClient mGoogleApiClient;
        GoogleSignInResult mGoogleSignInResult = null;

        public const string TAG = "GoogleService";
        public const int REQUEST_CODE_RESOLUTION = 3;

        public static void Init(Context context, FragmentActivity fragmentActivity)
        {
            var obj = new GoogleService(context, fragmentActivity);
            TestGoogleDrive.GoogleService.Current = obj;        //  interface
            TestGoogleDrive.Droid.GoogleService.Current = obj; // class
        }
        public GoogleService(Context context, FragmentActivity fragmentActivity)
        {
            mContext = context;
            mFragmentActivity = fragmentActivity;

            mGoogleApiClient = new GoogleApiClient.Builder(mContext)
                  .AddApi(DriveClass.API)
                  .AddScope(DriveClass.ScopeFile)
                  .AddConnectionCallbacks(this)
                  .AddOnConnectionFailedListener(OnConnectionFailed)
                  .Build();
        }        

        public void OnConnectionFailed(ConnectionResult result)
        {
            // An unresolvable error has occurred and Google APIs (including Sign-In) will not
            // be available.
            Log.Debug(TAG, "onConnectionFailed:" + result);
            if (!result.HasResolution)
            {
                GoogleApiAvailability.Instance.GetErrorDialog(mFragmentActivity, result.ErrorCode, 0).Show();
                return;
            }
            try
            {
                result.StartResolutionForResult(mFragmentActivity, REQUEST_CODE_RESOLUTION);
            }
            catch (IntentSender.SendIntentException e)
            {
                Log.Error(TAG, "Exception while starting resolution activity", e);
            }
        }


        public void ProcessActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == REQUEST_CODE_RESOLUTION)
            {
                switch (resultCode)
                {
                    case Result.Ok:
                        mGoogleApiClient.Connect();
                        break;
                    case Result.Canceled:
                        //Toast.MakeText(this, "Unable to sign in", ToastLength.Short).Show();
                        Log.Error(TAG, "Unable to sign in, is app registered for Drive access in Google Dev Console?");
                        break;
                    case Result.FirstUser:
                        //Toast.MakeText(this, "Unable to sign in: RESULT_FIRST_USER", ToastLength.Short).Show();
                        Log.Error(TAG, "Unable to sign in: RESULT_FIRST_USER");
                        break;
                    default:
                        Log.Error(TAG, "Should never be here: " + resultCode);
                        return;
                }
            }
        }
        


        void GoogleApiClient.IConnectionCallbacks.OnConnected(Bundle connectionHint)
        {
            Log.Info(TAG, "OnConnected()");           
        }

        void GoogleApiClient.IConnectionCallbacks.OnConnectionSuspended(int cause)
        {
            Log.Info(TAG, "OnConnectionSuspended()");
        }

        public void ChangeAccount()
        {
            if (!mGoogleApiClient.IsConnected)
                return;


            mGoogleApiClient.ClearDefaultAccountAndReconnect();
        }

        public string GetAccount()
        {
            string email = "";
            if (mGoogleSignInResult != null && mGoogleSignInResult.IsSuccess)
            {
                email = mGoogleSignInResult.SignInAccount.Email;
            }
            return email;
        }

        public void Connect()
        {
            if (mGoogleApiClient.IsConnected)
                return;

            mGoogleApiClient.Connect();
        }

        public void Disconnect()
        {
            if (mGoogleApiClient != null && mGoogleApiClient.IsConnected)
            {
                mGoogleApiClient.Disconnect();

            }
        }

        public void NewDriveContent()
        {
            if (!mGoogleApiClient.IsConnected)
                return;

            DriveClass.DriveApi.NewDriveContents(mGoogleApiClient).SetResultCallback(new NewDriveContentsResultCallback { Activity = this });          
        }
        
        private class NewDriveContentsResultCallback : Java.Lang.Object, IResultCallback
        {
            public GoogleService Activity { get; set; }

            public void OnResult(Java.Lang.Object result)
            {
                var contentResults = (result).JavaCast<IDriveApiDriveContentsResult>();
                if (!contentResults.Status.IsSuccess) // handle the error
                    return;

                Task.Run(() =>
                {
                    var writer = new OutputStreamWriter(contentResults.DriveContents.OutputStream);
                    writer.Write("Stack Overflow");
                    writer.Close();
                    MetadataChangeSet changeSet = new MetadataChangeSet.Builder()
                           .SetTitle("New Text File")
                           .SetMimeType("text/plain")
                           .Build();
                    DriveClass.DriveApi.GetRootFolder(Activity.mGoogleApiClient).CreateFile(Activity.mGoogleApiClient, changeSet, contentResults.DriveContents);

                    

                });


            }
        }

        public IDriveFolder GetRootFolder()
        {
            return DriveClass.DriveApi.GetRootFolder(mGoogleApiClient);
        }

        public void Test()
        {

            Task.Run(() =>
            {
                try
                {
                    IDriveFolder driveFolder = DriveClass.DriveApi.GetRootFolder(mGoogleApiClient);
                    var ret = driveFolder.ListChildrenAsync(mGoogleApiClient);
                }
                catch (System.Exception)
                {


                }



            });
   
           
        }

        void IResultCallback.OnResult(Java.Lang.Object result)
        {
            var contentResults = (result).JavaCast<IDriveApiDriveContentsResult>();
            if (!contentResults.Status.IsSuccess) // handle the error
                return;
            Task.Run(() =>
            {
                var writer = new OutputStreamWriter(contentResults.DriveContents.OutputStream);
                writer.Write("Stack Overflow");
                writer.Close();
                MetadataChangeSet changeSet = new MetadataChangeSet.Builder()
                       .SetTitle("New Text File")
                       .SetMimeType("text/plain")
                       .Build();
                DriveClass.DriveApi
                          .GetRootFolder(mGoogleApiClient)
                          .CreateFile(mGoogleApiClient, changeSet, contentResults.DriveContents);
            });
        }
    } 
}