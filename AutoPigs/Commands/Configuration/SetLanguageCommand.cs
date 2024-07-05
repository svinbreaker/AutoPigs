using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using СrossAppBot.Commands;
using СrossAppBot;
using BulbulatorLocalization;
using СrossAppBot.Entities;
using AutoPigs.Commands.Conditions;

namespace AutoPigs.Commands.Configuration
{
    public class SetLanguageCommand : AbstractCommand
    {
        [CommandArgument("language", null, true)]
        public string SelectedLanguage { get; set; }

        public SetLanguageCommand() : base("language", "COMMANDS_CONFIGURATION_LANGUAGE_DESCRIPTION") { }

        public override void Conditions() 
        {
            Condition(new SenderIsNotPigCommandCondition(Context));
            Condition(new AdminRightsCommandCondition(Context));
        }

        protected async override Task Executee()
        {
            AbstractBotClient client = Context.Client;
            ChatGroup guild = Context.ChatGroup;
            DatabaseHandler databaseHandler = AutoPigs.DatabaseHandler;
            Localizer localizer = AutoPigs.Localizer;
            string languageCode = await databaseHandler.GetGuildLanguage(guild);
            string result;

            List<string> languages = localizer.GetLanguages();

            if (SelectedLanguage == null | (!languages.Contains(SelectedLanguage)))
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(localizer.GetLocalizedString(languageCode, "COMMANDS_CONFIGURATION_LANGUAGE_AVAILABLE_LANGUAGES"));
                foreach (string language in languages)
                {
                    builder.Append($"{language},");
                }
                builder.Remove(builder.Length - 1, 1);
                result = builder.ToString();
            }
            else 
            {
                GuildConfig config = await databaseHandler.GetGuildConfig(guild);
                config.Language = SelectedLanguage;
                await databaseHandler.Database.UpdateAsync(config);
                result = localizer.GetLocalizedString(SelectedLanguage, "COMMANDS_CONFIGURATION_LANGUAGE_SUCCESS");
            }
           
            await client.SendMessageAsync(Context.Channel.Id, result);
        }
    }
}
