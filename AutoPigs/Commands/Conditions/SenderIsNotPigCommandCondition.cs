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
    public class SenderIsNotPigCommandCondition : AbstractCommandCondition
    {
        private CommandContext Context { get; }
        private DatabaseHandler _databaseHandler = AutoPigs.DatabaseHandler;

        public SenderIsNotPigCommandCondition(CommandContext context)
        {
            Context = context;
        }

        public override async Task<bool> Condition()
        {
            ChatGroup group = Context.ChatGroup;

            if (group == null)
            {
                return false; //Probably will not work in non-group discord groups
            }

            return !(await _databaseHandler.UserIsPig(Context.Sender, group));
        }

        public override async Task Action()
        {
            Localizer localizer = AutoPigs.Localizer;
            string languageCode = await _databaseHandler.GetGuildLanguage(Context.ChatGroup);

            await Context.AnswerAsync(localizer.GetLocalizedString(languageCode, "COMMANDS_ERROR_PIG"), null, true);
        }
    }
}
