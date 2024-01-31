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
        public async override Task Execute(CommandContext context = null)
        {
            AbstractBotClient client = context.Client;
            ChatGuild guild = context.Guild;
            DatabaseHandler databaseHandler = AutoPigs.DatabaseHandler;
            Localizer localizer = AutoPigs.Localizer;
            string languageCode = databaseHandler.GetGuildConfig(guild).Language;

            await client.SendMessageAsync(context.Channel.Id, localizer.GetLocalizedString(languageCode, "COMMANDS_GENERAL_TEST_SUCCESS"));
        }
    }
}
