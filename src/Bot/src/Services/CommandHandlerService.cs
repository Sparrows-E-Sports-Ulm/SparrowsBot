using System.Configuration.Assemblies;
using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Sparrows.Bot.Services {
    public class CommandHandlerService {
        public CommandHandlerService(DiscordSocketClient client, InteractionService commands, IServiceProvider services) {
            m_Client = client;
            m_Commands = commands;
            m_Services = services;
        }

        public async Task InitializeAsync() {
            await m_Commands.AddModulesAsync(Assembly.GetEntryAssembly(), m_Services);

            m_Client.InteractionCreated += HandleInteraction;
            m_Commands.SlashCommandExecuted += SlashCommandExecuted;
        }

        private Task SlashCommandExecuted(SlashCommandInfo arg1, Discord.IInteractionContext arg2, IResult arg3) {
            if (!arg3.IsSuccess)
            {
                switch (arg3.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        // implement
                        break;
                    case InteractionCommandError.UnknownCommand:
                        // implement
                        break;
                    case InteractionCommandError.BadArgs:
                        // implement
                        break;
                    case InteractionCommandError.Exception:
                        // implement
                        break;
                    case InteractionCommandError.Unsuccessful:
                        // implement
                        break;
                    default:
                        break;
                }
            }

            return Task.CompletedTask;
        }

        private async Task HandleInteraction(SocketInteraction interaction) {
            try {
                var context = new SocketInteractionContext(m_Client, interaction);
                await m_Commands.ExecuteCommandAsync(context, m_Services);
            } catch (Exception ex) {
                Console.WriteLine(ex);

                if(interaction.Type == InteractionType.ApplicationCommand) {
                    await interaction.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
                }                
            }
        }

        private readonly DiscordSocketClient m_Client;
        private readonly InteractionService m_Commands;
        private readonly IServiceProvider m_Services;
    }
}