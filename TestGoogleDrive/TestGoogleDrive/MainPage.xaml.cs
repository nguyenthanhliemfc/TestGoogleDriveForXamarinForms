﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TestGoogleDrive
{
	public partial class MainPage : ContentPage
	{
        
        public MainPage()
		{
			InitializeComponent();


		}

        
        private void Button_Clicked_Revoke(object sender, EventArgs e)
        {
            GoogleService.Current.ChangeAccount();
        }


        private void Button_Clicked_1(object sender, EventArgs e)
        {

            GoogleService.Current.NewDriveContent();
        }
        private void Button_Clicked_2(object sender, EventArgs e)
        {

            GoogleService.Current.Test();
        }

        private void Button_Clicked_3(object sender, EventArgs e)
        {

            GoogleService.Current.Test();
        }

    }
}
