using BulbulatorLocalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using СrossAppBot.Entities;
using СrossAppBot.Commands;
using СrossAppBot;
using AutoPigs.Commands.Conditions;

namespace AutoPigs.Commands.Pigs
{
    class PigsRemoveCommand : AbstractCommand
    {
        [CommandArgument("User", null)]
        public ChatUser Target { get; set; }
        public PigsRemoveCommand() : base("remove", "COMMANDS_PIGS_REMOVE_DESCRIPTION") { }

        public override void Conditions()
        {
            Condition(new SenderIsNotPigCommandCondition(Context));
            Condition(new HigherRightsCommandCondition(Context, Target));
            Condition(new UserExistCommandCondition(Context, Target));
            Condition(new TargetIsPigCommandCondition(Context, Target));
            Condition(new TargetIsNotSenderCommandCondition(Context, Target));
        }

        protected override async Task Executee()
        {

            ChatUser sender = Context.Sender;
            AbstractBotClient client = Context.Client;
            ChatGroup guild = Context.ChatGroup;
            string result;

            Localizer localizer = null;
            string languageCode = null;
            try
            {
                DatabaseHandler databaseHandler = AutoPigs.DatabaseHandler;
                localizer = AutoPigs.Localizer;
                languageCode = await databaseHandler.GetGuildLanguage(guild);

                await databaseHandler.RemovePig(await databaseHandler.GetUserAsPig(Target, guild));
                result = "COMMANDS_PIGS_REMOVE_SUCCESS";
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

