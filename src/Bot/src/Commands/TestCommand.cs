using Discord.Interactions;

namespace Sparrows.Bot.Commands {
   
    public class TestCommands : InteractionModuleBase<SocketInteractionContext> {
        public TestCommands () {}

        [SlashCommand("test", "Test the bot")]
        public async Task Test(string arg1, string arg2, string arg3) {
            await RespondAsync($"Arg 1 = {arg1} and Arg2= {arg2} arg3={arg3}");
        }

        [SlashCommand("8ball", "find your answer!")]
        public async Task EightBall(string question) {
            
            var replies = new List<string> {
                "yes",
                "no",
                "maybe",
                "hazzzzy...."
            };

            var answer = replies[new Random().Next(replies.Count - 1)];

            await RespondAsync($"You asked: [**{question}**], and your answer is: [**{answer}**]");
        }
    }
}