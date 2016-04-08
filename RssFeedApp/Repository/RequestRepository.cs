using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using RssFeedApp.Models;
using RssFeedApp.Properties;

namespace RssFeedApp.Repository
{
    public class RequestRepository : IRequestRepository, IDisposable
    {
        private static readonly string ConString = Settings.Default.RssConnectionString;
        private readonly SqlConnection _connection;

        public RequestRepository()
        {
            _connection = new SqlConnection(ConString);
            _connection.Open();
        }

        public void ExecuteProcedure(string query, params SqlParameter[] parameters)
        {
            using (var cmd = new SqlCommand(query, _connection))
            {
                if (parameters == null) return;

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(parameters);
                cmd.ExecuteNonQuery();
            }
        }

        //fx
        public IEnumerable<RssFeed> ExecuteReader(string query, RssFeed rssFeed, params SqlParameter[] parameters)
        {
            using (var cmd = new SqlCommand(query, _connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(parameters);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        yield return rssFeed;
                    }
                }
            } 
            yield return null;
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}