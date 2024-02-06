using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BulbulatorLocalization;
using СrossAppBot.Commands;
using СrossAppBot;
using СrossAppBot.Entities.Files;
using СrossAppBot.Entities;

namespace AutoPigs.Commands.Pigs.Categories
{
    public class CreateCategoryCommand : AbstractCommand
    {
        [CommandArgument("name", null)]
        public string CategoryName { get; set; }
        [CommandArgument("battle pictures", null, true, true)]
        public List<ChatPicture> BattlePictures { get; }
        public CreateCategoryCommand() : base("createCategory", "COMMANDS_PIGS_CATEGORIES_CREATE_DESCRIPTION") { }

        public override async Task Execute(CommandContext context = null)
        {
            ChatGuild guild = context.Guild;
            AbstractBotClient client = context.Client;

            Localizer localizer = null;
            DatabaseHandler databaseHandler;
            string languageCode = null;
            
            string result;
            try
            {
                databaseHandler = AutoPigs.DatabaseHandler;
                localizer = AutoPigs.Localizer;
                languageCode = await databaseHandler.GetGuildLanguage(guild);

                if (CategoryName == null)
                {
                    result = "COMMANDS_PIGS_CATEGORIES_CREATE_ERROR_INVALID_NAME";
                }
                else if (CategoryName.Length > 32) 
                {
                    result = "COMMANDS_PIGS_CATEGORIES_CREATE_ERROR_INVALID_NAME";
                }
                else if ((await databaseHandler.GetGuildCategories(guild)).Where(c => c.Name == CategoryName).ToList().Count > 0)
                {
                    result = "COMMANDS_PIGS_CATEGORIES_CREATE_ERROR_CATEGORY_ALREADY_EXIST";
                }              
                else 
                {
                    await databaseHandler.AddGuildCategory(CategoryName, guild);
                    result = "COMMANDS_PIGS_CATEGORIES_CREATE_SUCCESS";

                    List<ChatMessageFile> files = context.Message.Files.Where(f => f is ChatPicture).ToList();
                    if (files.Count > 0) 
                    {
                        foreach(ChatMessageFile file in files) 
                        {
                            ChatPicture picture = file as ChatPicture;
                        }
                    }
                }
            } 
            catch(Exception exception) 
            {
                Console.WriteLine($"An error occurred while executing the command '{Name}': {exception.ToString()}\n{exception.Message}");
                result = "COMMANDS_ERROR_UNKNOWN_ERROR";
            }

            await client.SendMessageAsync(context.Channel.Id, localizer.GetLocalizedString(languageCode, result));
        }
    }
}