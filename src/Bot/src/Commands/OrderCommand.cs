using Discord.Interactions;
using Sparrows.Bot.Services;
using Sparrows.Bot.Models;
using Discord;
using Microsoft.Extensions.Configuration;

namespace Sparrows.Bot.Commands {
   
    [Group("order", "Order food")]
    public class OrderCommand : InteractionModuleBase<SocketInteractionContext> {
        const string NO_PERMISSION_STRING = "https://cdn.discordapp.com/attachments/1060328084252934197/1169709376509067274/IMG_8236.png?ex=655663bf&is=6543eebf&hm=5eb452573eb9a804d0b7c6d1471e2dd763602c285f5ce710adb8df844c916fb1&";

        public OrderCommand(IOrderService orderService, IUserService userService, IConfiguration config) {
            m_OrderService = orderService;
            m_UserService = userService;
            m_AdminUsers = new List<ulong>();

            var adminUsers = config.GetRequiredSection("admin_users").GetChildren().ToArray();
            foreach(var user in adminUsers) {
                m_AdminUsers.Add(ulong.Parse(user.Value));
            }
        }

        [SlashCommand("food_lock", "Sets the lock on the orders", true)]
        public async Task FoodLock(bool isLocked) {
            if(!m_AdminUsers.Contains(Context.User.Id)) {
                await RespondAsync(NO_PERMISSION_STRING);
                return;
            }

            if(isLocked) {
                await m_OrderService.LockOrdering();
                await RespondAsync("Food ordering is now locked!");
            } else {
                await m_OrderService.UnlockOrdering();
                await RespondAsync("Food ordering is now unlocked!");
            }
        }

        [SlashCommand("food_now", "Opens up the orders", true)]
        public async Task FoodNow(string restaurantName, string url, string startTime, string endTimer) {
            if(!m_AdminUsers.Contains(Context.User.Id)) {
                await RespondAsync(NO_PERMISSION_STRING);
                return;
            }

            await m_OrderService.DeleteAllOrders();
            await m_OrderService.UnlockOrdering();

            if(!url.StartsWith("http")) {
                url = "https://" + url;
            }

            var embed = new EmbedBuilder {
                Title = "Essens Bestelling | Food Order 🍕🍴",
                Color = Color.Blue
            };

            embed.AddField("German", $"Hi, wir bestellem ab **{startTime}** bis **{endTimer}** bei **{restaurantName}**. Ihr könnt euch [hier]({url}) das Menü anschauen. Bitte nutzt ``/order add <Anzahl> <Gericht Nummer> <Gereicht Name> <Stück Preis>`` um Essen zu bestellen. Mehr infos könnt ihr euch mit ``/help`` anzeigen lassen.");
            embed.AddField("English", $"Hi, we are ordering food from **{startTime}** until **{endTimer}** at **{restaurantName}**. You can check out the menu [here]({url}). Please use ``/order add <Amount> <Dish Number> <Dish Name> <Item Price>`` to order food. You can view more infos with ``/help``.");
            embed.WithUrl(url);
            embed.WithImageUrl("https://upload.wikimedia.org/wikipedia/commons/a/a3/Eq_it-na_pizza-margherita_sep2005_sml.jpg");
            embed.WithCurrentTimestamp();
        
            await RespondAsync(embed: embed.Build());    
        }

