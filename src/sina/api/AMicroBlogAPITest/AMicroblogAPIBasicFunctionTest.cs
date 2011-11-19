using System;
using System.Threading;
using AMicroblogAPI;
using AMicroblogAPI.Common;
using AMicroblogAPI.DataContract;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AMicroblogAPITest
{
    [TestClass]
    public class AMicroblogAPIBasicFunctionTest
    {
        private const long currentTestUserID = 2320057781;
        private const long AldrickUserID = 1135616617;

        [TestInitialize]
        public void Init()
        {
            AMicroblog.Login("AMicroblogTest@sina.com", "amicroblogtest");

            AMicroblogAPI.Environment.ResponseError += new EventHandler<ResponseErrorEventArgs>(Environment_ResponseError);
        }

/*
// Sample code of calling a AMicroblog API
public void Post(string status)
{
    try
    {
        var updStatusInfo = new UpdateStatusInfo();
        updStatusInfo.Status = status;

        var statusInfo = AMicroblog.PostStatus(updStatusInfo);

        // Further post-process against statusInfo...
    }
    catch (AMicroblogException aex)
    {
        // Handles the exception only if the exception is not handled by Unified Response Error Handling mechanism.
        if (!aex.IsHandled)
        {
            if (aex.IsRemoteError)
            {
                ShowErrorMessage("Server returned an error: {0}", aex.Message);

                // You can customize the error message based on aex.RemoteErrorCode.
            }
            else
                ShowErrorMessage("Unexpected error occured: {0}", aex.Message);
        }
    }
    catch (Exception ex)
    {
        ShowErrorMessage("Unexpected error occured: {0}", ex.Message);                
    }
}
*/

        private void Environment_ResponseError(object sender, ResponseErrorEventArgs e)
        {
            Console.WriteLine("An error response for {0}: {1}", e.ErrorData.RequestUri, e.ErrorData.Exception.Message);
        }

        [TestMethod]
        public void TestVerifyCredential()
        {
            var user = AMicroblog.VerifyCredential();

            Assert.IsNotNull(user);
            Assert.AreEqual(currentTestUserID, user.ID);            
        }

        [TestMethod]
        public void TestGetUserSearchSuggestions()
        {
            var suggestions = AMicroblog.GetUserSearchSuggestions("sina");

            Assert.IsNotNull(suggestions);            
        }

        [TestMethod]
        public void TestSearchStatuses()
        {
            var result = AMicroblog.SearchStatuses("AMicroblogAPI", null, null, null, null, null, null, null, null, null, true);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestStatusLifecycle()
        {
            var rnd = new Random();
            var statusInfo = new UpdateStatusInfo();
            statusInfo.Status = "#AMicroblogAPI# This is a test " + rnd.Next().ToString();
            statusInfo.Latitude = 125.12f;
            statusInfo.Longitude = 125.12f;
            var status = AMicroblog.PostStatus(statusInfo);
            Assert.IsNotNull(status);
            Assert.AreEqual(statusInfo.Status, status.Text);

            // Get status
            var retrievedStatus = AMicroblog.GetStatus(status.ID);
            Assert.AreEqual(statusInfo.Status, retrievedStatus.Text);

            // Repost as comment
            var repostStatusText = "#AMicroblogAPI# This is a repost " + rnd.Next().ToString();
            var repostedStatus = AMicroblog.Forward(retrievedStatus.ID, repostStatusText, true);
            Assert.IsNotNull(repostedStatus);
            Assert.AreEqual(repostStatusText, repostedStatus.Text);

            // Comment
            var commentText = "This is a comment " + rnd.Next().ToString();
            var comment = AMicroblog.Comment(retrievedStatus.ID, commentText);
            Assert.IsNotNull(comment);
            Assert.AreEqual(commentText, comment.Text);

            // Reply comment
            var replyCommentText = "Reply comment " + rnd.Next().ToString();
            var replyComment = AMicroblog.ReplyComment(comment.ID, replyCommentText, retrievedStatus.ID);
            Assert.IsNotNull(replyComment);
            Assert.IsTrue(replyComment.Text.Contains(replyCommentText));

            var commentsCounter = AMicroblog.GetCountersOfCommentNForward(new long[] { status.ID, repostedStatus.ID });
            Assert.IsNotNull(replyComment);
            Assert.AreEqual(2, commentsCounter.Items.Count);

            // Delete comment
            var destroiedComment = AMicroblog.DeleteComment(comment.ID);
            Assert.AreEqual(comment.ID, destroiedComment.ID);

            // Delete another comment
            var destroiedReplyComments = AMicroblog.DeleteComments(new long[] { replyComment.ID });
            Assert.AreEqual(1, destroiedReplyComments.Items.Count);

            AMicroblog.DeleteStatus(status.ID);
            AMicroblog.DeleteStatus(repostedStatus.ID);
        }

        [TestMethod]
        [ExpectedException(typeof(AMicroblogException))]
        public void TestDirectMessageLifecycle()
        {
            // No permission to call DM APIs.
            var dMsgs = AMicroblog.GetDirectMessages();
            Assert.IsNotNull(dMsgs);
        }

        [TestMethod]
        public void TestResetCounter()
        {
           AMicroblog.ResetCounter(CounterType.Comment);
        }

        [TestMethod]
        public void TestGetEmotions()
        {
            var emotions = AMicroblog.GetEmotions(EmotionType.Cartoon, "twname");

            Assert.IsNotNull(emotions);
        }

        [TestMethod]
        public void TestGetCommentsTimeline()
        {
            var result = AMicroblog.GetCommentsTimeline();

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestGetCommentsByMe()
        {
            var result = AMicroblog.GetCommentsByMe();

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestGetCommentsToMe()
        {
            var result = AMicroblog.GetCommentsToMe();

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestGetUnreadInfo()
        {
            var result = AMicroblog.GetUnreadInfo();

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestGetUserInfo()
        {
            var user = AMicroblog.GetUserInfo(1135616617);

            Assert.IsNotNull(user);
            Assert.AreEqual(1135616617, user.ID);
        }

        [TestMethod]
        public void TestGetUserInfoByName()
        {
            var user = AMicroblog.GetUserInfo("Aldrick");

            Assert.IsNotNull(user);
            Assert.AreEqual(1135616617, user.ID);
        }

        [TestMethod]
        public void TestGetUserStatuses()
        {
            var result = AMicroblog.GetUserStatuses(currentTestUserID);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestGetPublicStatuses()
        {
            var result = AMicroblog.GetPublicStatuses(10);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestGetFriendsStatuses()
        {
            var result = AMicroblog.GetFriendsStatuses();

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestGetMentions()
        {
            var result = AMicroblog.GetMentions();

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestGetFriends()
        {
            var users = AMicroblog.GetFriends(currentTestUserID);

            Assert.IsNotNull(users);
        }

        [TestMethod]
        public void TestGetFollowers()
        {
            var users = AMicroblog.GetFollowers("苗拉");

            Assert.IsNotNull(users);
        }

        [TestMethod]
        public void TestGetRateLimitStatus()
        {
            var result = AMicroblog.GetRateLimitStatus();

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestGetHotUsers()
        {
            var users = AMicroblog.GetHotUsers();

            Assert.IsNotNull(users);
        }

        [TestMethod]
        public void TestGetSuggestedUsers()
        {
            var users = AMicroblog.GetSuggestedUsers();

            Assert.IsNotNull(users);
        }

        [TestMethod]
        public void TestFollowUser()
        {
            try
            {
                AMicroblog.Unfollow(null, "Aldrick");
            }
            catch { }

            var result = AMicroblog.Follow("Aldrick");
            Assert.IsNotNull(result);

            var friendship = AMicroblog.GetFriendshipInfo(result.ID);
            Assert.IsNotNull(friendship);
            Assert.IsTrue(friendship.Source.Following);

            var exists = AMicroblog.ExistsFriendship(currentTestUserID, result.ID);
            Assert.IsTrue(exists);

            AMicroblog.Unfollow(null, "Aldrick");

            AMicroblog.Follow(result.ID);

            result = AMicroblog.Unfollow(result.ID);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestGetFollowingOrFollowerUserIDs()
        {
            var result = AMicroblog.GetFollowingUserIDs();
            Assert.IsNotNull(result);

            result = AMicroblog.GetFollowerUserIDs();

            Assert.IsNotNull(result);
        }

        [TestMethod]        
        public void TestGetPrivacy()
        {
            var result = AMicroblog.GetPrivacy();

            Assert.IsNotNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(AMicroblogException))]
        public void TestUpdatePrivacy()
        {
            var privacy = AMicroblog.GetPrivacy();

            AMicroblog.UpdatePrivacy(0, 0);
        }

        [TestMethod]
        public void TestGetUserTags()
        {
            var result = AMicroblog.GetTags(AldrickUserID);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestGetSuggestedTags()
        {
            var result = AMicroblog.GetSuggestedTags(12, 2);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestTagsLifecycle()
        {
            var result = AMicroblog.CreateTags("Microblog", "WeiboAPI", "SINAAPI", "AMicroblogAPI");

            Assert.IsNotNull(result);

            var result2 = AMicroblog.DeleteTags(result.Items[0], result.Items[1]);
            Assert.IsNotNull(result2);
            Assert.AreEqual(2, result2.Items.Count);

            AMicroblog.DeleteTag(result.Items[2]);
        }

        [TestMethod]
        [ExpectedException(typeof(AMicroblogException))]
        public void TestUpdateProfile()
        {
            var info = new UpdateProfileInfo();
            info.ScreenName = "A-Microblog-API-Test";
            info.Province = 31;
            info.City = 7;
            info.Gender = "f";
            info.Description = "I am a test machine.";

            // No permssion to call this API.
            var result = AMicroblog.UpdateProfile(info);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestFavoritesLifecycle()
        {
            var rnd = new Random();
            var text = "#AMicroblogAPI# Test add to favorite 测试";
            var status = AMicroblog.PostStatus(new UpdateStatusInfo() { Status = text + rnd.Next().ToString() });
            var status2 = AMicroblog.PostStatus(new UpdateStatusInfo() { Status = text + rnd.Next().ToString() });
            Assert.IsNotNull(status);

            var result = AMicroblog.GetFavorites();
            Assert.IsNotNull(result);
            var favCount = result.Items.Count;

            var inFavoriteStatus = AMicroblog.AddToFavorite(status.ID);
            Assert.AreEqual(status.Text, inFavoriteStatus.Text);

            // Seems server uses an async way to perform the AddToFavorite operation.
            // So wait for a half second here.
            Thread.Sleep(1000);

            result = AMicroblog.GetFavorites();
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Items.Count == favCount + 1);

            var deletedFromFav = AMicroblog.DeleteFromFavorite(status.ID);
            Assert.AreEqual(status.Text, deletedFromFav.Text);

            AMicroblog.AddToFavorite(status2.ID);          
            result = AMicroblog.DeleteMultipleFromFavorite(status2.ID);
            Assert.IsNotNull(result);

            var deletedStatus = AMicroblog.DeleteStatus(status.ID);
            Assert.AreEqual(status.Text, deletedStatus.Text);

            var deletedStatus2 = AMicroblog.DeleteStatus(status2.ID);
            Assert.AreEqual(status2.Text, deletedStatus2.Text);
        }

        [TestMethod]
        public void TestBlockLifecycle()
        {
            var result = AMicroblog.Block(AldrickUserID);
            Assert.IsNotNull(result);

            var isBlocked = AMicroblog.IsBlocked(AldrickUserID);
            Assert.IsTrue(isBlocked);

            var blockList = AMicroblog.GetBlockingList();
            Assert.IsNotNull(blockList);
            Assert.IsTrue(blockList.Items.Count > 0);
            var blockCount = blockList.Items.Count;

            var userInfo = AMicroblog.Unblock(AldrickUserID);
            Assert.IsNotNull(userInfo);

            var blockListIDs = AMicroblog.GetBlockingListIDs();
            Assert.IsNotNull(blockListIDs);
            Assert.IsTrue(blockListIDs.IDs.Count < blockCount);
        }

        [TestMethod]
        public void TestTrendLifecycle()
        {
            var rnd = new Random();

            var hoourTrends = AMicroblog.GetHourTrends();
            Assert.IsNotNull(hoourTrends);

            var dayTrends = AMicroblog.GetDayTrends();
            Assert.IsNotNull(dayTrends);

            var weekTrends = AMicroblog.GetWeekTrends();
            Assert.IsNotNull(weekTrends);

            var trendStatuses = AMicroblog.GetTrendStatuses("AMicroblogAPI");
            Assert.IsNotNull(trendStatuses);

            var userTrends = AMicroblog.GetUserTrends(currentTestUserID);
            Assert.IsNotNull(userTrends);

            var followedTrendID = AMicroblog.FollowTrend("AMicroblogAPI" + rnd.Next().ToString());

            var unfollowed = AMicroblog.UnfollowTrend(followedTrendID);
            Assert.IsTrue(unfollowed);
        }

        [TestMethod]
        public void TestShortUrlLifecycle()
        {
            var weibo = "http://open.weibo.com";
            var result = AMicroblog.ConvertToShortUrls(weibo, "http://open.weibo.com/wiki/Short_url/shorten", "http://open.weibo.com/wiki/API%E6%96%87%E6%A1%A3");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Items.Count == 3);

            var weiboShort = result.Items[0].ShortUrl;
            result = AMicroblog.ConvertToLongUrls(weiboShort);
            Assert.IsTrue(result.Items[0].LongUrl == weibo);

            var sharedCount = AMicroblog.GetShortUrlSharedCount(weiboShort);
            Assert.IsNotNull(sharedCount);
            Assert.IsTrue(sharedCount.Items.Count == 1);

            var sharedStatues = AMicroblog.GetShortUrlSharedStatuses(weiboShort);
            Assert.IsNotNull(sharedStatues);

            var commentCount = AMicroblog.GetShortUrlCommentCount(weiboShort);
            Assert.IsNotNull(commentCount);

            var sharedComments = AMicroblog.GetShortUrlComments(weiboShort);
            Assert.IsNotNull(sharedComments);
        }
    }

    public class FakeErrorHandler : IResponseErrorHandler
    {
        public void Handle(ResponseErrorData errorData)
        {
            throw new NotImplementedException();
        }
    }
}
