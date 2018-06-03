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
    public class GoogleService : Java.Lang.Object, IGoogleService,
        GoogleApiClient.IConnectionCallbacks,
        GoogleApiClient.IOnConnectionFailedListener

    {
        public const string TAG = "GoogleService";
        public const int REQUEST_CODE_RESOLUTION = 3;

        public static GoogleService Current;
        Context mContext;
        FragmentActivity mFragmentActivity;

        public GoogleApiClient mGoogleApiClient;
        GoogleSignInResult mGoogleSignInResult = null;

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

        public Task<bool> NewDriveContent(string folderDriveId)
        {   
            var res = Task.Run<bool>(() =>
            {
                try
                {
                    IDriveApiDriveContentsResult contentResults = (DriveClass.DriveApi.NewDriveContents(mGoogleApiClient).Await()).JavaCast<IDriveApiDriveContentsResult>();
                    if (!contentResults.Status.IsSuccess) // handle the error
                        throw new System.Exception(contentResults.ToString());

                    var writer = new OutputStreamWriter(contentResults.DriveContents.OutputStream);
                    writer.Write("Stack Overflow");

                    writer.Close();
                    MetadataChangeSet changeSet = new MetadataChangeSet.Builder()
                       .SetTitle("New Text File")
                       .SetMimeType("text/plain")
                       .Build();

                    IDriveFolder driveFolder = null;
                    if (string.IsNullOrEmpty(folderDriveId))
                    {
                        driveFolder = DriveClass.DriveApi.GetRootFolder(mGoogleApiClient);
                    }
                    else
                    {
                        IDriveApiDriveIdResult driveIdResult = (DriveClass.DriveApi.FetchDriveId(mGoogleApiClient, folderDriveId).Await()).JavaCast<IDriveApiDriveIdResult>();
                        if (!driveIdResult.Status.IsSuccess) // handle the error
                            throw new System.Exception(driveIdResult.ToString());

                        driveFolder = driveIdResult.DriveId.AsDriveFolder();
                    }

                    IDriveFolderDriveFileResult folderDriveFileResult = (driveFolder.CreateFile(mGoogleApiClient, changeSet, contentResults.DriveContents)
                        .Await()).JavaCast<IDriveFolderDriveFileResult>();

                    if (!folderDriveFileResult.Status.IsSuccess) // handle the error
                        throw new System.Exception(folderDriveFileResult.ToString());

                    return true;
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    return false;
                }
                
            });
            return res;
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
       

        public void UploadFile(string folderDriveId,string fileName, System.IO.Stream stream)
        {
            if (!mGoogleApiClient.IsConnected)
                return;

            DriveClass.DriveApi.NewDriveContents(mGoogleApiClient).SetResultCallback(
                new UploadResultCallback
                {
                    FolderDriveId = folderDriveId,
                    FileName = fileName,
                    Stream = stream,
                    GoogleApiClient = mGoogleApiClient
                });
        }

        private class UploadResultCallback : Java.Lang.Object, IResultCallback
        {
            public string FolderDriveId { get; set; }
            public string FileName { get; set; }
            public System.IO.Stream Stream { get; set; }
            public GoogleApiClient GoogleApiClient { get; set; }

            public void OnResult(Java.Lang.Object result)
            {
                var contentResults = (result).JavaCast<IDriveApiDriveContentsResult>();
                if (!contentResults.Status.IsSuccess) // handle the error
                    return;

                Task.Run(() =>
                {
                    Stream.CopyTo(contentResults.DriveContents.OutputStream);
                    MetadataChangeSet changeSet = new MetadataChangeSet.Builder().SetTitle(FileName).Build();

                    IDriveFolder driveFolder = null;
                    if (string.IsNullOrEmpty(FolderDriveId))
                    {
                        driveFolder = DriveClass.DriveApi.GetRootFolder(GoogleApiClient);
                    }
                    else
                    {
                        IDriveApiDriveIdResult res = (DriveClass.DriveApi.FetchDriveId(GoogleApiClient, FolderDriveId).Await()).JavaCast<IDriveApiDriveIdResult>();
                        if (!res.Status.IsSuccess) // handle the error
                            return;

                        driveFolder = res.DriveId.AsDriveFolder();
                    }

                    IDriveFolderDriveFileResult driveFolderDriveFileResult = 
                        (driveFolder.CreateFile(GoogleApiClient, changeSet, contentResults.DriveContents).Await()).JavaCast<IDriveFolderDriveFileResult>();
                    
                });
            }
        }

        public void UploadAll(string folderDriveId)
        {
            if (!mGoogleApiClient.IsConnected)
                return;

            Task.Run(() =>
            {
                IDriveFolder driveFolder = null;
                if (string.IsNullOrEmpty(folderDriveId))
                {
                    driveFolder = DriveClass.DriveApi.GetRootFolder(mGoogleApiClient);
                }
                else
                {
                    IDriveApiDriveIdResult result = (DriveClass.DriveApi.FetchDriveId(mGoogleApiClient, folderDriveId).Await()).JavaCast<IDriveApiDriveIdResult>();
                    if (!result.Status.IsSuccess) // handle the error
                        return;

                    driveFolder = result.DriveId.AsDriveFolder();
                }

                UploadAll(PCLStorage.FileSystem.Current.LocalStorage, driveFolder);
            });
        }
        public void UploadAll(PCLStorage.IFolder folder, IDriveFolder driveFolder)
        {
            var listFiles = folder.GetFilesAsync().Result;
            foreach (var item in listFiles)
            {
                IDriveApiDriveContentsResult result = (DriveClass.DriveApi.NewDriveContents(mGoogleApiClient).Await()).JavaCast<IDriveApiDriveContentsResult>();
                if (!result.Status.IsSuccess) // handle the error
                    continue;

                System.IO.Stream stream = item.OpenAsync(PCLStorage.FileAccess.Read).Result;
                stream.CopyTo(result.DriveContents.OutputStream);
                MetadataChangeSet changeSet = new MetadataChangeSet.Builder().SetTitle(item.Name).Build();

                driveFolder.CreateFile(mGoogleApiClient, changeSet, result.DriveContents).Await();
            }

            var listFolders = folder.GetFoldersAsync().Result;
            foreach (var item in listFolders)
            {
                MetadataChangeSet changeSet = new MetadataChangeSet.Builder().SetTitle(item.Name).Build();
                IDriveFolderDriveFolderResult result = (driveFolder.CreateFolder(mGoogleApiClient, changeSet).Await()).JavaCast<IDriveFolderDriveFolderResult>();
                if (!result.Status.IsSuccess) // handle the error
                    continue;

                UploadAll(item, result.DriveFolder);
            }
        }
        public Task<List<Metadata>> GetChildrenInFolder(string folderDriveId)
        {
            var ret = Task.Run<List<Metadata>>(() =>
            {
                List<TestGoogleDrive.Metadata> metadatas = new List<TestGoogleDrive.Metadata>();
                try
                {
                    
                    IDriveFolder driveFolder = null;
                    if (string.IsNullOrEmpty(folderDriveId))
                    {
                        driveFolder = DriveClass.DriveApi.GetRootFolder(mGoogleApiClient);
                    }
                    else
                    {
                        IDriveApiDriveIdResult result = (DriveClass.DriveApi.FetchDriveId(mGoogleApiClient, folderDriveId).Await()).JavaCast<IDriveApiDriveIdResult>();
                        if (!result.Status.IsSuccess) // handle the error
                            throw new System.Exception(result.ToString());

                        driveFolder = result.DriveId.AsDriveFolder();
                    }

                    IDriveApiMetadataBufferResult driveApiMetadataBufferResult = (driveFolder.ListChildren(mGoogleApiClient).Await()).JavaCast<IDriveApiMetadataBufferResult>();
                    if (!driveApiMetadataBufferResult.Status.IsSuccess) // handle the error
                        throw new System.Exception(driveApiMetadataBufferResult.ToString());

                    foreach (var item in driveApiMetadataBufferResult.MetadataBuffer)
                    {
                        if (item.IsTrashed)
                            continue;

                        TestGoogleDrive.Metadata metadata = ConvertMetadata(item);
                        metadatas.Add(metadata);
                    }
                    return metadatas;
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    return metadatas;
                }
                
            });
            return ret;
        }

        public Task<bool> NewDriveFolder(string folderDriveId, string folderName)
        {            
            var res = Task.Run<bool>(() =>
            {
                try
                {
                    IDriveFolder driveFolder = null;
                    if (string.IsNullOrEmpty(folderDriveId))
                    {
                        driveFolder = DriveClass.DriveApi.GetRootFolder(mGoogleApiClient);
                    }
                    else
                    {
                        IDriveApiDriveIdResult result = (DriveClass.DriveApi.FetchDriveId(mGoogleApiClient, folderDriveId).Await()).JavaCast<IDriveApiDriveIdResult>();
                        if (!result.Status.IsSuccess) // handle the error
                            throw new System.Exception(result.ToString());

                        driveFolder = result.DriveId.AsDriveFolder();
                    }
                    MetadataChangeSet changeSet = new MetadataChangeSet.Builder().SetTitle(folderName).Build();
                    IDriveFolderDriveFolderResult driveFolderDriveFolderResult = (driveFolder.CreateFolder(mGoogleApiClient, changeSet).Await()).JavaCast<IDriveFolderDriveFolderResult>();

                    if(!driveFolderDriveFolderResult.Status.IsSuccess)
                        throw new System.Exception(driveFolderDriveFolderResult.ToString());

                    return true;
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    return false;
                }
                
            });
            return res;
        }


        static public TestGoogleDrive.Metadata ConvertMetadata(Android.Gms.Drive.Metadata item)
        {
            TestGoogleDrive.Metadata metadata = new TestGoogleDrive.Metadata()
            {
                IsShared = item.IsShared,
                IsStarred = item.IsStarred,
                IsTrashable = item.IsTrashable,
                IsTrashed = item.IsTrashed,
                IsViewed = item.IsTrashed,

                //LastViewedByMeDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(item.LastViewedByMeDate.Time),

                MimeType = item.MimeType,
                ModifiedDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(item.ModifiedDate.Time),
                IsRestricted = item.IsRestricted,
                OriginalFilename = item.OriginalFilename,
                QuotaBytesUsed = item.QuotaBytesUsed,

                //SharedWithMeDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(item.SharedWithMeDate.Time),
                Title = item.Title,
                WebContentLink = item.WebContentLink,
                //ModifiedByMeDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(item.ModifiedByMeDate.Time),
                IsPinned = item.IsPinned,
                IsInAppFolder = item.IsInAppFolder,
                WebViewLink = item.WebViewLink,
                AlternateLink = item.AlternateLink,
                ContentAvailability = item.ContentAvailability,
                CreatedDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(item.CreatedDate.Time),
                IsPinnable = item.IsPinnable,
                DriveId = item.DriveId.ResourceId,
                Description = item.Description,
                FileExtension = item.FileExtension,
                FileSize = item.FileSize,
                IsEditable = item.IsEditable,
                IsExplicitlyTrashed = item.IsExplicitlyTrashed,
                IsFolder = item.IsFolder,
                EmbedLink = item.EmbedLink,
                IsDataValid = item.IsDataValid,
            };
            return metadata;
        }
    }

    
}