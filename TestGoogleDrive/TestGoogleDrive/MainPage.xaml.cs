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
        Stack<Metadata> metadatas = new Stack<Metadata>();
        public MainPage()
		{
			InitializeComponent();

            ListDrive.ItemSelected += ListDrive_ItemSelected;
            //EventHandler eventHandler = GoogleService.Current.GetEventHandlerDriveUpdated();
            //eventHandler += MainPage_EventDriveConnected;
            UpdateGDrivePath();
        } 

        private void ListDrive_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Metadata metadata = e.SelectedItem as Metadata;
            if (String.IsNullOrEmpty(metadata.DriveId))
                return;

            if(metadata.IsFolder)
            {
                metadatas.Push(metadata);
                UpdateGDrivePath();
                UpdateGDriveList();
            }
            else
            {

            }
            
        }

        
  
        private void MainPage_eventTest(object sender, TestGoogleDrive.ListEventArgs e)
        {            
            
            ListDrive.ItemsSource = e.Metadatas;

          
        }

        private void Button_Clicked_Revoke(object sender, EventArgs e)
        {
            GoogleService.Current.ChangeAccount();
        }


        private async void Button_Clicked_Folder(object sender, EventArgs e)
        {
            bool result = await GoogleService.Current.NewDriveFolder(GetCurrGDriveId(), "NewFolder");
            if(result)
                UpdateGDriveList();
                      
        }
        private void Button_Clicked_Update(object sender, EventArgs e)
        {
            UpdateGDriveList();          
        }

        private async void Button_Clicked_3(object sender, EventArgs e)
        {
            var ret = await Plugin.FilePicker.CrossFilePicker.Current.PickFile();
            if (ret == null)
                return;

            var result = await GoogleService.Current.UploadFile(GetCurrGDriveId(), ret.FileName, ret.GetStream());
            if(result)
                UpdateGDriveList();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            switch (button.Text)
            {
                case "Create_Folder":
                    break;
                default:
                    break;
            }

        }

        private async void Button_Clicked_File(object sender, EventArgs e)
        {
            var result = await GoogleService.Current.NewDriveContent(GetCurrGDriveId());
            if (result)
                UpdateGDriveList();
        }

        private async void UpdateGDriveList()
        {           
            var ret = await GoogleService.Current.GetChildrenInFolder(GetCurrGDriveId());
            ListDrive.ItemsSource = ret;
        }

        private void Button_Clicked_DriveBack(object sender, EventArgs e)
        {
            if (metadatas.Count == 0)
                return;

            metadatas.Pop();
            UpdateGDrivePath();
            UpdateGDriveList();
        }
        private void UpdateGDrivePath()
        {
            string path = "GDrive:/";
            foreach (var item in metadatas)
            {
                path += item.Title;
                path += "/";

            }
            DrivePath.Text = path;
        }
        private string GetCurrGDriveId()
        {
            string driveId = "";
            if (metadatas.Count != 0)
            {
                driveId = metadatas.Peek().DriveId;
            }
            return driveId;
        }
    }
}
