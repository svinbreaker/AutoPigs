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
    public class DeleteCategoryCommand : AbstractCommand
    {
        [CommandArgument("category", null)]
        public Category Category { get; set; }
        public DeleteCategoryCommand() : base("deleteCategory", "COMMANDS_PIGS_CATEGORIES_DELETE_DESCRIPTION") { }

        public override async Task Execute(CommandContext context = null)
        {
            ChatGuild guild = context.Guild;
            AbstractBotClient client = context.Client;

            Localizer localizer = null;
            DatabaseHandler databaseHandler;
            string languageCode = null;

            string result;
            try
            {
                databaseHandler = AutoPigs.DatabaseHandler;
                localizer = AutoPigs.Localizer;
                languageCode = databaseHandler.GetGuildConfig(guild).Language;

                if (Category == null)
                {
                    result = "COMMANDS_PIGS_CATEGORIES_ERROR_NOT_EXIST";
                }
                else if (Category.Name == "Default") 
                {
                    result = "COMMANDS_PIGS_CATEGORIES_ERROR_DEFAULT";
                }
                else
                {
                    databaseHandler.RemoveGuildCategory(Category);
                    result = "COMMANDS_PIGS_CATEGORIES_DELETE_SUCCESS";
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