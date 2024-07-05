using BulbulatorLocalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using СrossAppBot.Commands;
using СrossAppBot.Entities;

namespace AutoPigs.Commands.Conditions
{
    public class TargetIsNotPigCommandCondition : AbstractCommandCondition
    {
        private CommandContext Context { get; }
        private ChatUser Target { get; }
        private DatabaseHandler _databaseHandler = AutoPigs.DatabaseHandler;

        public TargetIsNotPigCommandCondition(CommandContext context, ChatUser target)
        {
            Context = context;
            Target = target;
        }

        public override async Task<bool> Condition()
        {
            ChatGroup group = Context.ChatGroup;

            if (group == null)
            {
                return false; //Probably will not work in non-group discord groups
            }

            return !(await _databaseHandler.UserIsPig(Target, group));
        }

        public override async Task Action()
        {
            Localizer localizer = AutoPigs.Localizer;
            string languageCode = await _databaseHandler.GetGuildLanguage(Context.ChatGroup);

            await Context.AnswerAsync(localizer.GetLocalizedString(languageCode, "COMMANDS_ERROR_TARGET_IS_PIG"), null, true);
        }
    }
}
