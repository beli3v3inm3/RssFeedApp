using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.ServiceModel.Syndication;
using System.Web;
using System.Xml;
using RssFeedApp.Models;
using RssFeedApp.Repository;

namespace RssFeedApp.Provider
{
    public class RssRepository : IRssRepository
    {
        #region Singleton
        //private static volatile RssRepository _instance;
        //private static readonly object SyncRoot = new object();

        //private RssRepository() { }

        //public static RssRepository GetInstance
        //{
        //    get
        //    {
        //        if (_instance == null)
        //        {
        //            lock (SyncRoot)
        //            {
        //                if (_instance == null)
        //                    _instance = new RssRepository();
        //            }
        //        }

        //        return _instance;
        //    }
        //}
        #endregion

        public void AddFeed(UrlFeed urlFeed)
        {
            const string addFeedProc = "spAddFeed";
            const string addFeedItemProc = "spAddFeedItems";

            using (var repo = new RequestRepository())
            {
                var reader = new XmlTextReader(urlFeed.Url);
                var syndicationFeed = SyndicationFeed.Load(reader);
                var user = HttpContext.Current.User.Identity.Name;

                if (syndicationFeed == null) return;

                repo.ExecuteProcedure(
                    addFeedProc,
                    new SqlParameter("@url", urlFeed.Url),
                    new SqlParameter("@title", syndicationFeed.Title.Text),
                    new SqlParameter("@description", syndicationFeed.Description.Text),
                    new SqlParameter("@imageUrl", syndicationFeed.ImageUrl.AbsoluteUri),
                    new SqlParameter("@lastBuildDate", syndicationFeed.LastUpdatedTime.DateTime == DateTime.MinValue ? DateTime.Now : syndicationFeed.LastUpdatedTime.DateTime),
                    new SqlParameter("@userName", user));


                foreach (var item in syndicationFeed.Items)
                {
                    repo.ExecuteProcedure(
                        addFeedItemProc,
                        new SqlParameter("@url", urlFeed.Url),
                        new SqlParameter("@title", item.Title.Text),
                        new SqlParameter("@body", item.Summary.Text),
                        new SqlParameter("@link", item.Id),
                        new SqlParameter("@pubDate", item.PublishDate.DateTime == DateTime.MinValue ? DateTime.Now : item.PublishDate.DateTime),
                        new SqlParameter("@userName", user),
                        new SqlParameter("@isRead", false));
                }

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
            }
        }

        public IEnumerable<RssFeed> GetRssFeed(RssFeed rssFeed)
        {
            using (var connection = new DbConnection())
            {
                var user = HttpContext.Current.User.Identity.Name;

                connection.Connection.Open();
                connection.SqlCommand.Connection = connection.Connection;

                connection.SqlCommand.CommandText = "spGetItemsByUser";
                connection.SqlCommand.CommandType = CommandType.StoredProcedure;
                connection.SqlCommand.Parameters.AddWithValue("@userName", user);
                var reader = connection.SqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    rssFeed.Id = (int)reader["id"];
                    rssFeed.Title = reader["title"].ToString();
                    rssFeed.Body = reader["body"].ToString();
                    rssFeed.Link = reader["link"].ToString();
                    var pubDate = reader["pubDate"].ToString();
                    if (!string.IsNullOrEmpty(pubDate))
                    {
                        rssFeed.PubDate = Convert.ToDateTime(pubDate);
                    }
                    rssFeed.IsRead = (bool)reader["isread"];
                    yield return rssFeed;
                }
            }
        }

        public IEnumerable<RssFeed> GetRss(RssFeed rssFeed)
        {
            using (var connection = new DbConnection())
            {
                var user = HttpContext.Current.User.Identity.Name;

                connection.Connection.Open();
                connection.SqlCommand.Connection = connection.Connection;

                connection.SqlCommand.CommandText = "GetRssByUser";
                connection.SqlCommand.CommandType = CommandType.StoredProcedure;
                connection.SqlCommand.Parameters.AddWithValue("@userName", user);

                var reader = connection.SqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    rssFeed.Id = reader.GetInt32(0);
                    rssFeed.FeedTitle = reader.GetString(1);
                    rssFeed.FeedDescription = reader.GetString(2);
                    rssFeed.Image = reader.GetString(3);
                    rssFeed.FeedLastBuildDate = reader.GetDateTime(4);
                    yield return rssFeed;
                }
                yield return rssFeed;
            }
        }

        //fix
        public void RemoveFeed(RssFeed rssFeed)
        {
            using (var connection = new DbConnection())
            {
                connection.Connection.Open();
                connection.SqlCommand.Connection = connection.Connection;

                connection.SqlCommand.CommandText = "delete from feed where id = @id";
                connection.SqlCommand.Parameters.AddWithValue("id", rssFeed.Id);
                connection.SqlCommand.ExecuteNonQuery();
            }
        }

        public void SetReadItem(RssFeed rssFeed)
        {
            using (var context = new DbConnection())
            {
                context.Connection.Open();
                context.SqlCommand.Connection = context.Connection;

                context.SqlCommand.CommandText = "spReadFeedItemEdit";
                context.SqlCommand.CommandType = CommandType.StoredProcedure;

                context.SqlCommand.Parameters.AddWithValue("@feeId", rssFeed.Id);
                context.SqlCommand.Parameters.AddWithValue("@isRead", true);
                context.SqlCommand.ExecuteNonQuery();
            }
        }
    }
}