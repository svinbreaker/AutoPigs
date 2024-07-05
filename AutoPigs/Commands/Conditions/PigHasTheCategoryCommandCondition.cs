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
    public class PigHasTheCategoryCommandCondition : AbstractCommandCondition
    {
        private DatabaseHandler _databaseHandler = AutoPigs.DatabaseHandler;
        private CommandContext Context { get; }
        private Pig Pig { get; }
        private Category Category { get; }

        public PigHasTheCategoryCommandCondition(CommandContext context, Pig pig, Category category)
        {
            Context = context;
            Pig = pig;
            Category = category;
        }

        public override async Task<bool> Condition()
        {
            ChatGroup guild = Context.ChatGroup;

            if (guild == null)
            {
                return false; //Probably will not work in non-guild discord groups
            }

            return (await _databaseHandler.PigHasCategory(Pig, Category));
        }

        public override async Task Action()
        {
            Localizer localizer = AutoPigs.Localizer;
            DatabaseHandler databaseHandler = AutoPigs.DatabaseHandler;
            string languageCode = await databaseHandler.GetGuildLanguage(Context.ChatGroup);

            await Context.AnswerAsync(localizer.GetLocalizedString(languageCode, "СOMMANDS_PIGS_CATEGORIES_ERROR_PIG_HAS_NOT_THE_CATEGORY"), null, true);
        }
    }
}
