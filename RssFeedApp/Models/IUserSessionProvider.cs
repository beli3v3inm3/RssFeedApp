namespace RssFeedApp.Models
{
    public interface IUserSessionProvider
    {
        void SetUser(string usr);
        string GetUser();
    }
}
