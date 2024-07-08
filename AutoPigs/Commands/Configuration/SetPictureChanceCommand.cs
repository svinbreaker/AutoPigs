using BulbulatorLocalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using СrossAppBot.Commands;
using СrossAppBot;
using СrossAppBot.Entities;
using AutoPigs.Commands.Conditions;

namespace AutoPigs.Commands.Configuration
{
    public class SetPictureChanceCommand : AbstractCommand
    {
        [CommandArgument("chance (%)", null)]
        public int? Chance { get; set; }
        [CommandArgument("category", null, optional: true)]
        public Category Category { get; set; }
        public SetPictureChanceCommand() : base("pictureChance", "COMMANDS_CONFIGURATION_PICTURE_CHANCE_DESCRIPTION") { }

        public override void Conditions()
        {
            Condition(new SenderIsNotPigCommandCondition(Context));
            Condition(new AdminRightsCommandCondition(Context));
            Condition(new CategoryExistOrEmptyCommandCondition(Context, Category?.Name ?? null));
        }

        protected async override Task Executee()
        {
            AbstractBotClient client = Context.Client;
            ChatGroup guild = Context.ChatGroup;
            DatabaseHandler databaseHandler = AutoPigs.DatabaseHandler;
            Localizer localizer = AutoPigs.Localizer;
            string languageCode = await databaseHandler.GetGuildLanguage(guild);
            string result;

            try
            {
                if (Chance == null)
                {
                    result = "COMMANDS_CONFIGURATION_REACTION_CHANCE_ERROR";
                }
                else if (Chance < 0 || Chance > 100)
                {
                    result = "COMMANDS_CONFIGURATION_REACTION_CHANCE_ERROR";
                }
                else
                {
                    if (Category == null)
                    {
                        Category = await databaseHandler.GetDefaultCategory(guild);
                    }
                    CategoryConfig config = await databaseHandler.GetCategoryConfig(Category);
                    config.PictureChance = Chance.Value;
                    await databaseHandler.Database.UpdateAsync(config);

                    result = "COMMANDS_CONFIGURATION_PICTURE_CHANCE_SUCCESS";
                }
            } catch(Exception exception) 
            {
                Console.WriteLine($"An error occurred while executing the command '{Name}': {exception.ToString()}\n{exception.Message}");
                result = "COMMANDS_ERROR_UNKNOWN_ERROR";
            }
            await client.SendMessageAsync(Context.Channel.Id, text: localizer.GetLocalizedString(languageCode, result));
        }

    }
}
