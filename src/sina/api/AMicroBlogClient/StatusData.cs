using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AMicroblogAPI.DataContract;

namespace AMicroblogAPISample
{
    public class StatusData : PropertyChangeNotifier
    {
        /// <summary>
        /// Gets or sets the creation time of the status.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the stutus id.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the status text.
        /// </summary>
        private string _Text;
        public string Text
        {
            get
            {
                return _Text;
            }
            set
            {
                if (value != _Text)
                {
                    _Text = value;
                    OnPropertyChanged("Text");
                }
            }
        }

        /// <summary>
        /// Gets or sets the source of the status.
        /// </summary>
        private Source _Source;
        public Source Source
        {
            get
            {
                return _Source;
            }
            set
            {
                if (value != _Source)
                {
                    _Source = value;
                    OnPropertyChanged("Source");
                }
            }
        }

        /// <summary>
        /// Gets or sets a boolead indicates whether the status is favarited.
        /// </summary>
        private bool _Favorited;
        public bool Favorited
        {
            get
            {
                return _Favorited;
            }
            set
            {
                if (value != _Favorited)
                {
                    _Favorited = value;
                    OnPropertyChanged("Favorited");
                }
            }
        }

        /// <summary>
        /// Gets or sets a boolead indicates whether the status is truncated.
        /// </summary>
        private bool _Truncated;
        public bool Truncated
        {
            get
            {
                return _Truncated;
            }
            set
            {
                if (value != _Truncated)
                {
                    _Truncated = value;
                    OnPropertyChanged("Truncated");
                }
            }
        }

        /// <summary>
        /// Gets or sets the mid.
        /// </summary>
        private string _ReplyTo;
        public string ReplyTo
        {
            get
            {
                return _ReplyTo;
            }
            set
            {
                if (value != _ReplyTo)
                {
                    _ReplyTo = value;
                    OnPropertyChanged("ReplyTo");
                }
            }
        }

        /// <summary>
        /// Gets or sets the mid.
        /// </summary>
        private string _ReplyToUserId;
        public string ReplyToUserId
        {
            get
            {
                return _ReplyToUserId;
            }
            set
            {
                if (value != _ReplyToUserId)
                {
                    _ReplyToUserId = value;
                    OnPropertyChanged("ReplyToUserId");
                }
            }
        }

        /// <summary>
        /// Gets or sets the mid.
        /// </summary>
        private string _ReplyToUserScreenName;
        public string ReplyToUserScreenName
        {
            get
            {
                return _ReplyToUserScreenName;
            }
            set
            {
                if (value != _ReplyToUserScreenName)
                {
                    _ReplyToUserScreenName = value;
                    OnPropertyChanged("ReplyToUserScreenName");
                }
            }
        }

        /// <summary>
        /// Gets or sets the thumbnail_pic.
        /// </summary>
        private string _ThumbnailPic;
        public string ThumbnailPic
        {
            get
            {
                return _ThumbnailPic;
            }
            set
            {
                if (value != _ThumbnailPic)
                {
                    _ThumbnailPic = value;
                    OnPropertyChanged("ThumbnailPic");
                }
            }
        }

        /// <summary>
        /// Gets or sets the bmiddle_pic.
        /// </summary>
        private string _MiddlePic;
        public string MiddlePic
        {
            get
            {
                return _MiddlePic;
            }
            set
            {
                if (value != _MiddlePic)
                {
                    _MiddlePic = value;
                    OnPropertyChanged("MiddlePic");
                }
            }
        }

        /// <summary>
        /// Gets or sets the original_pic.
        /// </summary>
        private string _OriginalPic;
        public string OriginalPic
        {
            get
            {
                return _OriginalPic;
            }
            set
            {
                if (value != _OriginalPic)
                {
                    _OriginalPic = value;
                    OnPropertyChanged("OriginalPic");
                }
            }
        }

        /// <summary>
        /// Gets or sets the mid.
        /// </summary>
        private string _Mid;
        public string Mid
        {
            get
            {
                return _Mid;
            }
            set
            {
                if (value != _Mid)
                {
                    _Mid = value;
                    OnPropertyChanged("Mid");
                }
            }
        }

        /// <summary>
        /// Gets or sets the user who posts this status.
        /// </summary>
        private UserData _User;
        public UserData User
        {
            get
            {
                return _User;
            }
            set
            {
                if (value != _User)
                {
                    _User = value;
                    OnPropertyChanged("User");
                }
            }
        }

        /// <summary>
        /// Gets or sets the user who posts this status.
        /// </summary>
        private Geo _Geo;
        public Geo Geo
        {
            get
            {
                return _Geo;
            }
            set
            {
                if (value != _Geo)
                {
                    _Geo = value;
                    OnPropertyChanged("Geo");
                }
            }
        }

        /// <summary>
        /// Gets or sets the status that current status is reposted with.
        /// </summary>
        private StatusData _RetweetedStatus;
        public StatusData RetweetedStatus
        {
            get
            {
                return _RetweetedStatus;
            }
            set
            {
                if (value != _RetweetedStatus)
                {
                    _RetweetedStatus = value;
                    OnPropertyChanged("RetweetedStatus");
                }
            }
        }

        private int _Comments;
        public int Comments
        {
            get
            {
                return _Comments;
            }
            set
            {
                if (value != _Comments)
                {
                    _Comments = value;
                    OnPropertyChanged("Comments");
                }
            }
        }

        private int _Forwards;
        public int Forwards
        {
            get
            {
                return _Forwards;
            }
            set
            {
                if (value != _Forwards)
                {
                    _Forwards = value;
                    OnPropertyChanged("Forwards");
                }
            }
        }
    }
}
