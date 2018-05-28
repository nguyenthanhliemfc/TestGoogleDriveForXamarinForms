using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Auth.Api.Credentials;
using Android.Gms.Drive;
using Android.Gms.Common.Apis;
using Android.Gms.Common;
using Android.Util;
using Android.Content;
using System.Threading.Tasks;
using Java.IO;


namespace TestGoogleDrive.Droid
{
    [Activity(Label = "TestGoogleDrive", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,LaunchMode = Android.Content.PM.LaunchMode.SingleTask)]

    [IntentFilter(new[] { Intent.ActionView}, Categories = new[] {  Intent.CategoryBrowsable,Intent.CategoryDefault},
        DataScheme = "com.companyname.TestGoogleDrive")]

    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {        
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);
            GoogleService.Init(this,this);            
            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());            
        }


        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            Log.Debug("MainActivity", "onActivityResult:" + requestCode + ":" + resultCode + ":" + data);
            switch (requestCode)
            {
                case GoogleService.REQUEST_CODE_RESOLUTION:
                    GoogleService.Current.ProcessActivityResult(requestCode, resultCode, data);
                    break;
                default:
                    break;
            }
            
        }     
       
    }
}

