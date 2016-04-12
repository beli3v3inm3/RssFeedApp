using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.ServiceModel.Syndication;
using System.Web;
using System.Xml;
using RssFeedApp.Models;
using RssFeedApp.Repository;

namespace RssFeedApp.Provider
{
    public class RssRepository : IRssRepository, IDisposable
    {

        #region Singleton
        private static volatile RssRepository _instance;
        private static readonly object SyncRoot = new object();

        private RssRepository() { }

        public static RssRepository GetInstance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new RssRepository();
                    }
                }

                return _instance;
            }
        }
        #endregion

        public void AddFeed(Rss urlRss)
        {
            const string addFeedProc = "spAddFeed";
            const string addFeedItemProc = "spAddFeedItems";

            using (var repo = new RequestRepository())
            {
                var reader = new XmlTextReader(urlRss.Url);
                var syndicationFeed = SyndicationFeed.Load(reader);
                var user = HttpContext.Current.User.Identity.Name;

                if (syndicationFeed == null) return;

                repo.ExecuteProcedure(
                    addFeedProc,
                    new SqlParameter("@url", urlRss.Url),
                    new SqlParameter("@title", syndicationFeed.Title.Text),
                    new SqlParameter("@description", syndicationFeed.Description.Text),
                    new SqlParameter("@imageUrl", syndicationFeed.ImageUrl.AbsoluteUri),
                    new SqlParameter("@lastBuildDate", syndicationFeed.LastUpdatedTime.DateTime == DateTime.MinValue ? DateTime.Now : syndicationFeed.LastUpdatedTime.DateTime),
                    new SqlParameter("@userName", user));


                foreach (var item in syndicationFeed.Items)
                {
                    repo.ExecuteProcedure(
                        addFeedItemProc,
                        new SqlParameter("@url", urlRss.Url),
                        new SqlParameter("@title", item.Title.Text),
                        new SqlParameter("@body", item.Summary.Text),
                        new SqlParameter("@link", item.Id),
                        new SqlParameter("@pubDate", item.PublishDate.DateTime == DateTime.MinValue ? DateTime.Now : item.PublishDate.DateTime),
                        new SqlParameter("@userName", user),
                        new SqlParameter("@isRead", false));
                }
                #region old
                //if (feed == null) return;
                //connection.Connection.Open();
                //connection.SqlCommand.Connection = connection.Connection;
                //connection.SqlCommand.CommandText = "spAddFeed";
                //connection.SqlCommand.CommandType = CommandType.StoredProcedure;
                //connection.SqlCommand.Parameters.AddWithValue("@url", urlFeed.Url);
                //connection.SqlCommand.Parameters.AddWithValue("@title", feed.Title.Text);
                //connection.SqlCommand.Parameters.AddWithValue("@description", feed.Description.Text);
                //connection.SqlCommand.Parameters.AddWithValue("@imageUrl", feed.ImageUrl.AbsoluteUri);
                //connection.SqlCommand.Parameters.AddWithValue("@lastBuildDate", feed.LastUpdatedTime.DateTime);
                //connection.SqlCommand.Parameters.AddWithValue("@userName", user);
                //connection.SqlCommand.ExecuteNonQuery();
                //connection.SqlCommand.Parameters.Clear();

                //connection.SqlCommand.CommandText = "spAddFeedItems";
                //connection.SqlCommand.CommandType = CommandType.StoredProcedure;

                //foreach (var item in feed.Items)
                //{
                //    connection.SqlCommand.Parameters.AddWithValue("@url", urlFeed.Url);
                //    connection.SqlCommand.Parameters.AddWithValue("@title", item.Title.Text);
                //    connection.SqlCommand.Parameters.AddWithValue("@body", item.Summary.Text);
                //    connection.SqlCommand.Parameters.AddWithValue("@link", item.Id);
                //    if (item.PublishDate.DateTime != DateTime.MinValue)
                //    {
                //        connection.SqlCommand.Parameters.AddWithValue("@pubDate", item.PublishDate.DateTime);
                //    }
                //    connection.SqlCommand.Parameters.AddWithValue("@userName", user);
                //    connection.SqlCommand.Parameters.AddWithValue("@isRead", false);
                //    connection.SqlCommand.ExecuteNonQuery();
                //    connection.SqlCommand.Parameters.Clear();
                //}
                #endregion
            }
        }

        public IEnumerable<Feed> GetFeeds(Feed feed)
        {
            const string getFeedsProc = "spGetItemsByUser";

            using (var repo = new RequestRepository())
            {
                var user = HttpContext.Current.User.Identity.Name;

                foreach (object[] items in repo.ExecuteProcReader(
                    getFeedsProc,
                    new SqlParameter("@userName", user)))
                {
                    feed.Id = (int)items[0];
                    feed.Title = items[1].ToString();
                    feed.Body = items[2].ToString();
                    feed.Link = items[3].ToString();
                    if (items[4] is DateTime)
                    {
                        feed.PubDate = (DateTime)items[4];
                    }
                    feed.IsRead = (bool)items[5];
                    yield return feed;
                }
            }

            #region old
            //using (var connection = new DbConnection())
            //{
            //    var user = HttpContext.Current.User.Identity.Name;

            //    connection.Connection.Open();
            //    connection.SqlCommand.Connection = connection.Connection;

            //    connection.SqlCommand.CommandText = "spGetItemsByUser";
            //    connection.SqlCommand.CommandType = CommandType.StoredProcedure;
            //    connection.SqlCommand.Parameters.AddWithValue("@userName", user);
            //    var reader = connection.SqlCommand.ExecuteReader();
            //    while (reader.Read())
            //    {
            //        //rssFeed.Id = (int)reader["id"];
            //        //rssFeed.Title = reader["title"].ToString();
            //        //rssFeed.Body = reader["body"].ToString();
            //        //rssFeed.Link = reader["link"].ToString();
            //        //var pubDate = reader["pubDate"].ToString();
            //        //if (!string.IsNullOrEmpty(pubDate))
            //        //{
            //        //    rssFeed.PubDate = Convert.ToDateTime(pubDate);
            //        //}
            //        //rssFeed.IsRead = (bool)reader["isread"];
            //        yield return reader;
            //    }
            //}
            #endregion
        }

        public IEnumerable<Rss> GetRss(Rss rss)
        {
            const string getRssProc = "GetRssByUser";

            var user = HttpContext.Current.User.Identity.Name;
            using (var repo = new RequestRepository())
            {

                foreach (object[] items in repo.ExecuteProcReader(
                    getRssProc,
                    new SqlParameter("@userName", user)))
                {
                    rss.Id = (int) items[0];
                    rss.Title = items[2].ToString();
                    rss.Description = items[3].ToString();
                    rss.ImageUrl = items[4].ToString();
                    rss.PubDate = (DateTime) items[5];
                    yield return rss;
                }
            }

            #region old
            //using (var connection = new DbConnection())
            //{
            //    var user = HttpContext.Current.User.Identity.Name;

            //    connection.Connection.Open();
            //    connection.SqlCommand.Connection = connection.Connection;

            //    connection.SqlCommand.CommandText = "GetRssByUser";
            //    connection.SqlCommand.CommandType = CommandType.StoredProcedure;
            //    connection.SqlCommand.Parameters.AddWithValue("@userName", user);

            //    var reader = connection.SqlCommand.ExecuteReader();
            //    while (reader.Read())
            //    {
            //        feed.Id = (int)reader["id"];
            //        feed.Title = reader["title"].ToString();
            //        feed.Body = reader["body"].ToString(); 
            //        feed.Link = reader["link"].ToString(); 
            //        feed.Image = reader["imageUrl"].ToString();
            //        feed.PubDate = (DateTime)reader["pubDate"];
            //        feed.IsRead = (bool)reader["isread"];
            //        yield return feed;
            //    }
            //    yield return feed;
            //}
            #endregion
        }


        public void RemoveFeed(Feed feed)
        {
            const string removeFeedItem = "spRemoveRssItem";

            using (var repo = new RequestRepository())
            {
                var user = HttpContext.Current.User.Identity.Name;
                repo.ExecuteProcedure(
                    removeFeedItem,
                    new SqlParameter("@userName", user),
                    new SqlParameter("@Id", feed.Id));
            }
        }

        public void SetReadItem(Feed feed)
        {
            const string setReadItemProc = "spReadFeedItemEdit";

            using (var repo = new RequestRepository())
            {
                repo.ExecuteProcedure(
                    setReadItemProc,
                    new SqlParameter("@feeId", feed.Id),
                    new SqlParameter("@isRead", true));
            }

            #region old
            //using (var context = new DbConnection())
            //{
            //    context.Connection.Open();
            //    context.SqlCommand.Connection = context.Connection;

            //    context.SqlCommand.CommandText = "spReadFeedItemEdit";
            //    context.SqlCommand.CommandType = CommandType.StoredProcedure;

            //    context.SqlCommand.Parameters.AddWithValue("@feeId", feed.Id);
            //    context.SqlCommand.Parameters.AddWithValue("@isRead", true);
            //    context.SqlCommand.ExecuteNonQuery();
            //}
            #endregion
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}