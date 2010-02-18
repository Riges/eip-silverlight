﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.21006.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This code was auto-generated by Microsoft.Silverlight.ServiceReference, version 4.0.41108.0
// 
namespace EIP.ServiceEIP {
    using System.Runtime.Serialization;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Account", Namespace="http://schemas.datacontract.org/2004/07/EIP")]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(EIP.ServiceEIP.AccountTwitter))]
    public partial class Account : object, System.ComponentModel.INotifyPropertyChanged {
        
        private long accountIDField;
        
        private string nameField;
        
        private EIP.ServiceEIP.Account.TypeAccount typeAccountField;
        
        private long userIDField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long accountID {
            get {
                return this.accountIDField;
            }
            set {
                if ((this.accountIDField.Equals(value) != true)) {
                    this.accountIDField = value;
                    this.RaisePropertyChanged("accountID");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string name {
            get {
                return this.nameField;
            }
            set {
                if ((object.ReferenceEquals(this.nameField, value) != true)) {
                    this.nameField = value;
                    this.RaisePropertyChanged("name");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public EIP.ServiceEIP.Account.TypeAccount typeAccount {
            get {
                return this.typeAccountField;
            }
            set {
                if ((this.typeAccountField.Equals(value) != true)) {
                    this.typeAccountField = value;
                    this.RaisePropertyChanged("typeAccount");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long userID {
            get {
                return this.userIDField;
            }
            set {
                if ((this.userIDField.Equals(value) != true)) {
                    this.userIDField = value;
                    this.RaisePropertyChanged("userID");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
        [System.Runtime.Serialization.DataContractAttribute(Name="Account.TypeAccount", Namespace="http://schemas.datacontract.org/2004/07/EIP")]
        public enum TypeAccount : int {
            
            [System.Runtime.Serialization.EnumMemberAttribute()]
            Facebook = 0,
            
            [System.Runtime.Serialization.EnumMemberAttribute()]
            Twitter = 1,
            
            [System.Runtime.Serialization.EnumMemberAttribute()]
            Myspace = 2,
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="AccountTwitter", Namespace="http://schemas.datacontract.org/2004/07/EIP")]
    public partial class AccountTwitter : EIP.ServiceEIP.Account {
        
        private string tokenField;
        
        private string tokenSecretField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string token {
            get {
                return this.tokenField;
            }
            set {
                if ((object.ReferenceEquals(this.tokenField, value) != true)) {
                    this.tokenField = value;
                    this.RaisePropertyChanged("token");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string tokenSecret {
            get {
                return this.tokenSecretField;
            }
            set {
                if ((object.ReferenceEquals(this.tokenSecretField, value) != true)) {
                    this.tokenSecretField = value;
                    this.RaisePropertyChanged("tokenSecret");
                }
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServiceEIP.IServiceEIP")]
    public interface IServiceEIP {
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IServiceEIP/AuthorizeDesktop", ReplyAction="http://tempuri.org/IServiceEIP/AuthorizeDesktopResponse")]
        System.IAsyncResult BeginAuthorizeDesktop(string consumerKey, string consumerSecret, System.AsyncCallback callback, object asyncState);
        
        string EndAuthorizeDesktop(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IServiceEIP/GetAccessToken", ReplyAction="http://tempuri.org/IServiceEIP/GetAccessTokenResponse")]
        System.IAsyncResult BeginGetAccessToken(string consumerKey, string consumerSecret, string token, string pin, System.AsyncCallback callback, object asyncState);
        
        EIP.ServiceEIP.AccountTwitter EndGetAccessToken(System.IAsyncResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IServiceEIPChannel : EIP.ServiceEIP.IServiceEIP, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class AuthorizeDesktopCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public AuthorizeDesktopCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public string Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GetAccessTokenCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public GetAccessTokenCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public EIP.ServiceEIP.AccountTwitter Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((EIP.ServiceEIP.AccountTwitter)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ServiceEIPClient : System.ServiceModel.ClientBase<EIP.ServiceEIP.IServiceEIP>, EIP.ServiceEIP.IServiceEIP {
        
        private BeginOperationDelegate onBeginAuthorizeDesktopDelegate;
        
        private EndOperationDelegate onEndAuthorizeDesktopDelegate;
        
        private System.Threading.SendOrPostCallback onAuthorizeDesktopCompletedDelegate;
        
        private BeginOperationDelegate onBeginGetAccessTokenDelegate;
        
        private EndOperationDelegate onEndGetAccessTokenDelegate;
        
        private System.Threading.SendOrPostCallback onGetAccessTokenCompletedDelegate;
        
        private BeginOperationDelegate onBeginOpenDelegate;
        
        private EndOperationDelegate onEndOpenDelegate;
        
        private System.Threading.SendOrPostCallback onOpenCompletedDelegate;
        
        private BeginOperationDelegate onBeginCloseDelegate;
        
        private EndOperationDelegate onEndCloseDelegate;
        
        private System.Threading.SendOrPostCallback onCloseCompletedDelegate;
        
        public ServiceEIPClient() {
        }
        
        public ServiceEIPClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ServiceEIPClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServiceEIPClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServiceEIPClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.Net.CookieContainer CookieContainer {
            get {
                System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
                if ((httpCookieContainerManager != null)) {
                    return httpCookieContainerManager.CookieContainer;
                }
                else {
                    return null;
                }
            }
            set {
                System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
                if ((httpCookieContainerManager != null)) {
                    httpCookieContainerManager.CookieContainer = value;
                }
                else {
                    throw new System.InvalidOperationException("Unable to set the CookieContainer. Please make sure the binding contains an HttpC" +
                            "ookieContainerBindingElement.");
                }
            }
        }
        
        public event System.EventHandler<AuthorizeDesktopCompletedEventArgs> AuthorizeDesktopCompleted;
        
        public event System.EventHandler<GetAccessTokenCompletedEventArgs> GetAccessTokenCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> OpenCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> CloseCompleted;
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult EIP.ServiceEIP.IServiceEIP.BeginAuthorizeDesktop(string consumerKey, string consumerSecret, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginAuthorizeDesktop(consumerKey, consumerSecret, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        string EIP.ServiceEIP.IServiceEIP.EndAuthorizeDesktop(System.IAsyncResult result) {
            return base.Channel.EndAuthorizeDesktop(result);
        }
        
        private System.IAsyncResult OnBeginAuthorizeDesktop(object[] inValues, System.AsyncCallback callback, object asyncState) {
            string consumerKey = ((string)(inValues[0]));
            string consumerSecret = ((string)(inValues[1]));
            return ((EIP.ServiceEIP.IServiceEIP)(this)).BeginAuthorizeDesktop(consumerKey, consumerSecret, callback, asyncState);
        }
        
        private object[] OnEndAuthorizeDesktop(System.IAsyncResult result) {
            string retVal = ((EIP.ServiceEIP.IServiceEIP)(this)).EndAuthorizeDesktop(result);
            return new object[] {
                    retVal};
        }
        
        private void OnAuthorizeDesktopCompleted(object state) {
            if ((this.AuthorizeDesktopCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.AuthorizeDesktopCompleted(this, new AuthorizeDesktopCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void AuthorizeDesktopAsync(string consumerKey, string consumerSecret) {
            this.AuthorizeDesktopAsync(consumerKey, consumerSecret, null);
        }
        
        public void AuthorizeDesktopAsync(string consumerKey, string consumerSecret, object userState) {
            if ((this.onBeginAuthorizeDesktopDelegate == null)) {
                this.onBeginAuthorizeDesktopDelegate = new BeginOperationDelegate(this.OnBeginAuthorizeDesktop);
            }
            if ((this.onEndAuthorizeDesktopDelegate == null)) {
                this.onEndAuthorizeDesktopDelegate = new EndOperationDelegate(this.OnEndAuthorizeDesktop);
            }
            if ((this.onAuthorizeDesktopCompletedDelegate == null)) {
                this.onAuthorizeDesktopCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnAuthorizeDesktopCompleted);
            }
            base.InvokeAsync(this.onBeginAuthorizeDesktopDelegate, new object[] {
                        consumerKey,
                        consumerSecret}, this.onEndAuthorizeDesktopDelegate, this.onAuthorizeDesktopCompletedDelegate, userState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult EIP.ServiceEIP.IServiceEIP.BeginGetAccessToken(string consumerKey, string consumerSecret, string token, string pin, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginGetAccessToken(consumerKey, consumerSecret, token, pin, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        EIP.ServiceEIP.AccountTwitter EIP.ServiceEIP.IServiceEIP.EndGetAccessToken(System.IAsyncResult result) {
            return base.Channel.EndGetAccessToken(result);
        }
        
        private System.IAsyncResult OnBeginGetAccessToken(object[] inValues, System.AsyncCallback callback, object asyncState) {
            string consumerKey = ((string)(inValues[0]));
            string consumerSecret = ((string)(inValues[1]));
            string token = ((string)(inValues[2]));
            string pin = ((string)(inValues[3]));
            return ((EIP.ServiceEIP.IServiceEIP)(this)).BeginGetAccessToken(consumerKey, consumerSecret, token, pin, callback, asyncState);
        }
        
        private object[] OnEndGetAccessToken(System.IAsyncResult result) {
            EIP.ServiceEIP.AccountTwitter retVal = ((EIP.ServiceEIP.IServiceEIP)(this)).EndGetAccessToken(result);
            return new object[] {
                    retVal};
        }
        
        private void OnGetAccessTokenCompleted(object state) {
            if ((this.GetAccessTokenCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.GetAccessTokenCompleted(this, new GetAccessTokenCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void GetAccessTokenAsync(string consumerKey, string consumerSecret, string token, string pin) {
            this.GetAccessTokenAsync(consumerKey, consumerSecret, token, pin, null);
        }
        
        public void GetAccessTokenAsync(string consumerKey, string consumerSecret, string token, string pin, object userState) {
            if ((this.onBeginGetAccessTokenDelegate == null)) {
                this.onBeginGetAccessTokenDelegate = new BeginOperationDelegate(this.OnBeginGetAccessToken);
            }
            if ((this.onEndGetAccessTokenDelegate == null)) {
                this.onEndGetAccessTokenDelegate = new EndOperationDelegate(this.OnEndGetAccessToken);
            }
            if ((this.onGetAccessTokenCompletedDelegate == null)) {
                this.onGetAccessTokenCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnGetAccessTokenCompleted);
            }
            base.InvokeAsync(this.onBeginGetAccessTokenDelegate, new object[] {
                        consumerKey,
                        consumerSecret,
                        token,
                        pin}, this.onEndGetAccessTokenDelegate, this.onGetAccessTokenCompletedDelegate, userState);
        }
        
        private System.IAsyncResult OnBeginOpen(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return ((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(callback, asyncState);
        }
        
        private object[] OnEndOpen(System.IAsyncResult result) {
            ((System.ServiceModel.ICommunicationObject)(this)).EndOpen(result);
            return null;
        }
        
        private void OnOpenCompleted(object state) {
            if ((this.OpenCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.OpenCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void OpenAsync() {
            this.OpenAsync(null);
        }
        
        public void OpenAsync(object userState) {
            if ((this.onBeginOpenDelegate == null)) {
                this.onBeginOpenDelegate = new BeginOperationDelegate(this.OnBeginOpen);
            }
            if ((this.onEndOpenDelegate == null)) {
                this.onEndOpenDelegate = new EndOperationDelegate(this.OnEndOpen);
            }
            if ((this.onOpenCompletedDelegate == null)) {
                this.onOpenCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnOpenCompleted);
            }
            base.InvokeAsync(this.onBeginOpenDelegate, null, this.onEndOpenDelegate, this.onOpenCompletedDelegate, userState);
        }
        
        private System.IAsyncResult OnBeginClose(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return ((System.ServiceModel.ICommunicationObject)(this)).BeginClose(callback, asyncState);
        }
        
        private object[] OnEndClose(System.IAsyncResult result) {
            ((System.ServiceModel.ICommunicationObject)(this)).EndClose(result);
            return null;
        }
        
        private void OnCloseCompleted(object state) {
            if ((this.CloseCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.CloseCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void CloseAsync() {
            this.CloseAsync(null);
        }
        
        public void CloseAsync(object userState) {
            if ((this.onBeginCloseDelegate == null)) {
                this.onBeginCloseDelegate = new BeginOperationDelegate(this.OnBeginClose);
            }
            if ((this.onEndCloseDelegate == null)) {
                this.onEndCloseDelegate = new EndOperationDelegate(this.OnEndClose);
            }
            if ((this.onCloseCompletedDelegate == null)) {
                this.onCloseCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnCloseCompleted);
            }
            base.InvokeAsync(this.onBeginCloseDelegate, null, this.onEndCloseDelegate, this.onCloseCompletedDelegate, userState);
        }
        
        protected override EIP.ServiceEIP.IServiceEIP CreateChannel() {
            return new ServiceEIPClientChannel(this);
        }
        
        private class ServiceEIPClientChannel : ChannelBase<EIP.ServiceEIP.IServiceEIP>, EIP.ServiceEIP.IServiceEIP {
            
            public ServiceEIPClientChannel(System.ServiceModel.ClientBase<EIP.ServiceEIP.IServiceEIP> client) : 
                    base(client) {
            }
            
            public System.IAsyncResult BeginAuthorizeDesktop(string consumerKey, string consumerSecret, System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[2];
                _args[0] = consumerKey;
                _args[1] = consumerSecret;
                System.IAsyncResult _result = base.BeginInvoke("AuthorizeDesktop", _args, callback, asyncState);
                return _result;
            }
            
            public string EndAuthorizeDesktop(System.IAsyncResult result) {
                object[] _args = new object[0];
                string _result = ((string)(base.EndInvoke("AuthorizeDesktop", _args, result)));
                return _result;
            }
            
            public System.IAsyncResult BeginGetAccessToken(string consumerKey, string consumerSecret, string token, string pin, System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[4];
                _args[0] = consumerKey;
                _args[1] = consumerSecret;
                _args[2] = token;
                _args[3] = pin;
                System.IAsyncResult _result = base.BeginInvoke("GetAccessToken", _args, callback, asyncState);
                return _result;
            }
            
            public EIP.ServiceEIP.AccountTwitter EndGetAccessToken(System.IAsyncResult result) {
                object[] _args = new object[0];
                EIP.ServiceEIP.AccountTwitter _result = ((EIP.ServiceEIP.AccountTwitter)(base.EndInvoke("GetAccessToken", _args, result)));
                return _result;
            }
        }
    }
}
