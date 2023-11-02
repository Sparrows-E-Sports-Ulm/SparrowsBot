using Discord.Interactions;
using Sparrows.Bot.Services;

namespace Sparrows.Bot.Commands {
    public class RegisterCommand : InteractionModuleBase<SocketInteractionContext> {
        public RegisterCommand(IUserService userService) {
            m_UserService = userService;
        }
        
        [SlashCommand("register", "Register necessary data about your self")]
        public async Task Register(string firstName, string lastName, string paypalEmail) {
            bool isUserRegistered = await m_UserService.Exists(Context.User.Id);
            
            if(isUserRegistered) {
                await RespondAsync("You are already registered!");
                return;
            }

            await m_UserService.Add(new Models.User {
                FirstName = firstName,
                LastName = lastName,
                DiscordName = Context.User.GlobalName,
                DiscordId = Context.User.Id,
                PayPal = paypalEmail
            });

            await RespondAsync("Success! You are now registered!");
        }


        [SlashCommand("unregister", "Deletes all your data from the bot")]
        public async Task Unregister() {
            bool isDeleted = await m_UserService.Delete(Context.User.Id);

            if(!isDeleted) {
                await RespondAsync("You are not registered");
                return;
            }

            await RespondAsync("Success! You have been unregistered!");
        }

        private IUserService m_UserService;
    }
}