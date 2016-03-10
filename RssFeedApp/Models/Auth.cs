using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RssFeedApp.Models
{
    public class Auth
    {
        public List<UserModel> UserModels = new List<UserModel>();
         
    }
}