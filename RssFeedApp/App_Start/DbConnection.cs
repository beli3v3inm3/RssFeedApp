using System;
using System.Data.SqlClient;
using RssFeedApp.Properties;

namespace RssFeedApp
{
    public class DbConnection : IDisposable
    {
        private readonly string _conString = Settings.Default.RssConnectionString;
        public SqlConnection Connection;
        public SqlCommand SqlCommand;

        public DbConnection()
        {
            Connection = new SqlConnection(_conString);
            SqlCommand.Connection = Connection;
        }

        public void Dispose()
        {
            Connection.Dispose();
            SqlCommand.Dispose();
        }
    }
}