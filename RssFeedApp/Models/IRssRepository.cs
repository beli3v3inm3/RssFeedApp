using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RssFeedApp.Models
{
    public interface IRssRepository
    {
        void AddFeed(Rss urlRss);
        IEnumerable<Feed> GetFeeds(Feed feed);
        IEnumerable<Rss> GetRss(Rss rss);
        void RemoveFeed(Feed feed);
        void SetReadItem(Feed feed);
    }
}