using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoRedisApi.Abstract;

namespace MongoRedisApi.Models
{
    public class User : IUser
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set ; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
