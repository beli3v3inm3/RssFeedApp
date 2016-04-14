using System.Collections.Generic;
using System.Data.SqlClient;

namespace RssFeedApp.Models
{
    public interface IRequestRepository
    {
        void ExecuteProcedure(string query, params SqlParameter[] parameters);
        void ExecuteQuery(string query, params SqlParameter[] parameters);
        IEnumerable<object[]> ExecuteProcReader(string query, params SqlParameter[] parameters);
        IEnumerable<object[]> ExecuteQueryReader(string query, params SqlParameter[] parameters);
        void Dispose();
    }
}
