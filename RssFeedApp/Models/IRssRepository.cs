using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RssFeedApp.Models
{
    public interface IRssRepository
    {
        void AddFeed(UrlFeed urlFeed);
        IEnumerable<RssFeed> GetRssFeed(RssFeed rssFeed);
        IEnumerable<RssFeed> GetRss(RssFeed rssFeed);
        void RemoveFeed(RssFeed rssFeed);
        void SetReadItem(RssFeed rssFeed);
    }
}