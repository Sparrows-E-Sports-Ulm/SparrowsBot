using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Sparrows.Bot.Models {
    public record Order {
        [BsonId]
        public ObjectId Id;
        public int Index;
        public required ulong DiscordUserId;
        public required string DishNumber;
        public required string DishName;
        public required int Amount;
        public required decimal ItemPrice;
    }
}