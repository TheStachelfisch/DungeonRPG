using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace DungeonRPG.Handler
{
    public class CommandHandler
    {
        public static CommandService _commands;
        private readonly DiscordSocketClient _client;
        private CommandServiceConfig _config;
        private IServiceProvider _service;

        public CommandHandler(DiscordSocketClient client, CommandServiceConfig config)
        {
            _client = client;
            _config = config;

            _commands = new CommandService(_config);
            
            _service = new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton<InteractiveService>()
                .BuildServiceProvider();
            
            _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _service);

            _client.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var msg = arg as SocketUserMessage;
            if (msg == null) return;

            var context = new SocketCommandContext(_client, msg);

            var argPos = 0;
            if (msg.HasStringPrefix(".", ref argPos) || msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var result = await _commands.ExecuteAsync(context, argPos, _service);
                
                if (!result.IsSuccess)
                {
                    Console.WriteLine("Error happened while executing Command: " + result.ErrorReason + " ServerId: " + context.Guild.Id);
                }
            }
        }
    }
}