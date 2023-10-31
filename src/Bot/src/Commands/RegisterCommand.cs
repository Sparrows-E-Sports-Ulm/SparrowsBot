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

        private IUserService m_UserService;
    }
}