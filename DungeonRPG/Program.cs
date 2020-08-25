﻿using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DungeonRPG.Handler;

namespace DungeonRPG
{
    class Program
    {
        private static readonly string _token = File.ReadAllText(@"token.txt");

        private static DiscordSocketClient _client;
        private static DiscordSocketConfig _config;
        private static CommandServiceConfig _commandConfig;

        private static CommandHandler _commandHandler;

        static void Main(string[] args)
            => StartBotAsync().GetAwaiter().GetResult();

        public static async Task StartBotAsync()
        {
            _config = new DiscordSocketConfig();
            _config.MessageCacheSize = 250;
            _config.ExclusiveBulkDelete = true;
            
            _client = new DiscordSocketClient(_config);
            _commandConfig = new CommandServiceConfig();

            await _client.LoginAsync(TokenType.Bot, _token);

            await _client.StartAsync();
            await _client.SetStatusAsync(UserStatus.Online);
            await _client.SetGameAsync(".help");

            _client.Log += Log;
            _client.Ready += ReadyEvent;

            await Task.Delay(-1);
        }

        //Add Handlers here
        private static async Task ReadyEvent()
        {
            await _client.SetStatusAsync(UserStatus.Online);
            _commandHandler = new CommandHandler(_client, _commandConfig);
        }

        private static Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }
    }
}