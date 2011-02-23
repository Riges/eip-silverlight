<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LoginFB.aspx.cs" Inherits="EIP.Web.LoginFB" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
    </form>
    <script type="text/javascript">
        window.opener.LoginComplete('<%: this.AccessToken %>', '<%: this.ErrorDescription %>');
    </script>
</body>
</html>
