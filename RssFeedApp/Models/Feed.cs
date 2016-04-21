using System;

namespace RssFeedApp.Models
{
    public class Feed
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Link { get; set; }
        public string Url { get; set; }
        public bool IsRead { get; set; }
        public DateTime? PubDate { get; set; }
    }
}