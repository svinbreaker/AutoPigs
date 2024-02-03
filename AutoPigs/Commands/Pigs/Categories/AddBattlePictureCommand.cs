using BulbulatorLocalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using СrossAppBot;
using СrossAppBot.Commands;
using СrossAppBot.Entities;
using СrossAppBot.Entities.Files;

namespace AutoPigs.Commands.Pigs.Categories
{
    public class AddBattlePictureCommand : AbstractCommand
    {
        [CommandArgument("Category", null)]
        public Category Category { get; set; }
        public AddBattlePictureCommand() : base("addBattlePicture", "COMMANDS_PIGS_ADD_BATTLE_PICTURE_DESCRIPTION") { }

        public override async Task Execute(CommandContext context = null)
        {
            string result;
            DatabaseHandler databaseHandler = AutoPigs.DatabaseHandler;
            Localizer localizer = AutoPigs.Localizer;
            ChatGuild guild = context.Guild;
            string languageCode = databaseHandler.GetGuildConfig(guild).Language;

            try
            {
                if (Category == null)
                {
                    Category = databaseHandler.GetDefaultCategory(guild);
                }

                if (context.Message.Files.Where(file => file is ChatPicture).ToList().Count == 0)
                {
                    result = "COMMANDS_PIGS_BATTLE_PICTURE_FAIL_NO_PICTURE";
                }
                else
                {
                    foreach (ChatMessageFile file in context.Message.Files)
                    {
                        if (file is ChatPicture picture)
                        {
                            await databaseHandler.AddBattlePicture(picture, Category, guild);
                        }
                    }
                    if (context.Message.Files.Count > 1)
                    {
                        result = "COMMANDS_PIGS_ADD_BATTLE_PICTURE_MULTIPLE_SUCCESS";
                    }
                    else
                    {
                        result = "COMMANDS_PIGS_ADD_BATTLE_PICTURE_SUCCESS";
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message + "\n" + exception.ToString());
                result = "";
            }

            await guild.Client.SendMessageAsync(context.Channel.Id, localizer.GetLocalizedString(languageCode, result));
        }

    }
}

