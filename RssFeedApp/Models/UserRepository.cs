using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MongoDB.Driver;

namespace RssFeedApp.Models
{
    public class UserRepository : IUserRepository
    {
        private readonly RealEstateContext _context;

        public UserRepository(RealEstateContext context)
        {
            _context = context;
        }

        public async Task<UserModel> RegisterTask(UserModel userModel)
        {
            var user = new UserModel
            {
                UserName = userModel.UserName,
                Password = userModel.Password,
                ConfirmPassword = userModel.ConfirmPassword
            };

            await _context.UserCollection.InsertOneAsync(user);

            return user;
        }

        public async Task<IAsyncCursor<UserModel>> FindUserTask(string userName) => await _context.UserCollection.FindAsync(userName);

    }
}