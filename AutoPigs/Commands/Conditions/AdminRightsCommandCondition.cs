using BulbulatorLocalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using СrossAppBot.Commands;
using СrossAppBot.Entities;
using СrossAppBot;

namespace AutoPigs.Commands.Conditions
{
    public class AdminRightsCommandCondition : AbstractCommandCondition
    {
        private CommandContext Context { get; }

        public AdminRightsCommandCondition(CommandContext context)
        {
            Context = context;
        }

        public override async Task<bool> Condition()
        {
            ChatGroup guild = Context.ChatGroup;

            if (guild == null)
            {
                return false; //Probably will not work in non-guild discord groups
            }

           return (await Context.Sender.GetRightsAsync(guild)).Contains(UserRight.Administrator);
        }

        public override async Task Action()
        {
            Localizer localizer = AutoPigs.Localizer;
            DatabaseHandler databaseHandler = AutoPigs.DatabaseHandler;
            string languageCode = await databaseHandler.GetGuildLanguage(Context.ChatGroup);

            await Context.AnswerAsync(localizer.GetLocalizedString(languageCode, "COMMANDS_ERROR_NOT_ENOUGH_RIGHTS"), null, true); 
        }
    }
}
