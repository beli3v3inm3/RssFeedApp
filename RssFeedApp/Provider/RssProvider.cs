using System;
using System.Collections.Generic;
using System.Data;
using System.ServiceModel.Syndication;
using System.Web;
using System.Xml;
using RssFeedApp.Models;

namespace RssFeedApp.Provider
{
    public class RssProvider
    {
        private static volatile RssProvider _instance;
        private readonly UserProvider _userProvider = UserProvider.GetInstance;
        private static readonly object SyncRoot = new object();

        private RssProvider() { }

        public static RssProvider GetInstance
        {
            get
            {
                if (_instance != null) return _instance;
                lock (SyncRoot)
                {
                    if (_instance == null)
                        _instance = new RssProvider();
                }

                return _instance;
            }
        }

        public void AddFeed(UrlFeed urlFeed)
        {
            using (var connection = new DbConnection())
            {
                var reader = new XmlTextReader(urlFeed.Url);
                var feed = SyndicationFeed.Load(reader);
                var user = HttpContext.Current.User.Identity.Name;

                connection.Connection.Open();
                connection.SqlCommand.Connection = connection.Connection;

                connection.SqlCommand.CommandText = "spAddItemByUrl";
                connection.SqlCommand.CommandType = CommandType.StoredProcedure;
                if (feed == null) return;
                foreach (var item in feed.Items)
                {
                    connection.SqlCommand.Parameters.AddWithValue("@url", urlFeed.Url);
                    connection.SqlCommand.Parameters.AddWithValue("@title", item.Title.Text);
                    connection.SqlCommand.Parameters.AddWithValue("@body", item.Summary.Text);
                    connection.SqlCommand.Parameters.AddWithValue("@link", item.Id);
                    connection.SqlCommand.Parameters.AddWithValue("@userName", user);
                    connection.SqlCommand.Parameters.AddWithValue("@isRead", false);
                    connection.SqlCommand.ExecuteNonQuery();
                    connection.SqlCommand.Parameters.Clear();
                }
            }
        }

        public IEnumerable<RssFeed> GetRssFeed(RssFeed rssFeed)
        {
            using (var connection = new DbConnection())
            {
                connection.Connection.Open();
                connection.SqlCommand.Connection = connection.Connection;

                connection.SqlCommand.CommandText = "select * from feed";
                var reader = connection.SqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    rssFeed.Title = reader["title"].ToString();
                    rssFeed.Body = reader["body"].ToString();
                    rssFeed.Link = reader["link"].ToString();
                    yield return rssFeed;
                }
            }
            yield return rssFeed;
        }

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

        public void AddFeedByItem(RssFeed rssFeed)
        {

            using (var context = new DbConnection())
            {
                context.Connection.Open();
                context.SqlCommand.Connection = context.Connection;
                var user = HttpContext.Current.User.Identity.Name;

                context.SqlCommand.CommandText = "spAddFeedByItem";
                context.SqlCommand.CommandType = CommandType.StoredProcedure;

                context.SqlCommand.Parameters.AddWithValue("@title", rssFeed.Title);
                context.SqlCommand.Parameters.AddWithValue("body", rssFeed.Body);
                context.SqlCommand.Parameters.AddWithValue("@link", rssFeed.Link);
                context.SqlCommand.Parameters.AddWithValue("@userName", user);
                context.SqlCommand.Parameters.AddWithValue("@isRead", false);
                context.SqlCommand.ExecuteNonQuery();
            }
        }
    }
}