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

namespace AutoPigs.Commands.Pigs.Categories
{
    public class SetPigCategoryCommand : AbstractCommand
    {
        [CommandArgument("Pig", null)]
        public ChatUser Target { get; set; }
        [CommandArgument("Category", null, optional: true)]
        public Category Category { get; set; }

        public SetPigCategoryCommand() : base("setCategory", "COMMANDS_PIGS_CATEGORIES_SET_PIG_CATEGORY_DESCRIPTION") { }

        public override void Conditions() 
        {
            Condition(new SenderIsNotPigCommandCondition(Context));
            Condition(new HigherRightsCommandCondition(Context, Target));
            Condition(new TargetIsNotSenderCommandCondition(Context, Target));
            Condition(new TargetIsPigCommandCondition(Context, Target));
            Condition(new PigHasNotTheCategoryCommandCondition(Context, Pig.FromUser(Target, Context.ChatGroup), Category));
            Condition(new CategoryExistCommandCondition(Context, Category));
        }

        protected override async Task Executee()
        {
            ChatUser sender = Context.Sender;
            AbstractBotClient client = Context.Client;
            ChatGroup guild = Context.ChatGroup;
            string result;
            bool success = false;

            string languageCode = null;
            Localizer localizer = null;

            try
            {
                DatabaseHandler databaseHandler = AutoPigs.DatabaseHandler;
                localizer = AutoPigs.Localizer;
                languageCode = await databaseHandler.GetGuildLanguage(guild);

                if (await databaseHandler.PigHasCategory(await databaseHandler.GetUserAsPig(Target, guild), Category))
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

            await client.SendMessageAsync(Context.Channel.Id, text: result);
        }
    }
}

