using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using DungeonRPG.Data;

namespace DungeonRPG.Modules.Character
{
    // I mean it's shitcode but it works, and it somehow works pretty good
    public class Start : InteractiveBase<SocketCommandContext>
    {
        [Command("begin", RunMode = RunMode.Async), Alias("start", "create")]
        public async Task BeginAsync()
        {
            if (Context.Channel.Id != 701318935210885140)
                return;
            
            Regex rx = new Regex(@"/[.*+?^${}()|[\]\\]/g");
            
            EmbedBuilder embed = new EmbedBuilder();
            EmbedBuilder classEmbed = new EmbedBuilder();
            EmbedBuilder createdEmbed = new EmbedBuilder();

            if (CharacterService.GetifExists(Context.User.Id).Result)
            {
                embed.WithTitle("Error!");
                embed.WithDescription($"You already have a character {Context.User.Mention}");
                embed.WithCurrentTimestamp();
                embed.WithColor(Color.Red);
                
                await ReplyAndDeleteAsync("", false, embed.Build(), TimeSpan.FromSeconds(5));
                await Context.Message.DeleteAsync();
            }
            else
            {
                embed.WithTitle("Welcome");
                embed.WithDescription("Please enter a name for your character");
                embed.WithCurrentTimestamp();
                embed.WithColor(Color.Green);

                var success = await ReplyAsync("", false, embed.Build());
                var name = await NextMessageAsync(true, true, TimeSpan.FromMinutes(1));

                MatchCollection matches = rx.Matches(name.ToString());
                if (matches.Count >= 1)
                {
                    await ReplyAndDeleteAsync("Your character name contains invalid characters", false, null, TimeSpan.FromSeconds(10));
                    await Task.Delay(TimeSpan.FromSeconds(10));
                    await name.DeleteAsync();
                    await Context.Message.DeleteAsync();
                    await success.DeleteAsync();
                }
                else if (name.Content.Length >= 30 || name.Content.Length < 3)
                {
                    await ReplyAndDeleteAsync("Your character is too short or too long, character names must be between 3 and 30 characters", false, null, TimeSpan.FromSeconds(10));
                    await Task.Delay(TimeSpan.FromSeconds(10));
                    await name.DeleteAsync();
                    await Context.Message.DeleteAsync();
                    await success.DeleteAsync();
                }
                else
                {
                    IEmote archer = new Emoji("🏹");
                    IEmote mage = new Emoji("🎆");
                    IEmote thief = new Emoji("💰");

                    Classes selectedClass = Classes.Townie;
                    
                    classEmbed.WithTitle("Choose your class");
                    classEmbed.WithDescription("Please choose your class\n\n🏹 Archer\n🎆 Mage\n💰 Thief");
                    classEmbed.WithColor(Color.Orange);

                    var classMessage = await ReplyAsync("", false, classEmbed.Build());
                    await classMessage.AddReactionsAsync(new []{archer, mage, thief});

                    Context.Client.ReactionAdded += ClassHandler;
                    
                    async Task ClassHandler(Cacheable<IUserMessage, ulong> msg, ISocketMessageChannel channel, SocketReaction reaction)
                    {
                        if (reaction.User.Value.IsBot || reaction.User.Value.Id != Context.Message.Author.Id || reaction.MessageId != classMessage.Id)
                            return;
                        
                        if (reaction.Emote.Equals(archer))
                            selectedClass = Classes.Archer;
                        else if(reaction.Emote.Equals(mage))
                            selectedClass = Classes.Mage;
                        else if (reaction.Emote.Equals(thief))
                            selectedClass = Classes.Thief;

                        Context.Client.ReactionAdded -= ClassHandler;
                        await CharacterService.CreateCharacterAsync(name.ToString(), Context.Message.Author.Id, selectedClass);
                        createdEmbed.WithTitle("Successfully created a character for you").WithColor(Color.Green).WithDescription("A character was successfully created for you");
                        await Context.Message.Author.SendMessageAsync("", false, createdEmbed.Build());
                        await name.DeleteAsync();
                        await Context.Message.DeleteAsync();
                        await classMessage.DeleteAsync();
                        await success.DeleteAsync();
                    }
                }
            }
        }
    }
}