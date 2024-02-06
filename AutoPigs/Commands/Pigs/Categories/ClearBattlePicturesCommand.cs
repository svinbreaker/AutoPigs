using BulbulatorLocalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using СrossAppBot.Commands;
using СrossAppBot.Entities.Files;
using СrossAppBot;
using СrossAppBot.Entities;

namespace AutoPigs.Commands.Pigs.Categories
{
    public class ClearBattlePicturesCommand : AbstractCommand
    {
        [CommandArgument("Category", null)]
        public Category Category { get; }
        public ClearBattlePicturesCommand() : base("clearBattlePictures", "COMMANDS_PIGS_CATEGORIES_CLEAR_BATTLE_PICTURES_DESCRIPTION") { }

        public override async Task Execute(CommandContext context = null)
        {
            string result;
            DatabaseHandler databaseHandler = AutoPigs.DatabaseHandler;
            Localizer localizer = AutoPigs.Localizer;
            ChatGuild guild = context.Guild;
            string languageCode = await databaseHandler.GetGuildLanguage(guild);

            try
            {
                if (Category == null)
                {
                    result = "COMMANDS_PIGS_CATEGORIES_ERROR_NOT_EXIST";
                }
                else
                {
                    result = "COMMANDS_PIGS_CATEGORIES_CLEAR_BATTLE_PICTURES_SUCCESS";
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message + "\n" + exception.ToString());
                result = "COMMANDS_ERROR_UNKNOWN_ERROR";
            }

            await guild.Client.SendMessageAsync(context.Channel.Id, result);
        }

    }
}

