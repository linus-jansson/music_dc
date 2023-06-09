﻿using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;

public class Program
{
    private DiscordSocketClient _client = new();
    private ulong OwnerGuildId = 583235725948878858;

    public static Task Main(string[] args) => new Program().MainAsync();

    public async Task MainAsync()
    {

        var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var token = File.ReadAllText(path+"/.dc_token");
       
        _client.Log += Log;

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
        _client.Ready += isReady;

        // _client.Ready += 
        _client.SlashCommandExecuted += SlashCommandHandler;

        // Block this task until the program is closed.
        await Task.Delay(-1);
    }

    public async Task isReady()
    {
        // Let's build a guild command! We're going to need a guild so lets just put that in a variable.
        var guild = _client.GetGuild(OwnerGuildId);

        // Next, lets create our slash command builder. This is like the embed builder but for slash commands.
        var guildCommand = new SlashCommandBuilder();

        // Note: Names have to be all lowercase and match the regular expression ^[\w-]{3,32}$
        guildCommand.WithName("first-command");

        // Descriptions can have a max length of 100.
        guildCommand.WithDescription("This is my first guild slash command!");

        // Let's do our global command
        var globalCommand = new SlashCommandBuilder();
        globalCommand.WithName("first-global-command");
        globalCommand.WithDescription("This is my first global slash command");

        var pingCommand = new SlashCommandBuilder();

        pingCommand.WithName("ping");
        pingCommand.WithDescription("Ping the bot");


        try
        {
            // Now that we have our builder, we can call the CreateApplicationCommandAsync method to make our slash command.
            await guild.CreateApplicationCommandAsync(guildCommand.Build());
            await guild.CreateApplicationCommandAsync(pingCommand.Build());

            // With global commands we don't need the guild.
            await _client.CreateGlobalApplicationCommandAsync(globalCommand.Build());
            // Using the ready event is a simple implementation for the sake of the example. Suitable for testing and development.
            // For a production bot, it is recommended to only run the CreateGlobalApplicationCommandAsync() once for each command.
        }
        catch (HttpException exception)
        {
            // If our command was invalid, we should catch an ApplicationCommandException. This exception contains the path of the error as well as the error message. You can serialize the Error field in the exception to get a visual of where your error is.
            var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);

            // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
            Console.WriteLine(json);
        }
    }

    private async Task SlashCommandHandler(SocketSlashCommand Command)
    {
        switch (Command.Data.Name)
        {
            case "ping":
                await PingCommand(Command);
                break;
            default:
                await Command.RespondAsync($"You executed {Command.Data.Name}");
                break;
        }

    }

    private async Task PingCommand(SocketSlashCommand Command)
    {
        await Command.RespondAsync("Pong " + Command.User.Username);
    }


    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }
}