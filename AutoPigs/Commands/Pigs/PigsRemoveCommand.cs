using BulbulatorLocalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using СrossAppBot.Entities;
using СrossAppBot.Commands;
using СrossAppBot;

namespace AutoPigs.Commands.Pigs
{
    class PigsRemoveCommand : AbstractCommand
    {
        [CommandArgument("user", null)]
        public ChatUser Target { get; set; }
        public PigsRemoveCommand() : base("remove", "COMMANDS_PIGS_REMOVE_DESCRIPTION") { }

        public override async Task Execute(CommandContext context = null)
        {

            ChatUser sender = context.Sender;
            AbstractBotClient client = context.Client;
            ChatGuild guild = context.Guild;
            string result;
            bool success = false;

            Localizer localizer = null;
            string languageCode = null;
            try
            {
                DatabaseHandler databaseHandler = AutoPigs.DatabaseHandler;
                localizer = AutoPigs.Localizer;
                languageCode = await databaseHandler.GetGuildLanguage(guild);

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
                else if (await databaseHandler.UserIsPig(sender, guild))
                {
                    result = "COMMANDS_PIGS_REMOVE_SENDER_IS_PIG";
                }
                else if (!(await databaseHandler.UserIsPig(Target, guild)))
                {
                    result = "COMMANDS_PIGS_TARGET_IS_NOT_PIG";
                }
                else if (Target.Id.Equals(sender.Id))
                {
                    result = "COMMANDS_PIGS_REMOVE_TARGET_IS_SENDER";
                }
                else
                {
                    await databaseHandler.RemovePig(await databaseHandler.GetUserAsPig(Target, guild));
                    result = "COMMANDS_PIGS_REMOVE_SUCCESS";
                    success = true;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"An error occurred while executing the command '{Name}': {exception.ToString()}\n{exception.Message}");
                result = "COMMANDS_ERROR_UNKNOWN_ERROR";
            }
            
            await client.SendMessageAsync(context.Channel.Id, localizer.GetLocalizedString(languageCode, result));
        }
    }
}

