using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AMicroblogAPI.DataContract;
using System.ComponentModel;
using System.IO;
using System.Collections.ObjectModel;
using System.Threading;
using AMicroblogAPI.Common;
using AMicroblogAPI;
using System.Windows;

namespace AMicroblogAPISample
{
    public class MainViewModel : PropertyChangeNotifier
    {
        private SynchronizationContext syncContext;
        private Timer timer;
        private long checkForUpdateInterval = 30000; // 30 seconds as initial value.

        public MainViewModel()
        {
            syncContext = SynchronizationContext.Current;
            timer = new Timer(PerformPoll, null, Timeout.Infinite, Timeout.Infinite);
        }

        private UserInfo currentUser;
        public UserInfo CurrentUser
        {
            get
            {
                return currentUser;
            }
            set
            {
                if (value != currentUser)
                {
                    currentUser = value;
                    OnPropertyChanged("CurrentUser");
                }
            }
        }

        private ObservableCollection<StatusData> myHomeStatuses = new ObservableCollection<StatusData>();
        public ObservableCollection<StatusData> MyHomeStatuses
        {
            get
            {
                return myHomeStatuses;
            }
        }

        private ObservableCollection<StatusData> myStatuses = new ObservableCollection<StatusData>();
        public ObservableCollection<StatusData> MyStatuses
        {
            get
            {
                return myStatuses;
            }
        }

        private ObservableCollection<StatusData> atMeStatuses = new ObservableCollection<StatusData>();
        public ObservableCollection<StatusData> AtMeStatuses
        {
            get
            {
                return atMeStatuses;
            }
        }

        private ObservableCollection<StatusData> favouriteStatuses = new ObservableCollection<StatusData>();
        public ObservableCollection<StatusData> FavouriteStatuses
        {
            get
            {
                return favouriteStatuses;
            }
        }

        private ObservableCollection<UserData> userSuggestions;
        public ObservableCollection<UserData> UserSuggestions
        {
            get
            {
                return userSuggestions;
            }
            set
            {
                if (value != userSuggestions)
                {
                    userSuggestions = value;
                    OnPropertyChanged("UserSuggestions");
                }
            }
        }

        private ObservableCollection<UserData> followings = new ObservableCollection<UserData>();
        public ObservableCollection<UserData> Followings
        {
            get
            {
                return followings;
            }
        }

        private ObservableCollection<UserData> followers = new ObservableCollection<UserData>();
        public ObservableCollection<UserData> Followers
        {
            get
            {
                return followers;
            }
        }

        private ObservableCollection<StatusData> statusSearchResult = new ObservableCollection<StatusData>();
        public ObservableCollection<StatusData> StatusSearchResult
        {
            get
            {
                return statusSearchResult;
            }
        }

        private ObservableCollection<UserData> peopleSearchResult = new ObservableCollection<UserData>();
        public ObservableCollection<UserData> PeopleSearchResult
        {
            get
            {
                return peopleSearchResult;
            }
        }

        private ObservableCollection<CounterInfo> counters = new ObservableCollection<CounterInfo>();
        public ObservableCollection<CounterInfo> Counters
        {
            get
            {
                return counters;
            }
        }

        private ObservableCollection<CommentInfo> commentsToMe = new ObservableCollection<CommentInfo>();
        public ObservableCollection<CommentInfo> CommentsToMe
        {
            get
            {
                return commentsToMe;
            }
        }

        private ObservableCollection<CommentInfo> commentsByMe = new ObservableCollection<CommentInfo>();
        public ObservableCollection<CommentInfo> CommentsByMe
        {
            get
            {
                return commentsByMe;
            }
        }

        public void RetrieveHomeStatuses()
        {
            // Get home statuses
            var getHomeStatusesCallback = new AsyncCallback<Statuses>(delegate(AsyncCallResult<Statuses> result)
            {
                if (result.Success)
                {
                    var statuses = result.Data;
                    if (0 < statuses.Items.Count)
                    {
                        this.syncContext.Send(new SendOrPostCallback(delegate(object state)
                        {
                            foreach (var item in statuses.Items)
                            {
                                this.MyHomeStatuses.Add(DataConverter.ConvertFrom(item));
                            }
                        }), null);
                    }

                    ShowMessage("Home statuses retrieved.");

                    RetrieveMyStatuses();
                }
                else
                    ShowMessage("Failed to retrieve home statuses: {0}.", result.Exception.Message);

            });

            AMicroblog.GetFriendsStatusesAsync(getHomeStatusesCallback, null, null, null, 30);
        }

