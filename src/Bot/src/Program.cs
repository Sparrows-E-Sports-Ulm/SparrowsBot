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
                m_Client = services.GetRequiredService<DiscordSocketClient>();
                var commandHandler = services.GetRequiredService<CommandHandlerService>();
                m_InteractionService = services.GetRequiredService<InteractionService>();
                
                m_Client.Log += OnLog;
                m_Client.Ready += OnReady;
                m_InteractionService.Log += OnLog;

                await commandHandler.InitializeAsync();

                await m_Client.LoginAsync(TokenType.Bot, m_Config["token"]);
                await m_Client.StartAsync();

                await Task.Delay(Timeout.Infinite);
            }
        }

        private Task OnLog(LogMessage msg) {
            Console.WriteLine("[ Discord ] " + msg.ToString());

            if(m_LogChannel != null) {
                m_LogChannel.SendMessageAsync(msg.ToString());
            }

            return Task.CompletedTask;
        }

        private async Task OnReady() {
            if(m_InteractionService == null) {
                throw new Exception("Interactions Service needs to be initilized");
            }

            #if DEBUG
                await m_InteractionService.RegisterCommandsToGuildAsync(ulong.Parse(m_Config["test_guild_id"]));
            #else
                await m_InteractionService.RegisterCommandsGloballyAsync();
            #endif

            ulong logChannelId = ulong.Parse(m_Config.GetRequiredSection("log_channel").Value);
            m_LogChannel = m_Client.GetChannel(logChannelId) as IMessageChannel;
            
        }

        private ServiceProvider ConfigureServices() {
            return new ServiceCollection()
            .AddSingleton(m_Config)
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton<InteractionService>()
            .AddSingleton<CommandHandlerService>()
            .AddSingleton<IUserService, MemStoreUserService>()
            .AddSingleton<IOrderService, MemStoreOrderService>()
            .BuildServiceProvider();
        }

        private readonly IConfiguration m_Config;
        private InteractionService? m_InteractionService;
        private DiscordSocketClient? m_Client;
        private IMessageChannel? m_LogChannel;
        
    }
}