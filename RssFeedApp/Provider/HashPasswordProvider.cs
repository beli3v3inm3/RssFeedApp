using System.Linq;
using System.Security.Cryptography;
using System.Text;


namespace RssFeedApp.Provider
{
    public class HashPasswordProvider
    {
        public byte[] CreateSalt()
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
            //var userSalt = new byte[64];
            var tempPass = new byte[64];
            var tempSalt = new byte[64];
            using (var context = new DbConnection())
            {
                context.Connection.Open();
                context.SqlCommand.Connection = context.Connection;

                context.SqlCommand.CommandText = "select * from users where name = @userName";
                context.SqlCommand.Parameters.AddWithValue("@username", userName);
                var reader = context.SqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    tempPass = reader["password"] as byte[];
                    tempSalt = reader["salt"] as byte[];
                }
            }
            var passwordHash = Hash(password, tempSalt);

            return passwordHash.SequenceEqual(tempPass);
        }
    }
}