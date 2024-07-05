using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using СrossAppBot.Commands;
using СrossAppBot;
using BulbulatorLocalization;
using СrossAppBot.Entities;

namespace AutoPigs.Commands.Pigs
{
    public class PigsListCommand : AbstractCommand
    {
        public PigsListCommand() : base("list", "COMMANDS_PIGS_LIST_DESCRIPTION") { }

        protected override async Task Executee()
        {
            AbstractBotClient client = Context.Client;
            ChatGroup guild = Context.ChatGroup;
            string result;

            string languageCode;
            Localizer localizer;

            try
            {
                DatabaseHandler databaseHandler = AutoPigs.DatabaseHandler;
                localizer = AutoPigs.Localizer;
                languageCode = await databaseHandler.GetGuildLanguage(guild);

                List<Pig> pigs = await databaseHandler.GetPigs(guild.Id, client.Name);
                if (pigs.Count == 0)
                {
                    result = localizer.GetLocalizedString(languageCode, "COMMANDS_PIGS_LIST_EMPTY");
                }
                else
                {
                    StringBuilder builder = new StringBuilder();
                    builder.Append($"{localizer.GetLocalizedString(languageCode, "COMMANDS_PIGS_LIST_SUCCESS")}\n");
                    foreach (Pig pig in pigs)
                    {
                        builder.Append(client.Mention(pig.UserId)).Append("");                   
                        builder.Append(" (");
                        List<Category> categories = await databaseHandler.GetCategoriesOfPig(pig);
                        foreach(Category category in categories) 
                        {
                            builder.Append($"{category.Name}, ");
                        }
                        builder.Length -= 2;
                        builder.Append(")\n");
                    }
                    result = builder.ToString();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"An error occurred while executing the command '{Name}': {exception.ToString()}\n{exception.Message}");
                result = " COMMANDS_ERROR_UNKNOWN_ERROR";
            }

            await Context.AnswerAsync(result, null, true);
        }
    }
}

