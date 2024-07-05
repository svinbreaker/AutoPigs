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
    public class RemovePigCategory : AbstractCommand
    {
        [CommandArgument("User", null)]
        public ChatUser Target { get; set; }
        [CommandArgument("Category", null, optional: true)]
        public Category Category { get; set; }
        public RemovePigCategory() : base("removeCategory", "COMMANDS_PIGS_CATEGORIES_REMOVE_PIG_CATEGORY_DESCRIPTION") { }

        public override void Conditions()
        {
            Condition(new SenderIsNotPigCommandCondition(Context));
            Condition(new HigherRightsCommandCondition(Context, Target));
            Condition(new TargetIsNotSenderCommandCondition(Context, Target));
            Condition(new TargetIsPigCommandCondition(Context, Target));
            Condition(new PigHasTheCategoryCommandCondition(Context, Pig.FromUser(Target, Context.ChatGroup), Category));
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



                await databaseHandler.RemovePigCategory(await databaseHandler.GetUserAsPig(Target, guild), Category);
                result = "COMMANDS_PIGS_CATEGORIES_REMOVE_PIG_CATEGORY_SUCCESS";
                success = true;

            }
            catch (Exception exception)
            {
                Console.WriteLine($"An error occurred while executing the command '{Name}': {exception.ToString()}\n{exception.Message}");
                result = "COMMANDS_ERROR_UNKNOWN_ERROR";
            }

            await client.SendMessageAsync(Context.Channel.Id, localizer.GetLocalizedString(languageCode, result));
        }
    }
}