        private void RetrieveMyStatuses()
        {
            // Get my statuses
            var callback = new AsyncCallback<Statuses>(delegate(AsyncCallResult<Statuses> result)
            {
                if (result.Success)
                {
                    var statuses = result.Data;
                    if (0 < statuses.Items.Count)
                    {
                        this.syncContext.Send(new SendOrPostCallback(delegate(object state)
                        {
                            foreach (var item in statuses.Items)
                            {
                                this.MyStatuses.Add(DataConverter.ConvertFrom(item));
                            }
                        }), null);
                    }

                    ShowMessage("My statuses retrieved.");

                    RetrieveFollowers();
                }
                else
                    ShowMessage("Failed to retrieve my statuses: {0}.", result.Exception.Message);
            });

            AMicroblog.GetUserStatusesAsync(callback);
        }

        private void RetrieveFollowers()
        {
            var getFollowersCallback = new AsyncCallback<Users>(delegate(AsyncCallResult<Users> result)
            {
                if (result.Success)
                {
                    var users = result.Data;
                    if (0 < users.Items.Count)
                    {
                        this.syncContext.Send(new SendOrPostCallback(delegate(object state)
                        {
                            foreach (var user in users.Items)
                                this.Followers.Add(DataConverter.ConvertFrom(user));
                        }), null);

                    }

                    ShowMessage("Followers retrieved.");

                    RetrieveFriends();
                }
                else
                    ShowMessage("Failed to retrieve followers: {0}.", result.Exception.Message);
            });

            AMicroblog.GetFollowersAsync(getFollowersCallback);
        }

        private void RetrieveFriends()
        {
            // Get friends
            var getFriendsCallback = new AsyncCallback<Users>(delegate(AsyncCallResult<Users> result)
            {
                if (result.Success)
                {
                    var users = result.Data;
                    if (0 < users.Items.Count)
                    {
                        this.syncContext.Send(new SendOrPostCallback(delegate(object state)
                        {
                            foreach (var user in users.Items)
                                this.Followings.Add(DataConverter.ConvertFrom(user));
                        }), null);
                    }

                    foreach (var friend in this.Followings)
                    {
                        var exists = this.Followers.FirstOrDefault((item) => item.ID == friend.ID);
                        if (null != exists)
                        {
                            exists.Following = true;
                            exists.BiFollowing = true;
                            friend.BiFollowing = true;
                        }
                        friend.Following = true;
                    }

                    ShowMessage("Friends retrieved.");

                    RetrieveAtMeStatuses();
                }
                else
                    ShowMessage("Failed to retrieve friends: {0}.", result.Exception.Message);
            });

            AMicroblog.GetFriendsAsync(getFriendsCallback);
        }

        private void RetrieveAtMeStatuses()
        {
            var callback = new AsyncCallback<Statuses>(delegate(AsyncCallResult<Statuses> result)
            {
                if (result.Success)
                {
                    var statuses = result.Data;
                    if (0 < statuses.Items.Count)
                    {
                        this.syncContext.Send(new SendOrPostCallback(delegate(object state)
                        {
                            foreach (var item in statuses.Items)
                            {
                                this.AtMeStatuses.Add(DataConverter.ConvertFrom(item));
                            }
                        }), null);
                    }

                    ShowMessage("At-me statuses retrieved.");

                    RetrieveFavourites();
                }
                else
                    ShowMessage("Failed to retrieve at-me statuses: {0}.", result.Exception.Message);
            });

            AMicroblog.GetMentionsAsync(callback, null, null, null, 20);
        }

        private void RetrieveFavourites()
        {
            var callback = new AsyncCallback<Statuses>(delegate(AsyncCallResult<Statuses> result)
            {
                if (result.Success)
                {
                    var statuses = result.Data;
                    if (0 < statuses.Items.Count)
                    {
                        this.syncContext.Send(new SendOrPostCallback(delegate(object state)
                        {
                            foreach (var item in statuses.Items)
                            {
                                var data = DataConverter.ConvertFrom(item);
                                data.Favorited = true;
                                this.FavouriteStatuses.Add(data);
                            }
                        }), null);
                    }

                    ShowMessage("Favourite statuses retrieved.");

                    RetrieveCounters();
                }
                else
                    ShowMessage("Failed to retrieve favourite statuses: {0}.", result.Exception.Message);
            });

            AMicroblog.GetFavoritesAsync(callback);
        }

