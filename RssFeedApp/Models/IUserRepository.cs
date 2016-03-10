using System.Threading.Tasks;
using MongoDB.Driver;

namespace RssFeedApp.Models
{
    public interface IUserRepository
    {
        Task<UserModel> RegisterTask(UserModel userModel);
        Task<IAsyncCursor<UserModel>> FindUserTask(string userName);

    }
}
