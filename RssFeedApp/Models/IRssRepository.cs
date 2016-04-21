using System.Collections.Generic;

namespace RssFeedApp.Models
{
    public interface IRssRepository
    {
        void AddFeed(Rss urlRss);
        IEnumerable<Feed> GetFeeds();
        IEnumerable<Rss> GetRss();
        void RemoveFeed(Feed feed);
        void SetReadItem(Feed feed);
    }
}