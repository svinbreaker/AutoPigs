using AutoPigs.Commands.CommandConditions;
using AutoPigs.Commands.Conditions;
using BulbulatorLocalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using СrossAppBot;
using СrossAppBot.Commands;
using СrossAppBot.Entities;

namespace AutoPigs.Commands.Pigs
{
    public class PigsAddCommand : AbstractCommand
    {
        [CommandArgument("User", null)]
        public ChatUser Target { get; set; }
        [CommandArgument("Category", null, optional: true)]
        public Category Category { get; set; }
        public PigsAddCommand() : base("add", "COMMANDS_PIGS_ADD_DESCRIPTION") { }

        public override void Conditions()
        {
            Condition(new SenderIsNotPigCommandCondition(Context));
            Condition(new HigherRightsCommandCondition(Context, Target));
            Condition(new UserExistCommandCondition(Context, Target));
            Condition(new CategoryExistOrEmptyCommandCondition(Context, Category?.Name ?? null));
            Condition(new TargetIsNotPigCommandCondition(Context, Target));
            Condition(new TargetIsNotSenderCommandCondition(Context, Target));
        }

        protected override async Task Executee()
        {
            ChatUser sender = Context.Sender;
            AbstractBotClient client = Context.Client;
            ChatGroup guild = Context.ChatGroup;
            string result;

            string languageCode = null;
            Localizer localizer = null;

            try
            {
                DatabaseHandler databaseHandler = AutoPigs.DatabaseHandler;
                localizer = AutoPigs.Localizer;
                languageCode = await databaseHandler.GetGuildLanguage(guild);

                if (Category == null)
                {
                    Category = await databaseHandler.GetDefaultCategory(guild);
                }

                Pig pig = new Pig(Target, guild);
                await databaseHandler.AddPig(pig);
                await databaseHandler.SetPigCategory(pig, Category);
                result = "COMMANDS_PIGS_ADD_SUCCESS";
            }
            catch (Exception exception)
            {
                Console.WriteLine($"An error occurred while executing the command '{Name}': {exception.ToString()}\n{exception.Message}");
                result = "COMMANDS_ERROR_UNKNOWN_ERROR";
            }

            await Context.AnswerAsync(client.Mention(Target) + " " + localizer.GetLocalizedString(languageCode, result), null, true);
        }
    }
}
