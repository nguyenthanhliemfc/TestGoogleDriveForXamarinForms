using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TestGoogleDrive
{
    public interface IGoogleService
    {
        void ChangeAccount();
        void Connect();
        void Disconnect();

        Task<bool> NewDriveContent(string folderDriveId);
        Task<bool> NewDriveFolder(string folderDriveId,string folderName);
        Task<bool> UploadFile(string folderDriveId,string fileName, System.IO.Stream stream);
        Task<List<TestGoogleDrive.GoogleDriveMetadata>> GetChildrenInFolder(string folderDriveId);
    }


}
