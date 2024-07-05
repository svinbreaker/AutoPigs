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
using AutoPigs.Commands.Conditions;

namespace AutoPigs.Commands.Pigs.Categories
{
    public class ClearBattlePicturesCommand : AbstractCommand
    {
        [CommandArgument("Category", null)]
        public Category Category { get; set; }
        public ClearBattlePicturesCommand() : base("clearBattlePictures", "COMMANDS_PIGS_CATEGORIES_CLEAR_BATTLE_PICTURES_DESCRIPTION") { }

        public override void Conditions()
        {
            Condition(new SenderIsNotPigCommandCondition(Context));
            Condition(new AdminRightsCommandCondition(Context));
            Condition(new CategoryExistOrEmptyCommandCondition(Context, Category?.Name ?? null));
        }

        protected override async Task Executee()
        {
            string result;
            DatabaseHandler databaseHandler = AutoPigs.DatabaseHandler;
            Localizer localizer = AutoPigs.Localizer;
            ChatGroup guild = Context.ChatGroup;
            string languageCode = await databaseHandler.GetGuildLanguage(guild);

            try
            {
                if (Category == null)
                {
                    Category = await databaseHandler.GetDefaultCategory(guild);
                }
                await databaseHandler.ClearBattlePictures(Category);
                result = "COMMANDS_PIGS_CATEGORIES_CLEAR_BATTLE_PICTURES_SUCCESS";
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message + "\n" + exception.ToString());
                result = "COMMANDS_ERROR_UNKNOWN_ERROR";
            }

            await guild.Client.SendMessageAsync(Context.Channel.Id, result);
        }

    }
}

