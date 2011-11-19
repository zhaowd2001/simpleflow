using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AMicroblogAPISample
{
    public class UserData : PropertyChangeNotifier
    {
        /// <remarks/>
        private long _ID;
        public long ID
        {
            get
            {
                return _ID;
            }
            set
            {
                if (value != _ID)
                {
                    _ID = value;
                    OnPropertyChanged("ID");
                }
            }
        }

        /// <remarks/>
        private string _ScreenName;
        public string ScreenName
        {
            get
            {
                return _ScreenName;
            }
            set
            {
                if (value != _ScreenName)
                {
                    _ScreenName = value;
                    OnPropertyChanged("ScreenName");
                }
            }
        }

        /// <remarks/>
        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                if (value != _Name)
                {
                    _Name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        /// <summary>
        /// Gets or sets the markup(note) you set for this user.
        /// </summary>
        private string _DefineAs;
        public string DefineAs
        {
            get
            {
                return _DefineAs;
            }
            set
            {
                if (value != _DefineAs)
                {
                    _DefineAs = value;
                    OnPropertyChanged("DefineAs");
                }
            }
        }

        /// <remarks/>
        private string _Province;
        public string Province
        {
            get
            {
                return _Province;
            }
            set
            {
                if (value != _Province)
                {
                    _Province = value;
                    OnPropertyChanged("Province");
                }
            }
        }

        /// <remarks/>
        private string _City;
        public string City
        {
            get
            {
                return _City;
            }
            set
            {
                if (value != _City)
                {
                    _City = value;
                    OnPropertyChanged("City");
                }
            }
        }

        /// <remarks/>
        private string _Location;
        public string Location
        {
            get
            {
                return _Location;
            }
            set
            {
                if (value != _Location)
                {
                    _Location = value;
                    OnPropertyChanged("Location");
                }
            }
        }

        /// <remarks/>
        private string _Description;
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                if (value != _Description)
                {
                    _Description = value;
                    OnPropertyChanged("Description");
                }
            }
        }

        /// <remarks/>
        private string _Url;
        public string Url
        {
            get
            {
                return _Url;
            }
            set
            {
                if (value != _Url)
                {
                    _Url = value;
                    OnPropertyChanged("Url");
                }
            }
        }

        /// <remarks/>
        private string _ProfileImageUrl;
        public string ProfileImageUrl
        {
            get
            {
                return _ProfileImageUrl;
            }
            set
            {
                if (value != _ProfileImageUrl)
                {
                    _ProfileImageUrl = value;
                    OnPropertyChanged("ProfileImageUrl");
                }
            }
        }

        /// <remarks/>
        private string _Domain;
        public string Domain
        {
            get
            {
                return _Domain;
            }
            set
            {
                if (value != _Domain)
                {
                    _Domain = value;
                    OnPropertyChanged("Domain");
                }
            }
        }

        /// <remarks/>
        private string _Gender;
        public string Gender
        {
            get
            {
                return _Gender;
            }
            set
            {
                if (value != _Gender)
                {
                    _Gender = value;
                    OnPropertyChanged("Gender");
                }
            }
        }

        /// <remarks/>
        private int _FollowersCount;
        public int FollowersCount
        {
            get
            {
                return _FollowersCount;
            }
            set
            {
                if (value != _FollowersCount)
                {
                    _FollowersCount = value;
                    OnPropertyChanged("FollowersCount");
                }
            }
        }

        /// <remarks/>
        private int _FriendsCount;
        public int FriendsCount
        {
            get
            {
                return _FriendsCount;
            }
            set
            {
                if (value != _FriendsCount)
                {
                    _FriendsCount = value;
                    OnPropertyChanged("FriendsCount");
                }
            }
        }

        /// <remarks/>
        private int _StatusesCount;
        public int StatusesCount
        {
            get
            {
                return _StatusesCount;
            }
            set
            {
                if (value != _StatusesCount)
                {
                    _StatusesCount = value;
                    OnPropertyChanged("StatusesCount");
                }
            }
        }

        /// <remarks/>
        private int _FavouritesCount;
        public int FavouritesCount
        {
            get
            {
                return _FavouritesCount;
            }
            set
            {
                if (value != _FavouritesCount)
                {
                    _FavouritesCount = value;
                    OnPropertyChanged("FavouritesCount");
                }
            }
        }

        /// <remarks/>
        private string _CreatedAt;
        public string CreatedAt
        {
            get
            {
                return _CreatedAt;
            }
            set
            {
                if (value != _CreatedAt)
                {
                    _CreatedAt = value;
                    OnPropertyChanged("CreatedAt");
                }
            }
        }

        /// <remarks/>
        private bool _GeoEnabled;
        public bool GeoEnabled
        {
            get
            {
                return _GeoEnabled;
            }
            set
            {
                if (value != _GeoEnabled)
                {
                    _GeoEnabled = value;
                    OnPropertyChanged("GeoEnabled");
                }
            }
        }

        /// <remarks/>
        private bool _AllowAllActMsg;
        public bool AllowAllActMsg
        {
            get
            {
                return _AllowAllActMsg;
            }
            set
            {
                if (value != _AllowAllActMsg)
                {
                    _AllowAllActMsg = value;
                    OnPropertyChanged("AllowAllActMsg");
                }
            }
        }

        /// <remarks/>
        private bool _Following;
        public bool Following
        {
            get
            {
                return _Following;
            }
            set
            {
                if (value != _Following)
                {
                    _Following = value;
                    OnPropertyChanged("Following");
                }
            }
        }

        /// <remarks/>
        private bool _BiFollowing;
        public bool BiFollowing
        {
            get
            {
                return _BiFollowing;
            }
            set
            {
                if (value != _BiFollowing)
                {
                    _BiFollowing = value;
                    OnPropertyChanged("BiFollowing");
                }
            }
        }

        /// <remarks/>
        private bool _Verified;
        public bool Verified
        {
            get
            {
                return _Verified;
            }
            set
            {
                if (value != _Verified)
                {
                    _Verified = value;
                    OnPropertyChanged("Verified");
                }
            }
        }

        /// <remarks/>
        private StatusData _LatestStatus;
        public StatusData LatestStatus
        {
            get
            {
                return _LatestStatus;
            }
            set
            {
                if (value != _LatestStatus)
                {
                    _LatestStatus = value;
                    OnPropertyChanged("LatestStatus");
                }
            }
        }
    }
}
