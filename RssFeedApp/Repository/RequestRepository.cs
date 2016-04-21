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
        private bool _disposed;

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

        public void ExecuteQuery(string query, params SqlParameter[] parameters)
        {
            using (var cmd = new SqlCommand(query, _connection))
            {
                if (parameters == null) return;

                cmd.CommandText = query;
                cmd.Parameters.AddRange(parameters);
                cmd.ExecuteNonQuery();
            }
        }

        public IEnumerable<object[]> ExecuteProcReader(string query, params SqlParameter[] parameters)
        {
            using (var cmd = new SqlCommand(query, _connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(parameters);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var values = new object[reader.FieldCount];
                        reader.GetValues(values);
                        yield return values;    
                    }
                }
            }
        }

        public IEnumerable<object[]> ExecuteQueryReader(string query, params SqlParameter[] parameters)
        {
            using (var cmd = new SqlCommand(query, _connection))
            {
                cmd.CommandText = query;
                cmd.Parameters.AddRange(parameters);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var values = new object[reader.FieldCount];
                        reader.GetValues(values);
                        yield return values;
                    }
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _connection?.Dispose();
            }

            _disposed = true;
        }
    }
}