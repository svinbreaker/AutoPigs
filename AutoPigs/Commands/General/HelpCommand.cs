using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using СrossAppBot.Commands;
using СrossAppBot;
using System.Reflection.Metadata;
using System.Reflection;
using BulbulatorLocalization;
using СrossAppBot.Entities;

namespace AutoPigs.Commands.General
{
    public class HelpCommand : AbstractCommand
    {
        public ChatUser Target { get; set; }

        public HelpCommand() : base("help", "COMMANDS_GENERAL_HELP_DESCRIPTION") { }

        public override async Task Execute(CommandContext context = null)
        {

            ChatUser sender = context.Sender;
            AbstractBotClient client = context.Client;
            ChatGuild guild = context.Guild;
            string result;
            ChatUser target = Target;

            string languageCode;
            Localizer localizer;
            try
            {
                DatabaseHandler databaseHandler = AutoPigs.DatabaseHandler;
                localizer = AutoPigs.Localizer;
                languageCode = await databaseHandler.GetGuildLanguage(guild);

                string prefix = client.TextCommandProcessor.Prefix;
                StringBuilder builder = new StringBuilder();
                builder.Append($"{localizer.GetLocalizedString(languageCode, "COMMANDS_GENERAL_HELP_SUCCESS")}\n");
                Dictionary<string, AbstractCommand> commands = client.TextCommandProcessor.commands;
                foreach (KeyValuePair<string, AbstractCommand> args in commands)
                {
                    AbstractCommand command = args.Value;
                    builder.Append($"{prefix}{command.Name}");
                    foreach (PropertyInfo property in command.GetType().GetProperties().Where(property => property.IsDefined(typeof(CommandArgumentAttribute), false)).ToList())
                    {
                        builder.Append($"({property.GetCustomAttribute<CommandArgumentAttribute>().Name}) ");
                    }
                    builder.Append($" - {localizer.GetLocalizedString(languageCode, command.Description)}\n");
                }
                result = builder.ToString();
            }
            catch (Exception exception)
            {
                Console.WriteLine($"An error occurred while executing the command '{Name}': {exception.ToString()}\n{exception.Message}");
                result = "Unknown error";
            }

            await client.SendMessageAsync(context.Channel.Id, result);
        }
    }
}