        private void RetrieveCounters()
        {
            var statusIDs = new Collection<long>();
            foreach (var stat in this.MyHomeStatuses)
            {
                statusIDs.Add(stat.ID);
            }

            var callback = new AsyncCallback<Counters>(delegate(AsyncCallResult<Counters> result)
            {
                if (result.Success)
                {
                    var items = result.Data;
                    if (0 < items.Items.Count)
                    {
                        this.syncContext.Send(new SendOrPostCallback(delegate(object state)
                        {
                            foreach (var counterInfo in items.Items)
                            {
                                this.Counters.Add(counterInfo);

                                var targetID = counterInfo.StatusID;
                                // Traverse all statuses collection to set Favorited as true.
                                var exists = this.MyHomeStatuses.FirstOrDefault((item) => item.ID == targetID);
                                if (null != exists)
                                {
                                    exists.Comments = counterInfo.Comments;
                                    exists.Forwards = counterInfo.Forwards;
                                }
                                exists = this.MyStatuses.FirstOrDefault((item) => item.ID == targetID);
                                if (null != exists)
                                {
                                    exists.Comments = counterInfo.Comments;
                                    exists.Forwards = counterInfo.Forwards;
                                }
                                exists = this.AtMeStatuses.FirstOrDefault((item) => item.ID == targetID);
                                if (null != exists)
                                {
                                    exists.Comments = counterInfo.Comments;
                                    exists.Forwards = counterInfo.Forwards;
                                }
                                exists = this.StatusSearchResult.FirstOrDefault((item) => item.ID == targetID);
                                if (null != exists)
                                {
                                    exists.Comments = counterInfo.Comments;
                                    exists.Forwards = counterInfo.Forwards;
                                }
                            }
                        }), null);
                    }

                    RetrieveCommentsToMe();
                }
                else
                    ShowMessage("Failed to retrieve statuses counters due to: {0}.", result.Exception.Message);
            });

            // TODO: 20 each request
            if(0 < statusIDs.Count)
                AMicroblog.GetCountersOfCommentNForwardAsync(callback, statusIDs.ToArray());
        }

        private void RetrieveCommentsToMe()
        {
            var callback = new AsyncCallback<Comments>(delegate(AsyncCallResult<Comments> result)
            {
                if (result.Success)
                {
                    var comments = result.Data;
                    if (0 < comments.Items.Count)
                    {
                        this.syncContext.Send(new SendOrPostCallback(delegate(object state)
                        {
                            foreach (var item in comments.Items)
                            {
                                this.CommentsToMe.Add(item);
                            }
                        }), null);
                    }

                    ShowMessage("Comments-to-me retrieved.");

                    RetrieveCommentsByMe();
                }
                else
                    ShowMessage("Failed to retrieve comments-to-me: {0}.", result.Exception.Message);
            });

            AMicroblog.GetCommentsToMeAsync(callback);
        }

        private void RetrieveCommentsByMe()
        {
            var callback = new AsyncCallback<Comments>(delegate(AsyncCallResult<Comments> result)
            {
                if (result.Success)
                {
                    var comments = result.Data;
                    if (0 < comments.Items.Count)
                    {
                        this.syncContext.Send(new SendOrPostCallback(delegate(object state)
                        {
                            foreach (var item in comments.Items)
                            {
                                this.CommentsByMe.Add(item);
                            }
                        }), null);
                    }

                    ShowMessage("Comments-by-me retrieved.");

                    ShowMessage("All initial data retrieved.");

                    StartBackgroundPoller();
                }
                else
                    ShowMessage("Failed to retrieve comments-by-me due to: {0}.", result.Exception.Message);
            });

            AMicroblog.GetCommentsByMeAsync(callback);
        }

