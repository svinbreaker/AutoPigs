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
            if (client is TelegramBotClient) 
            {
                List<string> files = new List<string>() { 
                    "C:/Users/Cracker/Downloads/_14d9be6b-0c33-455d-b2e7-4bfbda546572.jpg", 
                    "C:/Users/Cracker/Downloads/chesscringe.mp4", 
                    "C:/Users/Cracker/Downloads/Magellanovo_Oblako_-_Aksioma_(patefon.org).mp3",
                    "C:/Users/Cracker/Downloads/Magellanovo_Oblako_-_Aksioma_(patefon.org).mp3"
                    };               
                await client.SendMessageAsync(context.Channel.Id, files: files);
            }
        }
    }
}
