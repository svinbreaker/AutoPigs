using BulbulatorLocalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using СrossAppBot.Commands;
using СrossAppBot.Entities;
using СrossAppBot;

namespace AutoPigs.Commands.Configuration
{
    public class RemoveReactionCommand : AbstractCommand
    {
        [CommandArgument("emoji", null)]
        public string EnteredEmoji { get; set; }
        [CommandArgument("category", null, optional: true)]
        public Category Category { get; set; }
        public RemoveReactionCommand() : base("removeReaction", "COMMANDS_CONFIGURATION_REMOVE_REACTION_DESCRIPTION") { }
        public async override Task Execute(CommandContext context = null)
        {
            AbstractBotClient client = context.Client;
            ChatGuild guild = context.Guild;
            DatabaseHandler databaseHandler = AutoPigs.DatabaseHandler;
            Localizer localizer = AutoPigs.Localizer;
            string languageCode = databaseHandler.GetGuildConfig(guild).Language;
            string result;


            try
            {
                if (Category == null)
                {
                    Category = databaseHandler.GetDefaultCategory(guild);
                }
                if (!(client as IEmojiable).IsReactableEmoji(EnteredEmoji))
                {
                    result = "COMMANDS_CONFIGURATION_REMOVE_REACTION_DESCRIPTION";
                }
                else
                {
                    CategoryConfig config = databaseHandler.GetCategoryConfig(Category);
                    List<string> emojis = databaseHandler.GetBattleEmojis(Category).Select(e => e.Emoji).ToList();
                    if (!emojis.Contains(EnteredEmoji))
                    {
                        result = "COMMANDS_CONFIGURATION_REMOVE_REACTION_ERROR_NOT_EXIST";
                    }
                    else
                    {
                        await databaseHandler.RemoveBattleReaction(EnteredEmoji, Category);
                        result = "COMMANDS_CONFIGURATION_REMOVE_REACTION_SUCCESS";
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
