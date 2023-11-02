using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Sparrows.Bot.Models {
    public record User {
        [BsonId]
        public ObjectId Id;
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public required ulong DiscordId { get; init; }
        public required string DiscordName { get; init; }
        public required string PayPal { get; init; }
    };
}