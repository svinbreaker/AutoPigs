using BulbulatorLocalization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using СrossAppBot;
using СrossAppBot.Commands;
using СrossAppBot.Entities;

namespace AutoPigs.Commands.Pigs.Categories
{
    public class CategoriesListCommand : AbstractCommand
    {
        public CategoriesListCommand() : base("categories", "COMMANDS_PIGS_CATEGORIES_LIST_DESCRIPTION") { }

        public override async Task Execute(CommandContext context = null)
        {
            AbstractBotClient client = context.Client;
            ChatGuild guild = context.Guild;
            string result;

            string languageCode;
            Localizer localizer;

            try
            {
                DatabaseHandler databaseHandler = AutoPigs.DatabaseHandler;
                localizer = AutoPigs.Localizer;
                languageCode = await databaseHandler.GetGuildLanguage(guild);

                List<Category> categories = await databaseHandler.GetGuildCategories(guild);
                if (categories.Count == 0)
                {
                    result = localizer.GetLocalizedString(languageCode, "COMMANDS_PIGS_CATEGORIES_LIST_EMPTY");
                }
                else
                {
                    StringBuilder builder = new StringBuilder();
                    builder.Append($"{localizer.GetLocalizedString(languageCode, "COMMANDS_PIGS_CATEGORIES_LIST_SUCCESS")}\n");
                    foreach (Category category in categories)
                    {
                        builder.Append(category.Name).Append(" (").Append((await databaseHandler.GetPigsOfCategory(category)).Count).Append(")\n");
                    }
                    result = builder.ToString();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"An error occurred while executing the command '{Name}': {exception.ToString()}\n{exception.Message}");
                result = "COMMANDS_ERROR_UNKNOWN_ERROR";
            }

            await client.SendMessageAsync(context.Channel.Id, result);
        }
    }
}