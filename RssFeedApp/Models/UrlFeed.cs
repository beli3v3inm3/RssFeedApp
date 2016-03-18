namespace RssFeedApp.Models
{
    public class UrlFeed
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public int FeedId { get; set; }
    }
}