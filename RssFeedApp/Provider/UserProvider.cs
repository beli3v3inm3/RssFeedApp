using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Security;
using RssFeedApp.Entities;
using RssFeedApp.Models;

namespace RssFeedApp.Provider
{
    public class UserProvider : IUserRepository
    {
        private static volatile UserProvider _instance;
        private static readonly object SyncRoot = new object();

        private UserProvider() { }

        public static UserProvider GetInstance
        {
            get
            {
                if (_instance != null) return _instance;
                lock (SyncRoot)
                {
                    if (_instance == null)
                        _instance = new UserProvider();
                }

                return _instance;
            }
        }

        public UserModel RegisterTask(UserModel userModel)
        {
            var salt = CreateSalt();
            var hashedPassword = CreatePasswordHash(userModel.Password, salt);
            var user = new UserModel
            {
                UserName = userModel.UserName,
                HashedPassword = hashedPassword,
                Salt = salt
            };
            using (var context = new DbConnection())
            {
                context.Connection.Open();
                context.SqlCommand.Connection = context.Connection;

                context.SqlCommand.CommandText = "insert into users(name, password, salt) values(@name, @password, @salt)";

                context.SqlCommand.Parameters.AddWithValue("@name", user.UserName);
                context.SqlCommand.Parameters.AddWithValue("@password", hashedPassword);
                context.SqlCommand.Parameters.AddWithValue("@salt", user.Salt);

                context.SqlCommand.ExecuteNonQuery();
            }
            ConfirmPassword(userModel.Password);

            return user;
        }


        public async Task<string> FindUserTask(string userName, string password)
        {
            string user = null;
            using (var context = new DbConnection())
            {
                context.Connection.Open();
                context.SqlCommand.Connection = context.Connection;

                context.SqlCommand.CommandText = "select * from users where name = @username and password = @password";
                context.SqlCommand.Parameters.AddWithValue("@username", userName);
                context.SqlCommand.Parameters.AddWithValue("@password", password);

                var reader = await context.SqlCommand.ExecuteReaderAsync();
                while (reader.Read())
                {
                    user = reader["name"].ToString();
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

        public UserModel GetUserId(string userName)
        {
            UserModel user = null;
            using (var context = new DbConnection())
            {
                context.Connection.Open();

                context.SqlCommand.CommandText = "select id from users where name = @userName";

                context.SqlCommand.Parameters.AddWithValue("@userName", userName);

                var reader = context.SqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    user = reader["id"] as UserModel;
                }
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
                    refreshToken = (RefreshToken) reader["id"];
                }
            }
            return refreshToken;
        }

        private static byte[] CreateSalt()
        {
            var byteArr = new byte[64];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(byteArr);
            }

            return byteArr;
        }

        private static byte[] CreatePasswordHash(string password, byte[] salt)
        {
            var pwdToBytes = Encoding.ASCII.GetBytes(password);
            HashAlgorithm hashAlgorithm;
            var pwdWithSalt = new byte[pwdToBytes.Length + salt.Length];

            for (var i = 0; i < pwdToBytes.Length; i++)
            {
                pwdWithSalt[i] = pwdToBytes[i];
            }
            for (var i = 0; i < salt.Length; i++)
            {
                pwdWithSalt[pwdToBytes.Length + i] = salt[i];
            }

            using (hashAlgorithm = new SHA256Managed())
            {
                return hashAlgorithm.ComputeHash(pwdWithSalt);
            }
        }

        public static byte[] Hash(string value, byte[] salt)
        {
            salt = new byte[64];
            //    using (var rng = new RNGCryptoServiceProvider())
            //    {
            //        rng.GetBytes(byteArr);
            //    }
            return Hash(Encoding.UTF8.GetBytes(value), salt);
        }

        public static byte[] Hash(byte[] value, byte[] salt)
        {
            var saltedValue = value.Concat(salt).ToArray();
            // Alternatively use CopyTo.
            //var saltedValue = new byte[value.Length + salt.Length];
            //value.CopyTo(saltedValue, 0);
            //salt.CopyTo(saltedValue, value.Length);

            return new SHA256Managed().ComputeHash(saltedValue);
        }

        public bool ConfirmPassword(string password)
        {
            var passwordSalt = new byte[64];
            using (var context = new DbConnection())
            {
                context.Connection.Open();
                context.SqlCommand.Connection = context.Connection;

                context.SqlCommand.CommandText = "select salt from users where name='test'";
                var reader = context.SqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    passwordSalt = reader["salt"] as byte[];
                }
            }
            var passwordHash = Hash(password, passwordSalt);

            return passwordHash.SequenceEqual(passwordHash);
        }

    }
}