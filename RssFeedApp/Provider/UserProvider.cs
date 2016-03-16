using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Threading.Tasks;
using MongoDB.Driver;
using RssFeedApp.Entities;
using RssFeedApp.Models;

namespace RssFeedApp.Provider
{
    public class UserProvider : IUserRepository
    {

        private static UserProvider _instance;

        private UserProvider() { }

        public static UserProvider GetInstance => _instance ?? (_instance = new UserProvider());

        public UserModel RegisterTask(UserModel userModel)
        {
            var user = new UserModel
            {
                UserName = userModel.UserName,
                Password = userModel.Password,
                ConfirmPassword = userModel.ConfirmPassword
            };
            using (var context = new DbConnection())
            {
                context.Connection.Open();
                context.SqlCommand.Connection = context.Connection;

                context.SqlCommand.CommandText = "insert into users(name, password, confirmpassword) values(@name, @password, @confirmpassword)";

                context.SqlCommand.Parameters.AddWithValue("@name", userModel.UserName);
                context.SqlCommand.Parameters.AddWithValue("@password", userModel.Password);
                context.SqlCommand.Parameters.AddWithValue("@confirmpassword", userModel.ConfirmPassword);

                context.SqlCommand.ExecuteNonQuery();
            }

            return user;
        }

        public async Task<UserModel> FindUserTask(string userName, string password)
        {
            UserModel user = null;
            using (var context = new DbConnection())
            {
                context.Connection.Open();
                context.SqlCommand.Connection = context.Connection;

                context.SqlCommand.CommandText = "select * from users where username = @username && password = @password";
                context.SqlCommand.Parameters.AddWithValue("@username", userName);
                context.SqlCommand.Parameters.AddWithValue("@password", password);

                var reader = await context.SqlCommand.ExecuteReaderAsync();
                while (reader.Read())
                {
                    user = (UserModel) reader["username"];
                }
            }
            return user;
        }

        public Client FindClient(string clientId)
        {
            Client user = null;
            using (var context = new DbConnection())
            {
                context.Connection.Open();
                context.SqlCommand.Connection = context.Connection;

                context.SqlCommand.CommandText = "select * from client where id = @clientId";
                context.SqlCommand.Parameters.AddWithValue("@clientId", clientId);

                var reader = context.SqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    user = (Client)reader["id"];
                }
            }
            return user;
        } 

        //public async Task<RefreshToken> AddRefreshToken(RefreshToken token)
        //{
        //    using (var context = new DbConnection())
        //    {
        //        var existingToken = context.RefreshTokenCollection.Find(
        //        Builders<RefreshToken>.Filter.Where(r => r.Subject == token.Subject && r.ClientId == token.ClientId))
        //        .SingleOrDefault();
        //        context.Connection.Open();
        //        context.SqlCommand.Connection = context.Connection;

        //        context.SqlCommand.CommandText = "select * from refreshtoken where subject = @subject && clientid = @clientid";
        //        context.SqlCommand.Parameters.AddWithValue("@subject", token.Subject);
        //        context.SqlCommand.Parameters.AddWithValue("@clientId", token.ClientId);

        //        var reader = context.SqlCommand.ExecuteReader();
        //        if (existingToken != null)
        //        {
        //            RemoveRefreshToken(existingToken);
        //        }

        //        await context.RefreshTokenCollection.InsertOneAsync(token);
        //    }


        //    return existingToken;
        //}

        //public RefreshToken RemoveRefreshToken(string refreshTokenId)
        //{
        //    var refreshToken = _context.RefreshTokenCollection.Find(Builders<RefreshToken>.Filter.Where(r => r.Id == refreshTokenId))
        //        .FirstOrDefault();

        //    if (refreshToken == null) return null;

        //    return _context.RefreshTokenCollection.FindOneAndDelete(
        //        Builders<RefreshToken>.Filter.Where(r => r.Id == refreshTokenId));
        //}

        //public void RemoveRefreshToken(RefreshToken refreshToken)
        //{
        //    _context.RefreshTokenCollection.DeleteOne(r => r.Id == refreshToken.Id);
        //}

        //public async Task<RefreshToken> FindRefreshToken(string refreshTokenId) => await _context.RefreshTokenCollection.Find(Builders<RefreshToken>.Filter.Where(r => r.Id == refreshTokenId)).FirstOrDefaultAsync();

        //public async Task<List<RefreshToken>> GetAllRefreshTokens() => await _context.RefreshTokenCollection.Find(Builders<RefreshToken>.Filter.Empty).ToListAsync();

    }
}