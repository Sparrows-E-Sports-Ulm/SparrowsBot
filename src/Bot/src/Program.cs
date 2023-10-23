using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace Sparrows.Bot {
    class Program {
        public static Task Main(string[] args) => new Program().MainAsync();

        public async Task MainAsync() {
            var configBuilder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config"))
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.dev.json", optional: true);
            IConfigurationRoot config = configBuilder.Build();
            
            m_Client = new DiscordSocketClient();
            m_Client.Log += Log;

            var token = config.GetSection("token").Value;

            await m_Client.LoginAsync(TokenType.Bot, token);
            await m_Client.StartAsync();

            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg) {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }


        private DiscordSocketClient m_Client;
    }
}