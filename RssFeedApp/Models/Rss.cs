using System;

namespace RssFeedApp.Models
{
    public class Rss
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public DateTime PubDate { get; set; }
    }
}