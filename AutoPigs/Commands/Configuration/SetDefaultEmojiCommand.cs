using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using СrossAppBot.Commands;
using СrossAppBot;
using Discord;
using BulbulatorLocalization;
using СrossAppBot.Entities;

namespace AutoPigs.Commands.Configuration
{
    public class SetDefaultEmojiCommand : AbstractCommand
    {
        [CommandArgument("emoji", null)]
        public string EnteredEmoji { get; set; }
        [CommandArgument("category", null, optional: true)]
        public Category Category { get; set; }
        public SetDefaultEmojiCommand() : base("setEmoji", "COMMANDS_CONFIGURATION_DEFAULT_EMOJI_DESCRIPTION") { }
        public async override Task Execute(CommandContext context = null)
        {
            AbstractBotClient client = context.Client;
            ChatGuild guild = context.Guild;
            DatabaseHandler databaseHandler = AutoPigs.DatabaseHandler;
            Localizer localizer = AutoPigs.Localizer;
            string languageCode = databaseHandler.GetGuildConfig(guild).Language;
            string result = "ъ";


            Emoji emoji = null;
            Emote emote = null;
            try
            {
                if (Category == null)
                {
                    Category = databaseHandler.GetDefaultCategory(guild);
                }
                if (!Emoji.TryParse(EnteredEmoji, out emoji) && !Emote.TryParse(EnteredEmoji, out emote))
                {
                    result = "COMMANDS_CONFIGURATION_DEFAULT_EMOJI_DESCRIPTION";
                }
                else
                {
                    CategoryConfig config = databaseHandler.GetCategoryConfig(Category);
                    if (emote == null)
                    {
                        config.ReactionEmoji = emoji.ToString();
                    }
                    else
                    {
                        config.ReactionEmoji = emote.ToString();
                    }
                    databaseHandler.Database.Update(config);
                    result = "COMMANDS_CONFIGURATION_DEFAULT_EMOJI_SUCCESS";
                }
            } catch(Exception exception) 
            {
                Console.WriteLine(exception.ToString());
            }
            await client.SendMessageAsync(context.Channel.Id, text: result);
        }
    }
}