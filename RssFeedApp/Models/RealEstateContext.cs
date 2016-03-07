using MongoDB.Driver;
using RssFeedApp.Properties;

namespace RssFeedApp.Models
{
    public class RealEstateContext
    {
        public readonly IMongoDatabase MongoDatabase;
        public readonly IMongoClient Client;


        public RealEstateContext()
        {
            Client = new MongoClient(Settings.Default.RealEstateConnectionString);
            MongoDatabase = Client.GetDatabase(Settings.Default.RealEstateDatabaseName);
        }
    }
}