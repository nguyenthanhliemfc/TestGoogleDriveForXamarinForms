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
		}

		protected override void OnStart ()
		{
            // Handle when your app starts
            GoogleService.Current.Connect();
		}

		protected override void OnSleep ()
		{
            // Handle when your app sleeps
            GoogleService.Current.Disconnect();
		}

		protected override void OnResume ()
		{
            // Handle when your app resumes
            GoogleService.Current.Connect();
        }
	}
}
