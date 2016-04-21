using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web;
using System.Xml;
using RssFeedApp.Models;
using RssFeedApp.Repository;

namespace RssFeedApp.Provider
{
    public class RssRepository : IRssRepository
    {
        private readonly IRequestRepository _requestRepository;

        public RssRepository(IRequestRepository repository)
        {
            _requestRepository = repository;
        }

        public void AddFeed(Rss rss)
        {
            const string addFeedProc = "spAddFeed";
            const string addFeedItemProc = "spAddFeedItems";

            var reader = new XmlTextReader(rss.Url);
            var syndicationFeed = SyndicationFeed.Load(reader);
            var user = HttpContext.Current.User.Identity.Name;

            if (syndicationFeed == null) return;

            _requestRepository.ExecuteProcedure(
                addFeedProc,
                new SqlParameter("@url", rss.Url),
                new SqlParameter("@title", syndicationFeed.Title.Text),
                new SqlParameter("@description", syndicationFeed.Description.Text),
                new SqlParameter("@imageUrl", syndicationFeed.ImageUrl?.AbsoluteUri),
                new SqlParameter("@lastBuildDate", syndicationFeed.LastUpdatedTime.DateTime == DateTime.MinValue ? (DateTime?)null : syndicationFeed.LastUpdatedTime.DateTime),
                new SqlParameter("@userName", user));


            foreach (var item in syndicationFeed.Items)
            {
                _requestRepository.ExecuteProcedure(
                    addFeedItemProc,
                    new SqlParameter("@url", rss.Url),
                    new SqlParameter("@title", item.Title.Text),
                    new SqlParameter("@body", item.Summary.Text),
                    new SqlParameter("@link", item.Links[0].Uri.AbsoluteUri),
                    new SqlParameter("@pubDate", item.PublishDate.DateTime == DateTime.MinValue ? (DateTime?)null : item.PublishDate.DateTime),
                    new SqlParameter("@userName", user),
                    new SqlParameter("@isRead", false));
            }

        }

        public IEnumerable<Feed> GetFeeds()
        {
            const string getFeedsProc = "spGetItemsByUser";
            var user = HttpContext.Current.User.Identity.Name;

            foreach (var items in _requestRepository.ExecuteProcReader(
                getFeedsProc,
                new SqlParameter("@userName", user)))
            {
                var feed = new Feed
                {
                    Id = (int)items[0],
                    Title = items[1].ToString(),
                    Body = items[2].ToString(),
                    Link = items[3].ToString(),
                    IsRead = (bool) items[5]
                };

                if (items[4] is DateTime)
                {
                    feed.PubDate = (DateTime)items[4];
                }


                yield return feed;
            }
        }

        public IEnumerable<Rss> GetRss()
        {
            const string getRssProc = "GetRssByUser";
            var user = HttpContext.Current.User.Identity.Name;

            return _requestRepository.ExecuteProcReader(
                getRssProc,
                new SqlParameter("@userName", user)).Select(items => new Rss
                {
                    Id = (int)items[0],
                    Title = items[2].ToString(),
                    Description = items[3].ToString(),
                    ImageUrl = items[4].ToString(),
                    PubDate = (DateTime?)items[5]
                });
        }


        public void RemoveFeed(Feed feed)
        {
            const string removeFeedItem = "spRemoveRssItem";
            var user = HttpContext.Current.User.Identity.Name;

            _requestRepository.ExecuteProcedure(
                removeFeedItem,
                new SqlParameter("@userName", user),
                new SqlParameter("@Id", feed.Id));

        }

        public void SetReadItem(Feed feed)
        {
            const string setReadItemProc = "spReadFeedItemEdit";

            _requestRepository.ExecuteProcedure(
                    setReadItemProc,
                    new SqlParameter("@feeId", feed.Id),
                    new SqlParameter("@isRead", true));

        }
    }
}