namespace Sparrows.Bot.Models {
    public record Order {
        public required ulong DiscordUserId;
        public required string DishNumber;
        public required string DishName;
        public required int Amount;
    }
}