using Discord.Interactions;
using Sparrows.Bot.Services;
using Sparrows.Bot.Models;
using Discord;

namespace Sparrows.Bot.Commands {
    public class HelpCommand : InteractionModuleBase<SocketInteractionContext> {
        [SlashCommand("sex","sex")]
        public async Task Sex() {
            await RespondAsync($"<@312254859808342016> 👀");
        }

        [SlashCommand("help","get some help")]
        public async Task Help() {
            var embed = new EmbedBuilder {
                Title = "Help | Hilfe ❓",
                Color = Color.Blue
            };

            embed.AddField("Deutsch", "Um eine Bestellung aufzugeben musst Du dich registrieren. \n Das machst du mit ``/register``. \n ``/order add``: fügt Gerichte zu deiner Bestellung hinzu \n ``/order list``: listet alle Teile deiner Bestellung auf \n ``/order remove``: entfernt den Teil der Bestellung am gegebenen Index \n ``/unregister``: entfernt alle Deine Daten im Bot");
            embed.AddField("English", "To place an order you have to register. \n You do that with ``/register``. \n ``/order add``: adds dishes to your order \n ``/order list``: lists all parts of your order \n ``/order remove``: removes the part of the order at the given index \n ``/unregister``: removes all your data in the bot");

            embed.WithCurrentTimestamp();
        
            await RespondAsync(embed: embed.Build());
        }

    }
}