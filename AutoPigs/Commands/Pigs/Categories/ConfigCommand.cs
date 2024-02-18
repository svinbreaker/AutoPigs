using BulbulatorLocalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using СrossAppBot.Commands;
using СrossAppBot.Entities;
using СrossAppBot;
using AutoPigs.Tables;

namespace AutoPigs.Commands.Pigs.Categories
{
    public class ConfigCommand : AbstractCommand
    {
        [CommandArgument("category", null, optional: true)]
        public Category Category { get; set; }
        public ConfigCommand() : base("config", "COMMANDS_CONFIG_DESCRIPTION") { }
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
                CategoryConfig config = await databaseHandler.GetCategoryConfig(Category);

                StringBuilder builder = new StringBuilder();
                builder.Append($"{Category.Name}:\n")
                        .Append($"picture chance: {config.PictureChance}%\n")
                        .Append($"pictures: {(await databaseHandler.GetBattlePictures(Category)).Count}\n");
                if (client is IAddReaction) 
                {
                    builder.Append($"reaction chance: {config.ReactionChance}%\n")
                           .Append($"reactions:  ");
                    foreach(BattleReaction reaction in await databaseHandler.GetBattleEmojis(Category)) 
                    {
                        builder.Append(reaction.Emoji).Append(", ");
                    }
                    builder.Length -= 2;
                }
                result = builder.ToString();
            }
            catch (Exception exception)
            {
                Console.WriteLine($"An error occurred while executing the command '{Name}': {exception.ToString()}\n{exception.Message}");
                result = "COMMANDS_ERROR_UNKNOWN_ERROR";
            }
            await client.SendMessageAsync(context.Channel.Id, text: result);
        }
    }
}
