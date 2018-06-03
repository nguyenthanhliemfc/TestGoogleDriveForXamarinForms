using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace TestGoogleDrive
{
	public partial class App : Application
	{
		public App ()
		{
			InitializeComponent();

			MainPage = new MainPage();

            int length = 50;
            for (int i = 0; i < length; i++)
            {
                string filename = i.ToString() + ".Txt";
                string foldername = i.ToString() + "_Folder";
                var ret = PCLStorage.FileSystem.Current.LocalStorage.CreateFileAsync(filename,PCLStorage.CreationCollisionOption.OpenIfExists).Result;
                var re2 = ret.OpenAsync(PCLStorage.FileAccess.ReadAndWrite).Result;
                re2.WriteByte(0);
                re2.Close();
               var sss =  PCLStorage.FileSystem.Current.LocalStorage.CreateFolderAsync(foldername,PCLStorage.CreationCollisionOption.ReplaceExisting).Result;
            }
		}

		protected override void OnStart ()
		{
            // Handle when your app starts
            GoogleService.Current.Connect();
		}

		protected override void OnSleep ()
		{
            // Handle when your app sleeps
           // GoogleService.Current.Disconnect();
		}

		protected override void OnResume ()
		{
            // Handle when your app resumes
            GoogleService.Current.Connect();
        }
	}
}
