using BulbulatorLocalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using СrossAppBot;
using СrossAppBot.Commands;
using СrossAppBot.Commands.Parameters;
using СrossAppBot.Entities;

namespace AutoPigs.Commands.General
{
    public class TestCommand : AbstractCommand
    {
        public TestCommand() : base("test", "COMMANDS_GENERAL_TEST_DESCRIPTION") { }
        protected async override Task Executee()
        {
            ChatGroup guild = Context.ChatGroup;
            DatabaseHandler databaseHandler = AutoPigs.DatabaseHandler;
            Localizer localizer = AutoPigs.Localizer;
            string languageCode = await databaseHandler.GetGuildLanguage(guild);

            await Context.AnswerAsync(localizer.GetLocalizedString(languageCode, "COMMANDS_GENERAL_TEST_SUCCESS"), null, true);
        }
    }
}
