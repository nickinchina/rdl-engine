using System;
using System.Collections.Generic;
using System.Text;

namespace Rdl.Runtime
{
    public class CredentialsPromptEventArgs : EventArgs
    {
        public string Prompt;
        public string UserID;
        public string Password;
    }
}
