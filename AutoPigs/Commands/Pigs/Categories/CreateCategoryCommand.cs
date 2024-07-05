using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BulbulatorLocalization;
using СrossAppBot.Commands;
using СrossAppBot;
using СrossAppBot.Entities.Files;
using СrossAppBot.Entities;
using AutoPigs.Commands.Conditions;

namespace AutoPigs.Commands.Pigs.Categories
{
    public class CreateCategoryCommand : AbstractCommand
    {
        [CommandArgument("name", null)]
        public string CategoryName { get; set; }
        [CommandArgument("battle pictures", null, true, true)]
        public List<ChatPicture> BattlePictures { get; }
        public CreateCategoryCommand() : base("createCategory", "COMMANDS_PIGS_CATEGORIES_CREATE_DESCRIPTION") { }

        public override void Conditions()
        {
            Condition(new SenderIsNotPigCommandCondition(Context));
            Condition(new AdminRightsCommandCondition(Context));
            Condition(new CategoryNotExistCommandCondition(Context, CategoryName));
        }

        protected override async Task Executee()
        {
            ChatGroup guild = Context.ChatGroup;
            AbstractBotClient client = Context.Client;

            Localizer localizer = null;
            DatabaseHandler databaseHandler;
            string languageCode = null;

            string result;
            try
            {
                databaseHandler = AutoPigs.DatabaseHandler;
                localizer = AutoPigs.Localizer;
                languageCode = await databaseHandler.GetGuildLanguage(guild);


                if (CategoryName.Length > 32)
                {
                    result = "COMMANDS_PIGS_CATEGORIES_CREATE_ERROR_INVALID_NAME";
                }
                else
                {
                    await databaseHandler.AddGuildCategory(CategoryName, guild);
                    result = "COMMANDS_PIGS_CATEGORIES_CREATE_SUCCESS";

                    List<ChatMessageFile> files = Context.Message.Files.Where(f => f is ChatPicture).ToList();
                    if (files.Count > 0)
                    {
                        foreach (ChatMessageFile file in files)
                        {
                            ChatPicture picture = file as ChatPicture;
                        }
                    }
                }
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