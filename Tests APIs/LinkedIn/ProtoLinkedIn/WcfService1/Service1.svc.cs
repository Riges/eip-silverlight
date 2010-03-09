using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using WcfService1.Objects;

namespace WcfService1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    public class Service1 : IService1
    {
        public string linkedInConnect()
        {
            oAuthLinkedIn _oauth = new oAuthLinkedIn();
            string authLink = _oauth.AuthorizationLinkGet();
            return authLink; 
        }
    }
}
