using System;

namespace RssFeedApp.Models
{
    public class RssFeed
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Content { get; set; }
        public string Link { get; set; }
        public string Url { get; set; }
        public bool IsRead { get; set; }
        public DateTime PubDate { get; set; }
        public string Image { get; set; }
        public string FeedTitle { get; set; }
        public string FeedDescription { get; set; }
        public DateTime FeedLastBuildDate { get; set; }
    }
}