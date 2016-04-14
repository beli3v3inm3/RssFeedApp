using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RssFeedApp.Models
{
    public interface IRssRepository
    {
        void AddFeed(Rss urlRss);
        IEnumerable<Feed> GetFeeds();
        IEnumerable<Rss> GetRss();
        void RemoveFeed(Feed feed);
        void SetReadItem(Feed feed);
        void Dispose();
    }
}