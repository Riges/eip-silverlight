<%@ Page Language="C#" AutoEventWireup="true" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">

    <script src="http://static.ak.connect.facebook.com/js/api_lib/v0.4/FeatureLoader.js.php" type="text/javascript"> </script>
    <script type="text/javascript" src="fblogin.js"></script>
    
    <link rel="shortcut icon" href="images/favicon.ico">
    <link rel="icon" type="image/gif" href="images/animated_favicon1.gif">
    
    <title>EIP</title>
    <style type="text/css">
    html, body {
	    height: 100%;
	    overflow: auto;
    }
    body {
	    padding: 0;
	    margin: 0;
    }
    #silverlightControlHost {
	    height: 100%;
	    text-align:center;
    }
    </style>

    

    <script type="text/javascript" src="Silverlight.js"></script>
    <script type="text/javascript">
        function onSilverlightError(sender, args) {
            var appSource = "";
            if (sender != null && sender != 0) {
              appSource = sender.getHost().Source;
            }
            
            var errorType = args.ErrorType;
            var iErrorCode = args.ErrorCode;

            if (errorType == "ImageError" || errorType == "MediaError") {
              return;
            }

            var errMsg = "Unhandled Error in Silverlight Application " +  appSource + "\n" ;

            errMsg += "Code: "+ iErrorCode + "    \n";
            errMsg += "Category: " + errorType + "       \n";
            errMsg += "Message: " + args.ErrorMessage + "     \n";

            if (errorType == "ParserError") {
                errMsg += "File: " + args.xamlFile + "     \n";
                errMsg += "Line: " + args.lineNumber + "     \n";
                errMsg += "Position: " + args.charPosition + "     \n";
            }
            else if (errorType == "RuntimeError") {           
                if (args.lineNumber != 0) {
                    errMsg += "Line: " + args.lineNumber + "     \n";
                    errMsg += "Position: " +  args.charPosition + "     \n";
                }
                errMsg += "MethodName: " + args.methodName + "     \n";
            }

            throw new Error(errMsg);
        }
    </script>
    <script type="text/javascript">
        var slPluginId = '_sl_facebookapp';

        function getSlPlugin() {
            if (typeof (window.slPlugin) == "undefined" || window.slPlugin == null) {
                window.slPlugin = document.getElementById(slPluginId);
            }
            return window.slPlugin.Content.slObject;
        }
        window.loginDialog = null;
        function fbLogin(uri)
        {
            window.loginDialog = window.open(uri, "loginDialog", "height=320,width=480,location=no,menubar=no,toolbar=no");
        }
        window.LoginComplete = function (accessToken, errorDescription)
        {
            if (window.loginDialog != null) {
                window.loginDialog.close();
            }
            getSlPlugin().LoginComplete(accessToken, errorDescription);
        }

        window.logoutDialog = null;
        function fblogout(uri)
        {
            window.logoutDialog = window.open(uri, "logoutDialog", "height=320,width=480,location=no,menubar=no,toolbar=no");
        }

    </script>

</head>
<body>
    <form id="form1" runat="server" style="height:100%">

    <%--<div id="fb-root"></div>
<script>
    window.fbAsyncInit = function () {
        FB.init({ appId: 'ye0c1f6b95b88d23bfc9727e0ea90602a', status: true, cookie: true,
            xfbml: true
        });
    };
    (function () {
        var e = document.createElement('script'); e.async = true;
        e.src = document.location.protocol +
      '//connect.facebook.net/en_US/all.js';
        document.getElementById('fb-root').appendChild(e);
    } ());
</script>--%>
     
    <div id="silverlightControlHost">
        <object data="data:application/x-silverlight-2," type="application/x-silverlight-2" width="100%" height="100%" id="_sl_facebookapp">
		  <param name="source" value="ClientBin/EIP.xap"/>
		  <param name="onError" value="onSilverlightError" />
		  <param name="background" value="white" />
         <param name="Windowless" value="bool" />
		  <param name="minRuntimeVersion" value="4.0.50401.0" /> <!-- 4.0.50401.0  4.0.41108.0 -->
		  <param name="autoUpgrade" value="true" />
		  <a href="http://go.microsoft.com/fwlink/?LinkID=149156&v=4.0.50401.0" style="text-decoration:none">
 			  <img src="http://go.microsoft.com/fwlink/?LinkId=161376" alt="Get Microsoft Silverlight" style="border-style:none"/>
		  </a>
	    </object><iframe id="_sl_historyFrame" style="visibility:hidden;height:0px;width:0px;border:0px"></iframe></div>
       
    </form>
   <%-- <iframe id="ifHtmlContent" style="position:absolute"/>--%>
</body>
</html>
