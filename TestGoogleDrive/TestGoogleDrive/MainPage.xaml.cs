using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TestGoogleDrive
{
    public class LocalDriveMetadata
    {
        public bool IsFolder { get; set; }
        public string Title { get; set; }
        public DateTime ModifiedDate { get; set; }
        public PCLStorage.IFile File { get; set; }
        public PCLStorage.IFolder Folder { get; set; }
    }

    public partial class MainPage : ContentPage
	{         
        Stack<GoogleDriveMetadata> gdriveFolders = new Stack<GoogleDriveMetadata>();
        Stack<LocalDriveMetadata> ldriveFolders = new Stack<LocalDriveMetadata>();
        public MainPage()
		{
			InitializeComponent();

            ListDrive.ItemSelected += ListDrive_ItemSelected;
            ListLDrive.ItemSelected += ListLDrive_ItemSelected;

            UpdateGDrivePath();
            UpdateLDrivePath();
        }

        private void ListLDrive_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            LocalDriveMetadata localDriveMetadata = e.SelectedItem as LocalDriveMetadata;
        
            if (localDriveMetadata.IsFolder)
            {
                ldriveFolders.Push(localDriveMetadata);
                UpdateLDrivePath();
                UpdateLDriveList();
            }
            else
            {

            }
        }

        private void ListDrive_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            GoogleDriveMetadata metadata = e.SelectedItem as GoogleDriveMetadata;
            if (String.IsNullOrEmpty(metadata.DriveId))
                return;

            if(metadata.IsFolder)
            {
                gdriveFolders.Push(metadata);
                UpdateGDrivePath();
                UpdateGDriveList();
            }
            else
            {

            }
            
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
            UpdateLDriveList();
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

        private async void UpdateLDriveList()
        {
            List<LocalDriveMetadata> localDriveMetadatas = new List<LocalDriveMetadata>();

            var resultFolders = await GetCurrLDriveId().GetFoldersAsync();
            foreach (var item in resultFolders)
            {
                LocalDriveMetadata localDriveMetadata = new LocalDriveMetadata()
                {
                    IsFolder = true,
                    Title = item.Name,
                    Folder = item
                };

                localDriveMetadatas.Add(localDriveMetadata);
            }

            var resultFiles = await GetCurrLDriveId().GetFilesAsync();
            foreach (var item in resultFiles)
            {
                LocalDriveMetadata localDriveMetadata = new LocalDriveMetadata()
                {
                    IsFolder = false,
                    Title = item.Name,
                    File = item
                };
                localDriveMetadatas.Add(localDriveMetadata);
            }


            ListLDrive.ItemsSource = localDriveMetadatas;
        }



        private void Button_Clicked_DriveBack(object sender, EventArgs e)
        {
            if (gdriveFolders.Count == 0)
                return;

            gdriveFolders.Pop();
            UpdateGDrivePath();
            UpdateGDriveList();
        }

        private void Button_Clicked_LDriveBack(object sender, EventArgs e)
        {
            if (ldriveFolders.Count == 0)
                return;

            ldriveFolders.Pop();
            UpdateLDrivePath();
            UpdateLDriveList();
        }

        private void UpdateGDrivePath()
        {
            string path = "GDrive:/";
            foreach (var item in gdriveFolders)
            {
                path += item.Title;
                path += "/";

            }
            GDrivePath.Text = path;
        }
        private string GetCurrGDriveId()
        {
            string driveId = "";
            if (gdriveFolders.Count != 0)
            {
                driveId = gdriveFolders.Peek().DriveId;
            }
            return driveId;
        }
        private PCLStorage.IFolder GetCurrLDriveId()
        {
            PCLStorage.IFolder folder = PCLStorage.FileSystem.Current.LocalStorage;
            if (ldriveFolders.Count != 0)
            {
                folder = ldriveFolders.Peek().Folder;
            }
            return folder;
        }

        private void UpdateLDrivePath()
        {
            string path = "LDrive:/";
            foreach (var item in ldriveFolders)
            {
                path += item.Folder.Name;
                path += "/";

            }
            LDrivePath.Text = path;
        }

        private void MenuItem_Clicked_LDelete(object sender, EventArgs e)
        {

        }
        private void MenuItem_Clicked_Upload(object sender, EventArgs e)
        {

        }
        private void MenuItem_Clicked_GDelete(object sender, EventArgs e)
        {

        }
        private void MenuItem_Clicked_Download(object sender, EventArgs e)
        {

        }
    }
}
