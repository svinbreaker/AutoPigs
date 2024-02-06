using BulbulatorLocalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using СrossAppBot.Commands;
using СrossAppBot;
using СrossAppBot.Entities;

namespace AutoPigs.Commands.Pigs.Categories
{
    public class SetPigCategoryCommand : AbstractCommand
    {
        [CommandArgument("Pig", null)]
        public ChatUser Target { get; set; }
        [CommandArgument("Category", null, optional: true)]
        public Category Category { get; set; }
        public SetPigCategoryCommand() : base("setCategory", "COMMANDS_PIGS_CATEGORIES_SET_PIG_CATEGORY_DESCRIPTION") { }

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
                    result = "COMMANDS_PIGS_ADD_FAIL_SENDER_IS_PIG";
                }
                else if (!(await databaseHandler.UserIsPig(Target, guild)))
                {
                    result = "COMMANDS_PIGS_REMOVE_FAIL_TARGET_IS_NOT_PIG";
                }
                else if (Target.Id.Equals(sender.Id))
                {
                    result = "COMMANDS_PIGS_ADD_FAIL_TARGET_IS_SENDER";
                }              
                else if (Category == null)
                {
                    result = "COMMANDS_PIGS_CATEGORIES_ERROR_NOT_EXIST";
                }
                else if (await databaseHandler.PigHasCategory(await databaseHandler.GetUserAsPig(Target, guild), Category))
                {
                    result = "COMMANDS_PIGS_CATEGORIES_SET_PIG_CATEGORY_ERROR_ALREADY_SETTED";
                }
                else
                {
                    await databaseHandler.SetPigCategory(await databaseHandler.GetUserAsPig(Target, guild), Category);
                    result = "COMMANDS_PIGS_CATEGORIES_SET_PIG_CATEGORY_SUCCESS";
                    success = true;
                }

            }
            catch (Exception exception)
            {
                Console.WriteLine($"An error occurred while executing the command '{Name}': {exception.ToString()}\n{exception.Message}");
                result = "COMMANDS_ERROR_UNKNOWN_ERROR";
            }
            result = localizer.GetLocalizedString(languageCode, result);
            if (success) 
            {
                result += $" {Category.Name}";
            }

            await client.SendMessageAsync(context.Channel.Id, text: result);
        }
    }
}

