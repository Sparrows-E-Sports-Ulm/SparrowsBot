using Discord.Interactions;
using Sparrows.Bot.Services;
using Sparrows.Bot.Models;
using Discord;
using Microsoft.Extensions.Configuration;

namespace Sparrows.Bot.Commands {
   
    [Group("order", "Order food")]
    public class OrderCommand : InteractionModuleBase<SocketInteractionContext> {

        public OrderCommand(IOrderService orderService, IUserService userService, IConfiguration config) {
            m_OrderService = orderService;
            m_UserService = userService;
            m_IsOrderingUnlocked = false;
            m_AdminUsers = new List<ulong>();

            var adminUsers = config.GetRequiredSection("admin_users").GetChildren().ToArray();
            foreach(var user in adminUsers) {
                m_AdminUsers.Add(ulong.Parse(user.Value));
            }

            #if DEBUG
                m_IsOrderingUnlocked = true;
            #endif
        }

        [SlashCommand("food_now", "Opens up the orders", true)]
        public async Task FoodNow(string restaurantName, string url, string startTime, string endTimer) {
            if(!m_AdminUsers.Contains(Context.User.Id)) {
                await RespondAsync("No permissions");
                return;
            }

            m_IsOrderingUnlocked = true;

            if(!url.StartsWith("http")) {
                url = "https://" + url;
            }

            var embed = new EmbedBuilder {
                Title = "Essens Bestelling | Food Order 🍕🍴",
                Color = Color.Blue
            };

            embed.AddField("German", $"Hi, wir bestellem ab **{startTime}** bis **{endTimer}** bei **{restaurantName}**. Ihr könnt euch [hier]({url}) das Menü anschauen. Bitte nutzt ``/order add <Anzahl> <Gericht Nummer> <Gereicht Name>`` um Essen zu bestellen.");
            embed.AddField("English", $"Hi, we are ordering food from **{startTime}** until **{endTimer}** at **{restaurantName}**. You can check out the menu [here]({url}). Please use ``/order add <Amount> <Dish Number> <Dish Name>`` to order food.");
            embed.WithUrl(url);
            embed.WithImageUrl("https://upload.wikimedia.org/wikipedia/commons/a/a3/Eq_it-na_pizza-margherita_sep2005_sml.jpg");
            embed.WithCurrentTimestamp();
        
            await RespondAsync(embed: embed.Build());    
        }

        [SlashCommand("food_done", "Closes the orders", true)]
        public async Task FoodDone() {
            if(!m_AdminUsers.Contains(Context.User.Id)) {
                await RespondAsync("No permissions");
                return;
            }

            m_IsOrderingUnlocked = false;

            var orders = m_OrderService.GetAllOrders();
            
            var fileStream = new MemoryStream();
            var writer = new StreamWriter(fileStream);

            writer.WriteLine("Discord User;Vorname;Nachname;Anzahl;Nummer;Gericht;PayPal");

            foreach(var userid in orders.Keys) {
                User user = m_UserService.Get(userid);
                var basket = orders[userid];

                foreach(var order in basket) {
                    writer.WriteLine($"{user.DiscordName};{user.FirstName};{user.LastName};{order.Amount};{order.DishNumber};{order.DishName};{user.PayPal}");
                }
            }

            writer.Flush();

            FileAttachment attachment = new FileAttachment(fileStream, "Order.csv");

            await Context.User.CreateDMChannelAsync();
            await Context.User.SendFileAsync(attachment);

            await RespondAsync($"Orders are now closed - Check your DMs {Context.User.Mention}");
        }

        [SlashCommand("add", "Add a dish to your order")]
        public async Task Add(int amount, string dish_number, string dish_name) {
            if(!m_IsOrderingUnlocked) {
                await RespondAsync("Hi, we are currently not ordering any food. Please wait until the next announcement to order food.");
            }

            if(!m_UserService.Exists(Context.User.Id)) {
                await RespondAsync("Please register using /register first before ordering food");
                return; 
            }

            m_OrderService.AddOrder(Context.User.Id, new Order {
                DiscordUserId = Context.User.Id,
                DishNumber = dish_number,
                DishName = dish_name,
                Amount = amount
            });

            await RespondAsync($"{amount}x {dish_name} ({dish_number}) has been added to your order");
        }

        [SlashCommand("remove", "Remove a dish from your order")]
        public async Task Remove(int number) {
            if(!m_IsOrderingUnlocked) {
                await RespondAsync("Hi, we are currently not ordering any food. Please wait until the next announcement to order food.");
            }

            int index = number - 1;

            var orders = m_OrderService.GetOrders(Context.User.Id);

            if(index < 0 || index > (orders.Count - 1)) {
                await RespondAsync("Invalid Index");
                return;
            }

            Order order = orders[index];
            m_OrderService.RemoveOrder(Context.User.Id, index);   

            await RespondAsync($"Your Order {order.Amount}x {order.DishName} ({order.DishNumber}) has been removed!");
        }

        [SlashCommand("list", "List your current orders")]
        public async Task List() {
            if(!m_IsOrderingUnlocked) {
                await RespondAsync("Hi, we are currently not ordering any food. Please wait until the next announcement to order food.");
            }
            
            var orders = m_OrderService.GetOrders(Context.User.Id);

            if(orders.Count == 0) {
                await RespondAsync("You don't have any orders currently");
                return;
            }

            string desc = "";
            for(int i = 0; i < orders.Count; i++) {
                desc += $"**{i+1}**. {orders[i].Amount}x {orders[i].DishName} ({orders[i].DishNumber})\n";
            }

            var embed = new EmbedBuilder {
                Title="Your Orders",
                Color=Color.Blue,
                Description=desc
            };

            await RespondAsync(embed: embed.Build());
        }

        private IOrderService m_OrderService;
        private IUserService m_UserService;
        private bool m_IsOrderingUnlocked;
        private List<ulong> m_AdminUsers;
    }
}