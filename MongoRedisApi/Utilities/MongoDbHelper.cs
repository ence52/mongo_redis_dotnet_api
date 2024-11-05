using Microsoft.AspNetCore.Components.Web;
using MongoDB.Bson;

namespace MongoRedisApi.Utilities
{
    public static class MongoDbHelper
    {
        public static bool isValidObjectId(string id)
        {
            return ObjectId.TryParse(id, out _);
        }
    }
}
