using Discord;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Sparrows.Bot.Models {
    public record OrderCounter {
        [BsonId]
        public ObjectId Id;
        public required ulong DiscordId;
        public required int OrderCount;
    }
}