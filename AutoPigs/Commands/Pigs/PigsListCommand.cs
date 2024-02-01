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
                languageCode = databaseHandler.GetGuildConfig(guild).Language;

                List<Pig> pigs = databaseHandler.GetPigs(guild.Id, client.Name);
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
                        List<Category> categories = databaseHandler.GetCategoriesOfPig(pig);
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
                result = "ошибка";
            }

            await client.SendMessageAsync(context.Channel.Id, result);
        }
    }
}

