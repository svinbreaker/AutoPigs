using AutoPigs.Commands.Conditions;
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
        public AddBattlePictureCommand() : base("addPicture", "COMMANDS_PIGS_ADD_BATTLE_PICTURE_DESCRIPTION") { }

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

                if (Context.Message.Files.Where(file => file is ChatPicture).ToList().Count == 0)
                {
                    result = "COMMANDS_PIGS_BATTLE_PICTURE_FAIL_NO_PICTURE";
                }
                else
                {
                    foreach (ChatMessageFile file in Context.Message.Files)
                    {
                        if (file is ChatPicture picture)
                        {
                            await databaseHandler.AddBattlePicture(picture, Category, guild);
                        }
                    }
                    if (Context.Message.Files.Count > 1)
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

            await guild.Client.SendMessageAsync(Context.Channel.Id, localizer.GetLocalizedString(languageCode, result));
        }

    }
}

