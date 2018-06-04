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

    public class GoogleDriveMetadata
    {

        public bool IsShared { get; set; }
        public bool IsStarred { get; set; }
        public bool IsTrashable { get; set; }
        public bool IsTrashed { get; set; }
        public bool IsViewed { get; set; }
        public DateTime LastViewedByMeDate { get; set; }
        public string MimeType { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsRestricted { get; set; }
        public string OriginalFilename { get; set; }
        public long QuotaBytesUsed { get; set; }
        public DateTime SharedWithMeDate { get; set; }
        public string Title { get; set; }
        public string WebContentLink { get; set; }
        public DateTime ModifiedByMeDate { get; set; }
        public bool IsPinned { get; set; }
        public bool IsInAppFolder { get; set; }
        public string WebViewLink { get; set; }
        public string AlternateLink { get; set; }
        public int ContentAvailability { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsPinnable { get; set; }
        public string DriveId { get; set; }
        public string Description { get; set; }
        public string FileExtension { get; set; }
        public long FileSize { get; set; }
        public bool IsEditable { get; set; }
        public bool IsExplicitlyTrashed { get; set; }
        public bool IsFolder { get; set; }
        public string EmbedLink { get; set; }
        public bool IsDataValid { get; set; }
    }
}
