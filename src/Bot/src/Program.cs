using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sparrows.Bot.Services;

namespace Sparrows.Bot {
    class Program {
        public Program() {
            m_Config = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config"))
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.dev.json", optional: true)
            .Build();
        }

        public static Task Main(string[] args) => new Program().MainAsync();

        public async Task MainAsync() {
           
            using(var services = ConfigureServices()) {
                var client = services.GetRequiredService<DiscordSocketClient>();
                var commandHandler = services.GetRequiredService<CommandHandlerService>();
                m_InteractionService = services.GetRequiredService<InteractionService>();

                client.Log += OnLog;
                client.Ready += OnReady;
                m_InteractionService.Log += OnLog;

                await client.LoginAsync(TokenType.Bot, m_Config["token"]);
                await client.StartAsync();

                await commandHandler.InitializeAsync();

                await Task.Delay(Timeout.Infinite);
            }
        }

        private Task OnLog(LogMessage msg) {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private async Task OnReady() {
            #if DEBUG
                await m_InteractionService.RegisterCommandsToGuildAsync(ulong.Parse(m_Config["test_guild_id"]));
            #else
                await m_InteractionService.RegisterCommandsGloballyAsync();
            #endif
        }

        private ServiceProvider ConfigureServices() {
            return new ServiceCollection()
            .AddSingleton(m_Config)
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton<InteractionService>()
            .AddSingleton<CommandHandlerService>()
            .BuildServiceProvider();
        }


        private readonly IConfiguration m_Config;
        private InteractionService m_InteractionService;
    }
}