        public void AddToFavourite(StatusData statusData, FrameworkElement target)
        {
            var callback = new AsyncCallback<StatusInfo>(delegate(AsyncCallResult<StatusInfo> result)
            {
                if (result.Success)
                {
                    var statusInFav = result.Data;

                    this.syncContext.Send(new SendOrPostCallback(delegate(object state)
                    {
                        var srvStatusData = DataConverter.ConvertFrom(statusInFav);
                        srvStatusData.Favorited = true;
                        var exists = this.FavouriteStatuses.FirstOrDefault((item) => item.ID == srvStatusData.ID);
                        if (null == exists)
                            this.FavouriteStatuses.Insert(0, srvStatusData); // TODO: Sorting

                        // Traverse all statuses collection to set Favorited as true.
                        exists = this.MyHomeStatuses.FirstOrDefault((item) => item.ID == srvStatusData.ID);
                        if (null != exists)
                            exists.Favorited = true;
                        exists = this.MyStatuses.FirstOrDefault((item) => item.ID == srvStatusData.ID);
                        if (null != exists)
                            exists.Favorited = true;
                        exists = this.AtMeStatuses.FirstOrDefault((item) => item.ID == srvStatusData.ID);
                        if (null != exists)
                            exists.Favorited = true;
                        exists = this.StatusSearchResult.FirstOrDefault((item) => item.ID == srvStatusData.ID);
                        if (null != exists)
                            exists.Favorited = true;

                    }), null);

                    ShowMessage("Status added to favourite.");
                }
                else
                    ShowMessage("Status failed to add to favourite due to: {0}.", result.Exception.Message);

                SetItemState(target, true);
            });

            target.IsEnabled = false;
            ShowMessage("Adding status to favourite...");
            AMicroblog.AddToFavoriteAsync(callback, statusData.ID);
        }

        public void RemoveFromFavourite(StatusData statusData, FrameworkElement target)
        {
            var callback = new AsyncCallback<StatusInfo>(delegate(AsyncCallResult<StatusInfo> result)
            {
                if (result.Success)
                {
                    var statusInFav = result.Data;

                    this.syncContext.Send(new SendOrPostCallback(delegate(object state)
                    {
                        var exists = this.FavouriteStatuses.FirstOrDefault((item) => item.ID == statusInFav.ID);
                        if (null != exists)
                            this.FavouriteStatuses.Remove(exists);

                        // Traverse all statuses collection to set Favorited as false.
                        exists = this.MyHomeStatuses.FirstOrDefault((item) => item.ID == statusInFav.ID);
                        if (null != exists)
                            exists.Favorited = false;
                        exists = this.MyStatuses.FirstOrDefault((item) => item.ID == statusInFav.ID);
                        if (null != exists)
                            exists.Favorited = false;
                        exists = this.AtMeStatuses.FirstOrDefault((item) => item.ID == statusInFav.ID);
                        if (null != exists)
                            exists.Favorited = false;
                        exists = this.StatusSearchResult.FirstOrDefault((item) => item.ID == statusInFav.ID);
                        if (null != exists)
                            exists.Favorited = false;
                    }), null);

                    ShowMessage("Status removed to favourite.");
                }
                else
                    ShowMessage("Status failed to remov from favourite due to: {0}.", result.Exception.Message);

                SetItemState(target, true);
            });

            target.IsEnabled = false;
            ShowMessage("Removing status from favourite.");
            AMicroblog.DeleteFromFavoriteAsync(callback, statusData.ID);
        }

        public void DoSearch(string keyword)
        {
            this.StatusSearchResult.Clear();
            this.PeopleSearchResult.Clear();
            ShowMessage("Searching people/statuses by '{0}'", keyword);

            // Status search
            var statusCallback = new AsyncCallback<Statuses>(delegate(AsyncCallResult<Statuses> result)
            {
                if (result.Success)
                {
                    var statuses = result.Data;
                    if (0 < statuses.Items.Count)
                    {
                        this.syncContext.Send(new SendOrPostCallback(delegate(object state)
                        {
                            foreach (var item in statuses.Items)
                                this.StatusSearchResult.Add(DataConverter.ConvertFrom(item));
                        }), null);
                    }

                    ShowMessage("{0} statuses found.", statuses.Items.Count);
                }
                else
                    ShowMessage("Searching statuses failed due to: {0}", result.Exception.Message);
            });

            // People search
            var peopleCallback = new AsyncCallback<Users>(delegate(AsyncCallResult<Users> result)
            {
                if (result.Success)
                {
                    var users = result.Data;
                    if (0 < users.Items.Count)
                    {
                        this.syncContext.Send(new SendOrPostCallback(delegate(object state)
                        {
                            foreach (var user in users.Items)
                                this.PeopleSearchResult.Add(DataConverter.ConvertFrom(user));
                        }), null);
                    }

                    ShowMessage("{0} people found.", users.Items.Count);

                    AMicroblog.SearchStatusesAsync(statusCallback, keyword);
                }
                else
                    ShowMessage("Searching people failed due to: {0}", result.Exception.Message);
            });

            AMicroblog.SearchUsersAsync(peopleCallback, keyword);
        }

