using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Xml;
using RssFeedApp.Models;

namespace RssFeedApp.Provider
{
    public class RssProvider
    {
        private static volatile RssProvider _instance;
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

        public void AddFeed(RssFeed rssFeed)
        {
            using (var connection = new DbConnection())
            {
                connection.Connection.Open();

                connection.SqlCommand.CommandText = "insert into Feed(url, title, body, link) values(@url, @title, @body, @link); insert into userstofeed(userid, feedid)";
                if (rssFeed.Url != null)
                {
                    var reader = new XmlTextReader(rssFeed.Url);
                    var feed = SyndicationFeed.Load(reader);

                    if (feed == null) return;
                    foreach (var item in feed.Items)
                    {
                        connection.SqlCommand.Parameters.AddWithValue("@url", rssFeed.Url);
                        connection.SqlCommand.Parameters.AddWithValue("@title", item.Title.Text);
                        connection.SqlCommand.Parameters.AddWithValue("@body", item.Summary.Text);
                        connection.SqlCommand.Parameters.AddWithValue("@link", item.Id);
                        connection.SqlCommand.ExecuteNonQuery();
                        connection.SqlCommand.Parameters.Clear();
                    }
                }
                else
                {
                    //connection.SqlCommand.Parameters.AddWithValue("@url", rssFeed.Url);
                    //connection.SqlCommand.Parameters.AddWithValue("@title", item.Title.Text);
                    //connection.SqlCommand.Parameters.AddWithValue("@body", item.Summary.Text);
                    //connection.SqlCommand.ExecuteNonQuery();
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
                    rssFeed.Url = reader["url"].ToString();
                    rssFeed.Title = reader["title"].ToString();
                    rssFeed.Body = reader["body"].ToString();
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

                context.SqlCommand.CommandText = "insert into feed(title, body, link) values(@title, @body, @link)";

                context.SqlCommand.Parameters.AddWithValue("@title", rssFeed.Title);
                context.SqlCommand.Parameters.AddWithValue("@body", rssFeed.Body);
                context.SqlCommand.Parameters.AddWithValue("@link", rssFeed.Link);
                context.SqlCommand.ExecuteNonQuery();
            }
        }
    }
}