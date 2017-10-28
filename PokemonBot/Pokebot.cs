using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.Net.Providers.WS4Net;
using Discord.WebSocket;

using PokemonBot.Modules;
using PokemonBot.Infos;

namespace PokemonBot
{
    public class Pokebot
    {
        #region Discord
        // private client member
        private DiscordSocketClient _client;
        // private commands member
        private CommandService _commands = new CommandService();
        // private service provider member to dependency injection
        private IServiceProvider _services;
        #endregion


        #region Constants
        // Admin ID
        private const ulong _adminID = 272493570798256140;
        // Bot token necessary to login bot
        private const string _token = "MzY4MTY2MDUwNTcyNzk1OTA0.DMGBGQ.AYXJzFbpTuTC0jd9Ey8i4Sa7yAQ";
        #endregion


        #region Public Methods
        public Pokebot()
        {
            // Creates the client and sets the message cache to 100  (100 messages that the bot can keep)
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                MessageCacheSize = 100,
                // WebSocket only necessary depending on the versions of .Netframework
                WebSocketProvider = WS4NetProvider.Instance,
            });

            // Logger events
            _client.Log += Logger;  
            _commands.Log += Logger;
        }

        public async Task MainAsync()
        {
            await _client.LoginAsync(TokenType.Bot, _token); // Bot login
            await _client.StartAsync(); // Starts the client

            _client.MessageReceived += HandleCommandsAsync; // Message Delegate / Event

            await DependencyInject(); // Call DependecyInject Method

            await Task.Delay(-1);   // Certifies that the task will continue running
        }
        #endregion


        #region Private Methods
        private static Task Logger(LogMessage message)
        {
            switch (message.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }

            Console.WriteLine($"{DateTime.Now} [{message.Severity}] [{message.Source}]: {message.Message} {message.Exception}");

            if (message.Message == "Ready")
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{DateTime.Now} [Info] [Debuggr]: Bot on");
            }

            Console.ResetColor();

            return Task.CompletedTask;
        }

        private async Task DependencyInject()
        {
            // Run all services and modules
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .AddSingleton(new UserModule())
                .AddSingleton(new HelpModule())
                .AddSingleton(new AdminModule())             
                .BuildServiceProvider();
        }

        private async Task HandleCommandsAsync(SocketMessage msg)
        {
            var message = msg as SocketUserMessage; // Certifies that the message is from a user/bot

            if (message.Author.Id == _client.CurrentUser.Id || message.Author.IsBot) return; // Certifies that the bot doesn't go reply itself

            var context = new SocketCommandContext(_client, message); // Creates the context

            int argPos = 0; // Argument position

            var result = await _commands.ExecuteAsync(context, argPos, _services); // Execute Async Service

            if (!result.IsSuccess) Console.WriteLine(result.ErrorReason);   // DEBUG: If error: Prints message

            if (message.ToString() == MessageList.Messages["gracinha"].Item1)
            {
                await context.Channel.SendMessageAsync(MessageList.Messages["gracinha"].Item2);
            }

            if (message.ToString() == MessageList.Messages["não"].Item1)
            {
                await context.Channel.SendMessageAsync(MessageList.Messages["não"].Item2);
            }
            Console.WriteLine(message); // DEBUG: Prints the command or message in the console
        }
        #endregion
    }
}