        #region Notification Message

        private string notificationMessage;
        public string NotificationMessage
        {
            get
            {
                return notificationMessage;
            }
            set
            {
                if (value != notificationMessage)
                {
                    notificationMessage = value;
                    OnPropertyChanged("NotificationMessage");
                }
            }
        }

        private bool isSuccessMsg;
        public bool IsSuccessMsg
        {
            get
            {
                return isSuccessMsg;
            }
            set
            {
                if (value != isSuccessMsg)
                {
                    isSuccessMsg = value;
                    OnPropertyChanged("IsSuccessMsg");
                }
            }
        }

        public void ShowMessage(string msg, params object[] parameters)
        {
            ShowMessage(msg, true, parameters);
        }

        public void ShowMessage(string msg, bool isSuccess, params object[] parameters)
        {
            NotificationMessage = string.Format(msg, parameters);
            IsSuccessMsg = isSuccess;
        }

        #endregion

        #region Background Updater

        public void StartBackgroundPoller()
        {
            timer.Change(checkForUpdateInterval, checkForUpdateInterval);
        }

        private bool isPolling;
        public bool IsPolling
        {
            get
            {
                return isPolling;
            }
            set
            {
                if (value != isPolling)
                {
                    isPolling = value;
                    OnPropertyChanged("IsPolling");
                }
            }
        }

        private void PerformPoll(object state)
        {
            try
            {
                IsPolling = true;

                // Get home statuses
                var getHomeStatusesCallback = new AsyncCallback<Statuses>(delegate(AsyncCallResult<Statuses> result)
                {
                    if (result.Success)
                    {
                        var statuses = result.Data;
                        if (null == this.syncContext)
                        {
                            return;
                        }

                        if (0 < statuses.Items.Count)
                        {
                            this.syncContext.Send(new SendOrPostCallback(delegate(object stateObj)
                            {
                                foreach (var item in statuses.Items)
                                {
                                    this.MyHomeStatuses.Insert(0, DataConverter.ConvertFrom(item));
                                }
                            }), null);

                            if (checkForUpdateInterval > 20000)
                            {
                                checkForUpdateInterval -= 10000;

                                timer.Change(checkForUpdateInterval, checkForUpdateInterval);
                            }
                        }
                        else
                        {
                            if (checkForUpdateInterval < 120000)
                            {
                                checkForUpdateInterval += 10000;

                                timer.Change(checkForUpdateInterval, checkForUpdateInterval);
                            }
                        }
                    }
                    else
                        ShowMessage("Failed to check updates for home statuses.");
                });

                long? sinceID = null;
                if (0 < this.MyHomeStatuses.Count)
                    sinceID = this.MyHomeStatuses[0].ID;

                AMicroblog.GetFriendsStatusesAsync(getHomeStatusesCallback, sinceID, null, null, 50);
            }
            finally
            {
                IsPolling = false;
            }
        }

        #endregion 
        
        public void RemoveStatusFromView(StatusInfo status)
        {
            this.syncContext.Send(new SendOrPostCallback(delegate(object state)
            {
                var found = this.MyStatuses.FirstOrDefault((d) => d.ID == status.ID);
                if (null != found)
                    this.MyStatuses.Remove(found);

                found = this.MyHomeStatuses.FirstOrDefault((d) => d.ID == status.ID);
                if (null != found)
                    this.MyHomeStatuses.Remove(found);


            }), null);
        }

        public void AddStatusToView(StatusInfo status)
        {
            this.syncContext.Send(new SendOrPostCallback(delegate(object state)
            {
                this.MyHomeStatuses.Insert(0, DataConverter.ConvertFrom(status));
                this.MyStatuses.Insert(0, DataConverter.ConvertFrom(status));

            }), null);
        }

        private void SetItemState(FrameworkElement target, bool val)
        {
            this.syncContext.Send(new SendOrPostCallback(delegate(object state)
            {
                target.IsEnabled = val;
            }), null);
        }
    }
}
