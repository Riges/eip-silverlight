﻿// Verify this variable matches the Silverlight plugin ID
var silverlightPluginId = '_sl_facebookapp';

function facebook_init(appid) {
    FB.init(appid, "/xd_receiver.htm");
}

function isUserConnected() {
    FB.ensureInit(function () {
        FB.Connect.get_status().waitUntilReady(function (status) {
            var plugin = document.getElementById(silverlightPluginId);
        });
    });
}

function facebook_login() {
    FB.ensureInit(function () {
        FB.Connect.requireSession(facebook_getSession, true);
        facebook_prompt_permission("read_stream");
        facebook_prompt_permission("offline_access");
        facebook_prompt_permission("publish_stream");
    });
}

function facebook_logout() {
    FB.Connect.logout(facebook_onlogout);
}

function facebook_getSession() {

    FB.Facebook.get_sessionState().waitUntilReady(function () {
        var session = FB.Facebook.apiClient.get_session();
        var plugin = document.getElementById(silverlightPluginId);
        plugin.Content.FacebookLoginControl.LoggedIn(session.session_key, session.secret, session.expires, session.uid);
    });
}

function facebook_onlogout() {
    var plugin = document.getElementById(silverlightPluginId);
    plugin.Content.FacebookLoginControl.LoggedOut();
}

function facebook_onpermission(accepted) {
    var plugin = document.getElementById(silverlightPluginId);
    plugin.Content.FacebookLoginControl.PermissionCallback(accepted);
}

function facebook_prompt_permission(permission) {
    FB.ensureInit(function () {
        FB.Connect.showPermissionDialog(permission, facebook_onpermission);
    });
}