using System;
using System.Collections.Generic;
using System.Text;

namespace RdlRuntime
{
    public class CredentialsPromptEventArgs : EventArgs
    {
        public string Prompt;
        public string UserID;
        public string Password;
    }
}
