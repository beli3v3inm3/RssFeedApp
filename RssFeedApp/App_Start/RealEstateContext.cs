using System;
using MongoDB.Driver;
using RssFeedApp.Entities;
using RssFeedApp.Models;
using RssFeedApp.Properties;

namespace RssFeedApp
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

        public IMongoCollection<UserModel> UserCollection => MongoDatabase.GetCollection<UserModel>("Users");
        public IMongoCollection<Client> ClientCollection => MongoDatabase.GetCollection<Client>("Clients");
        public IMongoCollection<RefreshToken> RefreshTokenCollection => MongoDatabase.GetCollection<RefreshToken>("RefreshToken"); 

    }
}