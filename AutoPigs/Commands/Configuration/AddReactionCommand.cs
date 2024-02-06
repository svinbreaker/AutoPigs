using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using СrossAppBot.Commands;
using СrossAppBot;
using BulbulatorLocalization;
using СrossAppBot.Entities;
using AutoPigs.Tables;


namespace AutoPigs.Commands.Configuration
{
    public class AddReactionCommand : AbstractCommand
    {
        [CommandArgument("emoji", null)]
        public string EnteredEmoji { get; set; }
        [CommandArgument("category", null, optional: true)]
        public Category Category { get; set; }
        public AddReactionCommand() : base("addReaction", "COMMANDS_CONFIGURATION_ADD_REACTION_DESCRIPTION") { }
        public async override Task Execute(CommandContext context = null)
        {
            AbstractBotClient client = context.Client;
            ChatGuild guild = context.Guild;
            DatabaseHandler databaseHandler = AutoPigs.DatabaseHandler;
            Localizer localizer = AutoPigs.Localizer;
            string languageCode = await databaseHandler.GetGuildLanguage(guild);
            string result;


            try
            {
                if (Category == null)
                {
                    Category = await databaseHandler.GetDefaultCategory(guild);
                }
                if (!(client as IEmojiable).IsReactableEmoji(EnteredEmoji))
                {
                    result = "COMMANDS_CONFIGURATION_ADD_REACTION_DESCRIPTION";
                }
                else
                {
                    CategoryConfig config = await databaseHandler.GetCategoryConfig(Category);
                    List<string> emojis = (await databaseHandler.GetBattleEmojis(Category)).Select(e => e.Emoji).ToList();
                    if (emojis.Contains(EnteredEmoji))
                    {
                        result = "COMMANDS_CONFIGURATION_ADD_REACTION_ERROR_ALREADY_EXISTS";
                    }
                    else 
                    {
                        await databaseHandler.AddBattleReaction(EnteredEmoji, Category);
                        result = "COMMANDS_CONFIGURATION_ADD_REACTION_SUCCESS";
                    }


                    
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"An error occurred while executing the command '{Name}': {exception.ToString()}\n{exception.Message}");
                result = "COMMANDS_ERROR_UNKNOWN_ERROR";
            }
            await client.SendMessageAsync(context.Channel.Id, text: localizer.GetLocalizedString(languageCode, result));
        }
    }
}