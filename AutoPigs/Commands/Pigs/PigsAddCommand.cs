using BulbulatorLocalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using СrossAppBot;
using СrossAppBot.Commands;
using СrossAppBot.Entities;

namespace AutoPigs.Commands.Pigs
{
    public class PigsAddCommand : AbstractCommand
    {
        [CommandArgument("User", null)]
        public ChatUser Target { get; set; }
        [CommandArgument("Category", null, optional: true)]
        public Category Category { get; set; }
        public PigsAddCommand() : base("add", "COMMANDS_PIGS_ADD_DESCRIPTION") { }

        public override async Task Execute(CommandContext context = null)
        {
            ChatUser sender = context.Sender;
            AbstractBotClient client = context.Client;
            ChatGuild guild = context.Guild;
            string result;
            bool success = false;

            string languageCode = null;
            Localizer localizer = null;

            try
            {
                DatabaseHandler databaseHandler = AutoPigs.DatabaseHandler;
                localizer = AutoPigs.Localizer;
                languageCode = databaseHandler.GetGuildConfig(guild).Language;


                if (Target == null)
                {
                    result = "COMMANDS_ERROR_USER_NOT_FOUND";
                }
                else if
                    (!(
                        sender.IsAdmin & !Target.IsOwner
                        || sender.IsOwner
                    ))
                {
                    result = "COMMANDS_ERROR_NOT_ENOUGH_RIGHTS";
                }
                else if (databaseHandler.UserIsPig(sender, guild))
                {
                    result = "COMMANDS_PIGS_ADD_FAIL_SENDER_IS_PIG";
                }
                else if (databaseHandler.UserIsPig(Target, guild))
                {
                    result = "COMMANDS_PIGS_ADD_FAIL_TARGET_IS_PIG";
                }
                else if (Target.Id.Equals(sender.Id))
                {
                    result = "COMMANDS_PIGS_ADD_FAIL_TARGET_IS_SENDER";
                }
                else 
                {
                    if (Category == null) 
                    {
                        Category = databaseHandler.GetDefaultCategory(guild);
                    }

                    Pig pig = new Pig(Target, guild);
                    databaseHandler.AddPig(pig);
                    databaseHandler.SetPigCategory(pig, Category);
                    result = "COMMANDS_PIGS_ADD_SUCCESS";
                    success = true;
                }

            }
            catch (Exception exception)
            {
                Console.WriteLine($"An error occurred while executing the command '{Name}': {exception.ToString()}\n{exception.Message}");
                result = "COMMANDS_ERROR_UNKNOWN_ERROR";
            }

            if (localizer != null)
            {
                result = localizer.GetLocalizedString(languageCode, result);
            }
            if (success)
            {
                result = $"{client.Mention(Target)} {result}";
            }
            await client.SendMessageAsync(context.Channel.Id, result);
        }
    }
}
