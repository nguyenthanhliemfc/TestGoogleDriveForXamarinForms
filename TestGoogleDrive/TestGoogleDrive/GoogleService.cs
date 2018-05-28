using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace TestGoogleDrive
{
    public class GoogleService
    {
        public static IGoogleService Current;       
    }

    public class DriveIdResouece
    {

        public string OriginalFilename { get; }
        public string Title { get; } 
        public string DriveId { get; }
        public string Description { get; }
        public string FileExtension { get; }
        public bool IsFolder { get; }
    }
}
