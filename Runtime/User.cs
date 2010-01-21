using System;
using System.Collections.Generic;
using System.Text;

namespace Rdl.Runtime
{
    public class User
    {
        private Rdl.Engine.Report _rpt;

        public User(Rdl.Engine.Report rpt)
        {
            _rpt = rpt;
        }

        public object this[string key]
        {
            get 
            {
                switch (key)
                {
                    case "UserID":
                        return UserID;
                    case "Language":
                        return Language;
                }
                return null;
            }
        }

        public string UserID
        {
            get { return string.Empty; }
        }

        public string Language
        {
            get { return "English"; }
        }
    }
}
