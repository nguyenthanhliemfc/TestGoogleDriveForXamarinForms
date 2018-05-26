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
        GoogleApiClient.IOnConnectionFailedListener
        
    {
        Context mContext;
        FragmentActivity mFragmentActivity;

        public GoogleApiClient mGoogleApiClient;
        GoogleSignInResult mGoogleSignInResult = null;

        public const string TAG = "GoogleService";
        public const int REQUEST_CODE_SIGNIN = 9001;
        public const int REQUEST_CODE_RESOLUTION = 3;

        EventHandler handlerSignIn;
        EventHandler handlerSignOut;

        public GoogleService(Context context, FragmentActivity fragmentActivity)
        {
            mContext = context;
            mFragmentActivity = fragmentActivity;
            TestGoogleDrive.GoogleService.Current = this;
        }
        public void Init()
        {
            // [START configure_signin]
            // Configure sign-in to request the user's ID, email address, and basic
            // profile. ID and basic profile are included in DEFAULT_SIGN_IN.
            GoogleSignInOptions gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                .RequestScopes(new Scope(Scopes.DriveAppfolder))
                    .RequestEmail()
                    .Build();
            // [END configure_signin]

            // [START build_client]
            // Build a GoogleApiClient with access to the Google Sign-In API and the
            // options specified by gso.
            mGoogleApiClient = new GoogleApiClient.Builder(mContext, this, this)
                    .EnableAutoManage(mFragmentActivity /* FragmentActivity */, this /* OnConnectionFailedListener */)
                    .AddApi(Auth.GOOGLE_SIGN_IN_API, gso)
                    .AddApi(DriveClass.API)
                    .Build();
            // [END build_client
        }

        public void SilentSignIn()
        {
            var opr = Auth.GoogleSignInApi.SilentSignIn(mGoogleApiClient);
            if (opr.IsDone)
            {
                // If the user's cached credentials are valid, the OptionalPendingResult will be "done"
                // and the GoogleSignInResult will be available instantly.
                Log.Debug(TAG, "Got cached sign-in");
                var result = opr.Get() as GoogleSignInResult;
                HandleSignInResult(result);
            }
            else
            {
                // If the user has not previously signed in on this device or the sign-in has expired,
                // this asynchronous branch will attempt to sign in the user silently.  Cross-device
                // single sign-on will occur in this branch.
                //ShowProgressDialog();
                opr.SetResultCallback(new SignInResultCallback { Activity = this });
            }
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

        public void HandleSignInResult(GoogleSignInResult result)
        {
            Log.Debug(TAG, "handleSignInResult:" + result.IsSuccess);
            mGoogleSignInResult = result;
            if (result.IsSuccess)
            {
                // Signed in successfully, show authenticated UI.
                var acct = result.SignInAccount;
               
                //mStatusTextView.Text = string.Format(GetString(Resource.String.signed_in_fmt), acct.DisplayName);
                //UpdateUI(true);


                
            }
            else
            {
                // Signed out, show unauthenticated UI.
                //UpdateUI(false);
                                                
            }

            handlerSignIn?.Invoke(this, EventArgs.Empty);
        }

        public void HandleSignoutResult()
        {
            Log.Debug(TAG, "HandleSignoutResult");
            mGoogleSignInResult = null;
            
            handlerSignOut?.Invoke(this, EventArgs.Empty);
        }




        public void ProcessActivityResult(int requestCode, Result resultCode, Intent data)
        {           
            var result = Auth.GoogleSignInApi.GetSignInResultFromIntent(data);
            HandleSignInResult(result);
        }
        


        void GoogleApiClient.IConnectionCallbacks.OnConnected(Bundle connectionHint)
        {
            Log.Info(TAG, "OnConnected()");
        }

        void GoogleApiClient.IConnectionCallbacks.OnConnectionSuspended(int cause)
        {
            Log.Info(TAG, "OnConnectionSuspended()");
        }



        public void SignIn(EventHandler eventHandler)
        {
            if (eventHandler != null)
                handlerSignIn = eventHandler;

            var signInIntent = Auth.GoogleSignInApi.GetSignInIntent(mGoogleApiClient);

            mFragmentActivity.StartActivityForResult(signInIntent, REQUEST_CODE_SIGNIN);            
        }

        public void SignOut(EventHandler eventHandler)
        {
            if (!mGoogleApiClient.IsConnected)
                return;

            if (eventHandler != null)
                handlerSignOut = eventHandler;

            Auth.GoogleSignInApi.SignOut(mGoogleApiClient).SetResultCallback(new SignOutResultCallback { Activity = this });
        }

        public void RevokeAccess()
        {
            if (!mGoogleApiClient.IsConnected)
                return;

            Auth.GoogleSignInApi.RevokeAccess(mGoogleApiClient).SetResultCallback(new RevokeResultCallback { Activity = this });
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

        private class SignInResultCallback : Java.Lang.Object, IResultCallback
        {
            public GoogleService Activity { get; set; }

            public void OnResult(Java.Lang.Object result)
            {
                Log.Debug(TAG, "SignInResultCallback");
                var googleSignInResult = result as GoogleSignInResult;
                //Activity.HideProgressDialog();
                Activity.HandleSignInResult(googleSignInResult);
            }
        }

        private class SignOutResultCallback : Java.Lang.Object, IResultCallback
        {
            public GoogleService Activity { get; set; }

            public void OnResult(Java.Lang.Object result)
            {
                Log.Debug(TAG, "SignOutResultCallback");
                Activity.HandleSignoutResult();
            }
        }

        private class RevokeResultCallback : Java.Lang.Object, IResultCallback
        {
            public GoogleService Activity { get; set; }

            public void OnResult(Java.Lang.Object result)
            {
                Log.Debug(TAG, "RevokeResultCallback");
                
            }
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
                    DriveClass.DriveApi
                              .GetRootFolder(Activity.mGoogleApiClient)
                              .CreateFile(Activity.mGoogleApiClient, changeSet, contentResults.DriveContents);
                });
            }
        }
    }

}