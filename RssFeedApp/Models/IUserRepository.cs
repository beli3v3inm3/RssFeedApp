using System.Collections.Generic;
using System.Threading.Tasks;
using RssFeedApp.Entities;

namespace RssFeedApp.Models
{
    public interface IUserRepository
    {
        UserModel RegisterTask(UserModel userModel);
        Task<UserModel> FindUserTask(string userName, string password);
        Client FindClient(string clientId);
        //Task<RefreshToken> AddRefreshToken(RefreshToken token);
        //RefreshToken RemoveRefreshToken(string refreshTokenId);
        //void RemoveRefreshToken(RefreshToken refreshToken);
        //Task<RefreshToken> FindRefreshToken(string refreshTokenId);
        //Task<List<RefreshToken>> GetAllRefreshTokens();

    }
}
