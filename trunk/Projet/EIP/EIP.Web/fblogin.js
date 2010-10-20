// Verify this variable matches the Silverlight plugin ID
var silverlightPluginId = '_sl_facebookapp';



function facebook_init(appid) {
    FB.init(appid, "/xd_receiver.htm");


    /*
   FB.Bootstrap.requireFeatures(["Connect"], function()
	    {
	        FB.Facebook.init(appid, "/xd_receiver.htm");
	        FB.Connect.requireSession(function()
	        {
	            FB.Facebook.apiClient.users_hasAppPermission("read_mailbox", function(result)
	            {
	                if (result != 0)
	                    return callback(true);
	 
	                FB.Connect.showPermissionDialog(permission, function(result)
	                {
	                    alert(result == null ? " not granted" : " granted");
	                   
	                }, true, null);
	            });
	        });
	    });

*/
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
        facebook_prompt_permission("read_mailbox");
        facebook_prompt_permission("read_stream");
        facebook_prompt_permission("offline_access");
        facebook_prompt_permission("publish_stream");


        facebook_prompt_permission("read_requests");
        facebook_prompt_permission("read_insights");
        facebook_prompt_permission("read_friendlists"); 


        facebook_prompt_permission("user_photos");
        facebook_prompt_permission("friends_photos");
          	
        facebook_prompt_permission("user_birthday");
        facebook_prompt_permission("friends_birthday");

        facebook_prompt_permission("user_activities");
        facebook_prompt_permission("friends_activities");
          	
        facebook_prompt_permission("user_education_history");
        facebook_prompt_permission("friends_education_history");

        facebook_prompt_permission("user_hometown"); 
        facebook_prompt_permission("friends_hometown");

        facebook_prompt_permission("user_interests");
        facebook_prompt_permission("friends_interests");

        facebook_prompt_permission("user_likes");
        facebook_prompt_permission("friends_likes");

        facebook_prompt_permission("user_location");
        facebook_prompt_permission("friends_location");

        facebook_prompt_permission("user_online_presence");
        facebook_prompt_permission("friends_online_presence");

        facebook_prompt_permission("user_photo_video_tags");
        facebook_prompt_permission("friends_photo_video_tags"); 
          
        facebook_prompt_permission("user_relationships");   
        facebook_prompt_permission("friends_relationships");   

        facebook_prompt_permission("user_relationship_details");   
        facebook_prompt_permission("friends_relationship_details");  
         
        facebook_prompt_permission("user_religion_politics");   
        facebook_prompt_permission("friends_religion_politics");  
         
        facebook_prompt_permission("user_status");   
        facebook_prompt_permission("friends_status"); 
          
        facebook_prompt_permission("user_videos");   
        facebook_prompt_permission("friends_videos"); 
          
        facebook_prompt_permission("user_website");   
        facebook_prompt_permission("friends_website"); 
          
        facebook_prompt_permission("user_work_history");   
        facebook_prompt_permission("friends_work_history");   

             
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