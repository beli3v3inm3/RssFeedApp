using System;
using System.Data.SqlClient;
using RssFeedApp.Properties;

namespace RssFeedApp
{
    public class RssConnection : IDisposable
    {
        private readonly string _conString = Settings.Default.RssConnectionString;
        public SqlConnection Connection;
        public SqlCommand SqlCommand;

        public RssConnection()
        {
            Connection = new SqlConnection(_conString);
            SqlCommand = new SqlCommand();
        }

        public void Dispose()
        {
            Connection.Close();
        }
    }
}