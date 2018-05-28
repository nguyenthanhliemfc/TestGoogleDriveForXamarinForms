using System;
using System.Collections.Generic;
using System.Text;

namespace TestGoogleDrive
{
    public interface IGoogleService
    {
        void ChangeAccount();
        void Connect();
        void Disconnect();
        void NewDriveContent();

        void Test();
    }
}
