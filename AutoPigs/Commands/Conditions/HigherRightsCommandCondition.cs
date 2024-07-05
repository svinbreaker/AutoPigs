using BulbulatorLocalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using СrossAppBot.Commands;
using СrossAppBot.Entities;
using СrossAppBot;
using AutoPigs;


namespace AutoPigs.Commands.Conditions
{
    public class HigherRightsCommandCondition : AbstractCommandCondition
    {
        private CommandContext Context { get; }
        private ChatUser Target { get; set; }

        public HigherRightsCommandCondition(CommandContext context, ChatUser target)
        {
            Context = context;
            Target = target;
        }

        public override async Task<bool> Condition()
        {        
            ChatGroup guild = Context.ChatGroup;

            if (guild == null || Target == null)
            {
                return false; //Probably will not work in non-guild discord groups
            }

            ChatUser author = Context.Sender;

            List<UserRight> authorRights = await author.GetRightsAsync(guild);
            if (authorRights.Contains(UserRight.Owner)) 
            {
                return true;
            }
            else 
            {
                List<UserRight> targetRights = await Target.GetRightsAsync(guild);
                return authorRights.Contains(UserRight.Administrator) & !targetRights.Contains(UserRight.Administrator);
            }
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