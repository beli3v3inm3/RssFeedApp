﻿using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using RssFeedApp.Entities;
using RssFeedApp.Models;

namespace RssFeedApp.Provider
{
    public class UserRepository : IUserRepository
    {
        private readonly IRequestRepository _repository;

        public UserRepository(IRequestRepository repository)
        {
            _repository = repository;
        }

        public UserModel RegisterTask(UserModel userModel)
        {
            const string addUserQuery = "insert into users(name, password, salt) values(@name, @password, @salt)"; ;
            var salt = CreateSalt();
            var hashedPassword = Hash(userModel.Password, salt);
            var user = new UserModel
            {
                UserName = userModel.UserName,
                HashedPassword = hashedPassword,
                Salt = salt
            };

            _repository.ExecuteQuery(
                    addUserQuery,
                    new SqlParameter("@name", user.UserName),
                    new SqlParameter("@password", hashedPassword),
                    new SqlParameter("@salt", user.Salt));


            return user;
        }


        public string FindUser(string userName, string password)
        {
            var correctCredentials = ConfirmPassword(userName, password);
            return !correctCredentials ? null : userName;
        }

        public Client FindClient(string clientId)
        {
            Client user = null;
            const string findClientQuery = "select id from client where id = @clientId";


            foreach (var items in _repository.ExecuteQueryReader(findClientQuery, new SqlParameter("@clientId", clientId)))
            {
                user = (Client)items[0];
            }

            return user;
        }

        public UserModel GetUserId(string userName)
        {
            const string getUserIdQuery = "select id from users where name = @userName";
            UserModel user = null;


            foreach (var items in _repository.ExecuteQueryReader(getUserIdQuery, new SqlParameter("@userName", userName)))
            {
                user = items[0] as UserModel;
            }


            return user;
        }

        
        public async Task<RefreshToken> AddRefreshToken(RefreshToken token)
        {
            RefreshToken existingToken = null;
            using (var context = new DbConnection())
            {

                context.Connection.Open();
                context.SqlCommand.Connection = context.Connection;

                context.SqlCommand.CommandText = "select * from refreshtokens where subject = @subject && clientid = @clientid";
                context.SqlCommand.Parameters.AddWithValue("@subject", token.Subject);
                context.SqlCommand.Parameters.AddWithValue("@clientid", token.ClientId);

                var reader = await context.SqlCommand.ExecuteReaderAsync();
                while (reader.Read())
                {
                    existingToken = (RefreshToken)reader["subject"];
                }
                if (existingToken != null)
                {
                    await RemoveRefreshToken(existingToken.ToString());
                }
                context.SqlCommand.Parameters.Clear();
                context.SqlCommand.CommandText = "insert into refreshtokens(id, subject, clientid, issuedutc, expiredutc, protectedticket) values(@id, @subject, @clientid, @issuedutc, @expiredutc, @protectedticket)";
                context.SqlCommand.Parameters.AddWithValue("@id", token.Id);
                context.SqlCommand.Parameters.AddWithValue("@subject", token.Subject);
                context.SqlCommand.Parameters.AddWithValue("@clientid", token.ClientId);
                context.SqlCommand.Parameters.AddWithValue("@issuedutc", token.IssuedUtc);
                context.SqlCommand.Parameters.AddWithValue("@expiredutc", token.ExpiresUtc);
                context.SqlCommand.Parameters.AddWithValue("@protectedticket", token.ProtectedTicket);

                context.SqlCommand.ExecuteNonQuery();
            }

            return existingToken;
        }

        
        public async Task RemoveRefreshToken(string refreshTokenId)
        {
            using (var context = new DbConnection())
            {
                context.Connection.Open();
                context.SqlCommand.Connection = context.Connection;

                context.SqlCommand.CommandText = "delete from refreshtokens where id = @refreshtoken";
                context.SqlCommand.Parameters.AddWithValue("@refreshtoken", refreshTokenId);
                await context.SqlCommand.ExecuteNonQueryAsync();
            }
        }

        
        public void RemoveRefreshToken(RefreshToken refreshToken)
        {
            using (var context = new DbConnection())
            {
                context.Connection.Open();
                context.SqlCommand.Connection = context.Connection;

                context.SqlCommand.CommandText = "delete from refreshtokens where id = @refreshtoken";
                context.SqlCommand.Parameters.AddWithValue("@refreshtoken", refreshToken.Id);
                context.SqlCommand.ExecuteNonQuery();
            }
        }

        
        public RefreshToken FindRefreshToken(string refreshTokenId)
        {
            RefreshToken refreshToken = null;
            using (var context = new DbConnection())
            {
                context.Connection.Open();
                context.SqlCommand.Connection = context.Connection;

                context.SqlCommand.CommandText = "select * from refreshtokens where id = @refreshtoken";
                context.SqlCommand.Parameters.AddWithValue("@refreshtoken", refreshTokenId);
                var reader = context.SqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    refreshToken = (RefreshToken)reader["id"];
                }
            }
            return refreshToken;
        }

        private static byte[] CreateSalt()
        {
            var userSalt = new byte[64];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(userSalt);
            }

            return userSalt;
        }

        public static byte[] Hash(string userPassword, byte[] salt)
        {
            return Hash(Encoding.UTF8.GetBytes(userPassword), salt);
        }

        public static byte[] Hash(byte[] userPassword, byte[] salt)
        {
            var saltedValue = userPassword.Concat(salt).ToArray();

            return new SHA256Managed().ComputeHash(saltedValue);
        }

        public bool ConfirmPassword(string userName, string password)
        {
            const string getHashQuery = "select * from users where name = @userName";
            var tempPass = new byte[64];
            var tempSalt = new byte[64];

            foreach (var items in _repository.ExecuteQueryReader(getHashQuery, new SqlParameter("@username", userName)))
            {
                tempPass = items[2] as byte[];
                tempSalt = items[3] as byte[];
            }

            var passwordHash = Hash(password, tempSalt);

            return passwordHash.SequenceEqual(tempPass);
        }
    }
}