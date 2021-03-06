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
namespace ProtoLinkedIn.ServiceLinkedin {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServiceLinkedin.IService1")]
    public interface IService1 {
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IService1/linkedInConnect", ReplyAction="http://tempuri.org/IService1/linkedInConnectResponse")]
        System.IAsyncResult BeginlinkedInConnect(System.AsyncCallback callback, object asyncState);
        
        string EndlinkedInConnect(System.IAsyncResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IService1Channel : ProtoLinkedIn.ServiceLinkedin.IService1, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class linkedInConnectCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public linkedInConnectCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
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
    public partial class Service1Client : System.ServiceModel.ClientBase<ProtoLinkedIn.ServiceLinkedin.IService1>, ProtoLinkedIn.ServiceLinkedin.IService1 {
        
        private BeginOperationDelegate onBeginlinkedInConnectDelegate;
        
        private EndOperationDelegate onEndlinkedInConnectDelegate;
        
        private System.Threading.SendOrPostCallback onlinkedInConnectCompletedDelegate;
        
        private BeginOperationDelegate onBeginOpenDelegate;
        
        private EndOperationDelegate onEndOpenDelegate;
        
        private System.Threading.SendOrPostCallback onOpenCompletedDelegate;
        
        private BeginOperationDelegate onBeginCloseDelegate;
        
        private EndOperationDelegate onEndCloseDelegate;
        
        private System.Threading.SendOrPostCallback onCloseCompletedDelegate;
        
        public Service1Client() {
        }
        
        public Service1Client(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public Service1Client(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Service1Client(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Service1Client(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
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
        
        public event System.EventHandler<linkedInConnectCompletedEventArgs> linkedInConnectCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> OpenCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> CloseCompleted;
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult ProtoLinkedIn.ServiceLinkedin.IService1.BeginlinkedInConnect(System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginlinkedInConnect(callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        string ProtoLinkedIn.ServiceLinkedin.IService1.EndlinkedInConnect(System.IAsyncResult result) {
            return base.Channel.EndlinkedInConnect(result);
        }
        
        private System.IAsyncResult OnBeginlinkedInConnect(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return ((ProtoLinkedIn.ServiceLinkedin.IService1)(this)).BeginlinkedInConnect(callback, asyncState);
        }
        
        private object[] OnEndlinkedInConnect(System.IAsyncResult result) {
            string retVal = ((ProtoLinkedIn.ServiceLinkedin.IService1)(this)).EndlinkedInConnect(result);
            return new object[] {
                    retVal};
        }
        
        private void OnlinkedInConnectCompleted(object state) {
            if ((this.linkedInConnectCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.linkedInConnectCompleted(this, new linkedInConnectCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void linkedInConnectAsync() {
            this.linkedInConnectAsync(null);
        }
        
        public void linkedInConnectAsync(object userState) {
            if ((this.onBeginlinkedInConnectDelegate == null)) {
                this.onBeginlinkedInConnectDelegate = new BeginOperationDelegate(this.OnBeginlinkedInConnect);
            }
            if ((this.onEndlinkedInConnectDelegate == null)) {
                this.onEndlinkedInConnectDelegate = new EndOperationDelegate(this.OnEndlinkedInConnect);
            }
            if ((this.onlinkedInConnectCompletedDelegate == null)) {
                this.onlinkedInConnectCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnlinkedInConnectCompleted);
            }
            base.InvokeAsync(this.onBeginlinkedInConnectDelegate, null, this.onEndlinkedInConnectDelegate, this.onlinkedInConnectCompletedDelegate, userState);
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
        
        protected override ProtoLinkedIn.ServiceLinkedin.IService1 CreateChannel() {
            return new Service1ClientChannel(this);
        }
        
        private class Service1ClientChannel : ChannelBase<ProtoLinkedIn.ServiceLinkedin.IService1>, ProtoLinkedIn.ServiceLinkedin.IService1 {
            
            public Service1ClientChannel(System.ServiceModel.ClientBase<ProtoLinkedIn.ServiceLinkedin.IService1> client) : 
                    base(client) {
            }
            
            public System.IAsyncResult BeginlinkedInConnect(System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[0];
                System.IAsyncResult _result = base.BeginInvoke("linkedInConnect", _args, callback, asyncState);
                return _result;
            }
            
            public string EndlinkedInConnect(System.IAsyncResult result) {
                object[] _args = new object[0];
                string _result = ((string)(base.EndInvoke("linkedInConnect", _args, result)));
                return _result;
            }
        }
    }
}
