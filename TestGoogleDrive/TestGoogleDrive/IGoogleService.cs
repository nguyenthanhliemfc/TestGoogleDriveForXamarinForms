using System;
using System.Collections.Generic;
using System.Text;

namespace TestGoogleDrive
{
    public interface IGoogleService
    {
        void Init();
        void SilentSignIn();
        void SignIn(EventHandler eventHandler);
        void SignOut(EventHandler eventHandler);
        void RevokeAccess();
        
        string GetAccount();

        void Connect();
        void Disconnect();
        void NewDriveContent();
    }
}