        [SlashCommand("food_done", "Closes the orders", true)]
        public async Task FoodDone() {
            if(!m_AdminUsers.Contains(Context.User.Id)) {
                await RespondAsync(NO_PERMISSION_STRING);
                return;
            }

            await RespondAsync("Processing Orders... You will receive a DM shortly.");

            await m_OrderService.LockOrdering();

            var orders = await m_OrderService.GetAllOrders();
            
            var fileStream = new MemoryStream();
            var writer = new StreamWriter(fileStream);

            writer.WriteLine("Discord User;Vorname;Nachname;Anzahl;Nummer;Gericht;Einzel Preis;Gesamt;PayPal");

            decimal total = 0;
            ulong lastId = orders[0].DiscordUserId;
            foreach(var order in orders) {
                User user = await m_UserService.Get(order.DiscordUserId);
                
                if(lastId != user.DiscordId) {
                    writer.WriteLine($"TOTAL:;;;;;;;{total};");
                    writer.WriteLine();
                    total = 0;
                }

                writer.WriteLine($"{user.DiscordName};{user.FirstName};{user.LastName};{order.Amount};{order.DishNumber};{order.DishName};{order.ItemPrice};{order.ItemPrice * order.Amount};{user.PayPal}");

                lastId = user.DiscordId;
                total += order.ItemPrice * order.Amount;
            }

            writer.WriteLine($"TOTAL:;;;;;;;{total};");
            writer.Flush();

            FileAttachment attachment = new FileAttachment(fileStream, "Order.csv");

            await Context.User.CreateDMChannelAsync();
            await Context.User.SendFileAsync(attachment);

            await FollowupAsync($"Orders are now closed - Check your DMs {Context.User.Mention}");
        }

        [SlashCommand("add", "Add a dish to your order")]
        public async Task Add(int amount, string dishNumber, string dishName, decimal itemPrice) {
            if(await m_OrderService.IsOrderingLocked()) {
                await RespondAsync("Hi, we are currently not ordering any food. Please wait until the next announcement to order food.");
                return;
            }

            bool isUserRegistered = await m_UserService.Exists(Context.User.Id);
            if(!isUserRegistered) {
                await RespondAsync($"Please register using /register first before ordering food {Context.User.Mention}");
                return; 
            }

            await m_OrderService.AddOrder(Context.User.Id, new Order {
                DiscordUserId = Context.User.Id,
                DishNumber = dishNumber,
                DishName = dishName,
                Amount = amount,
                ItemPrice = itemPrice
            });

            await RespondAsync($"{amount}x {dishName} ({dishNumber}) {itemPrice}€ - {itemPrice * amount}€ has been added to your order");
        }

        [SlashCommand("remove", "Remove a dish from your order")]
        public async Task Remove(int number) {
            if(await m_OrderService.IsOrderingLocked()) {
                await RespondAsync("Hi, we are currently not ordering any food. Please wait until the next announcement to order food.");
                return;
            }

            var success = await m_OrderService.RemoveOrder(Context.User.Id, number);   

            if(!success) {
                await RespondAsync("Invalid Index");
                return;
            } 

            await RespondAsync("Your Order has been removed!");
        }

        [SlashCommand("list", "List your current orders")]
        public async Task List() {
            if(await m_OrderService.IsOrderingLocked()) {
                await RespondAsync("Hi, we are currently not ordering any food. Please wait until the next announcement to order food.");
                return;
            }
            
            var orders = await m_OrderService.GetOrders(Context.User.Id);

            if(orders.Count == 0) {
                await RespondAsync("You don't have any orders currently");
                return;
            }

            string desc = "";
            decimal total = 0;
            for(int i = 0; i < orders.Count; i++) {
                desc += $"**{i+1}**. {orders[i].Amount}x {orders[i].DishName} ({orders[i].DishNumber}) {orders[i].ItemPrice}€ : {orders[i].ItemPrice * orders[i].Amount}€\n";
                total += orders[i].ItemPrice * orders[i].Amount;
            }

            var embed = new EmbedBuilder {
                Title="Your Orders",
                Color=Color.Blue,
                Description=desc
            };

            embed.AddField("Total", $"{total}€", true);
            embed.WithCurrentTimestamp();
            embed.WithAuthor(Context.User);

            await RespondAsync(embed: embed.Build());
        }

        private IOrderService m_OrderService;
        private IUserService m_UserService;
        private List<ulong> m_AdminUsers;
    }
}