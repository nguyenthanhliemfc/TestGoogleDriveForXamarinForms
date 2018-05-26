using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TestGoogleDrive
{
	public partial class MainPage : ContentPage
	{
        event EventHandler handlerSignIn;
        event EventHandler handlerSignOut;
        
        public MainPage()
		{
			InitializeComponent();

            handlerSignIn += MainPage_handlerSignIn;
            handlerSignOut += MainPage_handlerSignOut;
		}


        private void Button_Clicked(object sender, EventArgs e)
        {

            GoogleService.Current.SignIn(handlerSignIn);
        }

        private void Button_Clicked_SignOut(object sender, EventArgs e)
        {
           
            GoogleService.Current.SignOut(handlerSignOut);
        }

        private void Button_Clicked_Revoke(object sender, EventArgs e)
        {
            GoogleService.Current.RevokeAccess();
        }

        private void MainPage_handlerSignOut(object sender, EventArgs e)
        {
            SignInResult.Text = GoogleService.Current.GetAccount();
        }

        private void MainPage_handlerSignIn(object sender, EventArgs e)
        {
            SignInResult.Text = GoogleService.Current.GetAccount();
        }


        private void Button_Clicked_DriveConnect(object sender, EventArgs e)
        {

            GoogleService.Current.Connect();
        }
        private void Button_Clicked_DriveDisconnect(object sender, EventArgs e)
        {

            GoogleService.Current.Disconnect();
        }

        private void Button_Clicked_NewDriveContent(object sender, EventArgs e)
        {

            GoogleService.Current.NewDriveContent();
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            SignInResult.Text = GoogleService.Current.GetAccount();
        }
    }
}
