using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Xml;
using RssFeedApp.Models;

namespace RssFeedApp.Provider
{
    public class RssProvider
    {
        private readonly RssConnection _connection;
        private SyndicationFeed _feed;

        public RssProvider()
        {
            _connection = new RssConnection();
        }

        public void AddFeed(RssFeed rssFeed)
        {
            var reader = new XmlTextReader(rssFeed.Url);
            _feed = SyndicationFeed.Load(reader);

            _connection.Connection.Open();
            _connection.SqlCommand.Connection = _connection.Connection;

            _connection.SqlCommand.CommandText = "insert into Feed(url, title, body) values(@url, @title, @body)";

            //_connection.SqlCommand.Parameters.AddWithValue("@url", rssFeed.Url);

            if (_feed != null)
            {
                foreach (var item in _feed.Items)
                {
                    _connection.SqlCommand.Parameters.AddWithValue("@url", rssFeed.Url);
                    _connection.SqlCommand.Parameters.AddWithValue("@title", item.Title.Text);
                    _connection.SqlCommand.Parameters.AddWithValue("@body", item.Summary.Text);
                    _connection.SqlCommand.ExecuteNonQuery();
                    _connection.SqlCommand.Parameters.Clear();
                }
            }


            _connection.Dispose();
        }

        public IEnumerable<RssFeed> GetRssFeed(RssFeed rssFeed)
        {
            _connection.Connection.Open();
            _connection.SqlCommand.Connection = _connection.Connection;

            _connection.SqlCommand.CommandText = "select * from feed";
            var reader = _connection.SqlCommand.ExecuteReader();
            while (reader.Read())
            {
                rssFeed.Url = reader["url"].ToString();
                rssFeed.Title = reader["title"].ToString();
                rssFeed.Body = reader["body"].ToString();
                yield return rssFeed;
            }
            _connection.Dispose();

            yield return rssFeed;
        }
    }
}