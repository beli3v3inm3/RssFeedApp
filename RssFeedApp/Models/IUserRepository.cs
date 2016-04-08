using System.Threading.Tasks;
using RssFeedApp.Entities;

namespace RssFeedApp.Models
{
    public interface IUserRepository
    {
        UserModel RegisterTask(UserModel userModel);
        Task<string> FindUserTask(string userName, string password);
        Client FindClient(string clientId);
        UserModel GetUserId(string userName);
        Task<RefreshToken> AddRefreshToken(RefreshToken token);
        Task RemoveRefreshToken(string refreshTokenId);
        void RemoveRefreshToken(RefreshToken refreshToken);
        RefreshToken FindRefreshToken(string refreshTokenId);


    }
}
