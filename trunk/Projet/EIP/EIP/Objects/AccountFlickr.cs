using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using EIP.ServiceEIP;
using System.Runtime.Serialization;
using FlickrNet;
using System.Collections.Generic;
using System.IO;

namespace EIP
{
    [KnownTypeAttribute(typeof(AccountFlickrLight))]
    public class AccountFlickrLight : AccountLight
    {
        private Flickr flickr;
        public Person userInfos { get; set; }
        public Dictionary<string, PhotosetCollection> albums { get; set; }
        public Dictionary<string, PhotosetPhotoCollection> photos { get; set; }
        public ContactCollection friends { get; set; }


        

        public AccountFlickrLight()
        {
            this.account = new AccountFlickr();
            this.userInfos = new Person();
            this.albums = new Dictionary<string, PhotosetCollection>();
            this.photos = new Dictionary<string, PhotosetPhotoCollection>();
            this.friends = new ContactCollection();


            Connexion.dispatcher.BeginInvoke(() =>
            {
                this.flickr = new Flickr(Connexion.keyFlickr, Connexion.secretFlickr, ((AccountFlickr)this.account).token);
            });

        }





        public delegate void OnGetUserInfoCompleted(Person user);
        public event OnGetUserInfoCompleted GetUserInfoCalled;

        public void GetUserInfo(string uid)
        {
            flickr.PeopleGetInfoAsync(uid, PeopleGetInfo_Completed);
        }

        private void PeopleGetInfo_Completed(FlickrResult<Person> result)
        {
            userInfos = result.Result;


            if (this.GetUserInfoCalled != null)
                this.GetUserInfoCalled.Invoke(userInfos);
        }

        /***********************************************/
        ////////////////////// Albums ///////////////////
        /***********************************************/

        public delegate void OnGetAlbumsCompleted(PhotosetCollection albums);
        public event OnGetAlbumsCompleted GetAlbumsCalled;

        /// <summary>
        /// Récupérer les albums d'un user
        /// </summary>
        /// <param name="uid">user id</param>
        public void GetAlbums(string uid)
        {
            
            if (!this.albums.ContainsKey(uid))
                this.flickr.PhotosetsGetListAsync(uid, GetAlbums_Completed);
            else
                if (this.GetAlbumsCalled != null)//evite que ca plante si pas dabo
                    this.GetAlbumsCalled.Invoke(this.albums[uid]);
        }

        private void GetAlbums_Completed(FlickrResult<PhotosetCollection> result)
        {
            PhotosetCollection albums = result.Result;

            if (result.Error == null && albums.Count > 0)
            {
                string uid = albums[0].OwnerId;

                this.albums[uid] = albums;

                if (this.GetAlbumsCalled != null)//evite que ca plante si pas dabo
                    this.GetAlbumsCalled.Invoke(this.albums[uid]);
            }
            else
            {
                if (this.GetAlbumsCalled != null)//evite que ca plante si pas dabo
                    this.GetAlbumsCalled.Invoke(new PhotosetCollection());
            }
        }

        /***********************************************/
        ////////////////////// Create album //////////////////
        /***********************************************/

        public delegate void CreateAlbumCompleted(Photoset album);
        public event CreateAlbumCompleted CreateAlbumCalled;

        public void CreateAlbum(string name, string location, string description)
        {
            this.flickr.PhotosetsCreateAsync(name, description, "", CreateAlbum_Completed);
        }

        private void CreateAlbum_Completed(FlickrResult<Photoset> result)
        {
            if(result.Error == null)
                if (this.CreateAlbumCalled != null)
                    this.CreateAlbumCalled.Invoke(result.Result);
        }

        /***********************************************/
        ////////////////////// Photos ///////////////////
        /***********************************************/

        public delegate void OnGetPhotosCompleted(string aid, PhotosetPhotoCollection photos);
        public event OnGetPhotosCompleted GetPhotosCalled;

       
        /// <summary>
        /// Récupérer les photos d'un album
        /// </summary>
        /// <param name="aid">album id</param>
        public void GetPhotos(string aid)
        {
            Photo tof = new Photo();

            if (this.photos.ContainsKey(aid))
            {
                if (this.GetPhotosCalled != null)//evite que ca plante si pas dabo
                    this.GetPhotosCalled.Invoke(aid, this.photos[aid]);
            }
            else
            {
               
                this.flickr.PhotosetsGetPhotosAsync(aid, 1, 500, GetPhotos_Completed);
            }
        }

        private void GetPhotos_Completed(FlickrResult<PhotosetPhotoCollection> result)
        { 
            PhotosetPhotoCollection tofs = result.Result;
            if (result.Error == null && tofs.Count > 0)
            {
                this.photos[tofs.PhotosetId] = tofs;
                if (this.GetPhotosCalled != null)//evite que ca plante si pas dabo
                    this.GetPhotosCalled.Invoke(tofs.PhotosetId, this.photos[tofs.PhotosetId]);
            }
            else
            {
                if (this.GetPhotosCalled != null)//evite que ca plante si pas dabo
                    this.GetPhotosCalled.Invoke(tofs.PhotosetId, new PhotosetPhotoCollection());
            }
          
        }

        /***********************************************/
        /////////////////// Upload Photo ////////////////
        /***********************************************/

        public delegate void UploadPhotoCompleted();
        public event UploadPhotoCompleted UploadPhotoCalled;
        private string uploadPhotoSetID = string.Empty;

        public void UploadPhoto(string aid, string caption, Stream photo, string fileName)
        {
            this.uploadPhotoSetID = aid;
            this.flickr.UploadPictureAsync(photo, fileName, caption, null, null, true, false, false, ContentType.Photo, SafetyLevel.Safe, HiddenFromSearch.Visible, UploadPhoto_Completed);
        }

        private void UploadPhoto_Completed(FlickrResult<string> result)
        {
            if (result.Error == null)
            {
                string photoID = result.Result;

                if (photoID != null && photoID != "")
                {
                    this.flickr.PhotosetsAddPhotoAsync(uploadPhotoSetID, photoID, PhotosetsAddPhoto_Completed);
                }
            }
            //this.uploadPhotoSetID = string.Empty;
        }

        private void PhotosetsAddPhoto_Completed(FlickrResult<NoResponse> result)
        {
            if (this.UploadPhotoCalled != null)//evite que ca plante si pas dabo
                    this.UploadPhotoCalled.Invoke();
        }

        /***********************************************/
        ////////////////////// Friends //////////////////
        /***********************************************/

        public delegate void OnGetFriendsCompleted(ContactCollection friends, long accountID);
        public event OnGetFriendsCompleted GetFriendsCalled;

        public void GetFriends()
        {
            if (this.friends == null || this.friends.Count == 0)
                this.flickr.ContactsGetListAsync(GetFriends_Completed);
            else
            {
                if (this.GetFriendsCalled != null)//evite que ca plante si pas dabo
                    this.GetFriendsCalled.Invoke(this.friends, this.account.accountID);
            }
        }

        private void GetFriends_Completed(FlickrResult<ContactCollection> result)
        {
            if (result.Error == null)
                if (result.Result.Count > 0)
                {
                    this.friends = result.Result;

                    if (this.GetFriendsCalled != null)//evite que ca plante si pas dabo
                        this.GetFriendsCalled.Invoke(this.friends, this.account.accountID);
                }
        }






    }
}
