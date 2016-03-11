using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using MongoDB.Driver;
using RssFeedApp.Entities;
using RssFeedApp.Provider;

namespace RssFeedApp.Models
{
    public class UserRepository : IUserRepository
    {
        private readonly RealEstateContext _context;

        public UserRepository()
        {
            _context = new RealEstateContext();
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

        public async Task<UserModel> FindUserTask(string userName, string password) => await _context.UserCollection.Find(Builders<UserModel>.Filter.Where(q => q.UserName == userName && q.Password == password)).FirstOrDefaultAsync();

        public Client FindClient(string clientId) => _context.ClientCollection.Find(Builders<Client>.Filter.Where(q => q.Id == clientId)).FirstOrDefault();
        
        public async Task<RefreshToken> AddRefreshToken(RefreshToken token)
        {

            var existingToken = _context.RefreshTokenCollection.Find(
                Builders<RefreshToken>.Filter.Where(r => r.Subject == token.Subject && r.ClientId == token.ClientId))
                .SingleOrDefault();

            if (existingToken != null)
            {
                RemoveRefreshToken(existingToken);
            }

            await _context.RefreshTokenCollection.InsertOneAsync(token);

            return existingToken;
        }

        public RefreshToken RemoveRefreshToken(string refreshTokenId)
        {
            var refreshToken = _context.RefreshTokenCollection.Find(Builders<RefreshToken>.Filter.Where(r => r.Id == refreshTokenId))
                .FirstOrDefault();

            if (refreshToken == null) return null;
            
            return _context.RefreshTokenCollection.FindOneAndDelete(
                Builders<RefreshToken>.Filter.Where(r => r.Id == refreshTokenId));
        }

        public void RemoveRefreshToken(RefreshToken refreshToken)
        {
            _context.RefreshTokenCollection.DeleteOne(r => r.Id == refreshToken.Id);
        }

        public async Task<RefreshToken> FindRefreshToken(string refreshTokenId) => await _context.RefreshTokenCollection.Find(Builders<RefreshToken>.Filter.Where(r => r.Id == refreshTokenId)).FirstOrDefaultAsync();

        public async Task<List<RefreshToken>> GetAllRefreshTokens() => await _context.RefreshTokenCollection.Find(Builders<RefreshToken>.Filter.Empty).ToListAsync();

    }
}