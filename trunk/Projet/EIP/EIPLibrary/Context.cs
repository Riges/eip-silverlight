using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace EIPLibrary
{
    public class Context
    {
        private static readonly Context _Instance = new Context();
        private readonly string _ConnStr = ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;

        private Context()
        {
        }

        public static Context Instance
        {
            get
            {
                return _Instance;
            }
        }

        public string ConnectStr
        {
            get
            {
                return this._ConnStr;
            }
        }
    }
}
