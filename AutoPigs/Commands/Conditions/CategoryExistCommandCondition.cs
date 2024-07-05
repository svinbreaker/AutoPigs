using BulbulatorLocalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using СrossAppBot.Entities;
using СrossAppBot;
using СrossAppBot.Commands;

namespace AutoPigs.Commands.Conditions
{
    public class CategoryExistCommandCondition : AbstractCommandCondition
    {
        private DatabaseHandler _databaseHandler = AutoPigs.DatabaseHandler;
        private CommandContext Context { get; }
        private Category Category { get;  }

        public CategoryExistCommandCondition(CommandContext context, Category category)
        {
            Context = context;
            Category = category;
        }

        public override async Task<bool> Condition()
        {
            ChatGroup guild = Context.ChatGroup;

            if (guild == null)
            {
                return false; //Probably will not work in non-guild discord groups
            }

            return Category != null;
        }

        public override async Task Action()
        {
            Localizer localizer = AutoPigs.Localizer;
            string languageCode = await _databaseHandler.GetGuildLanguage(Context.ChatGroup);

            await Context.AnswerAsync(localizer.GetLocalizedString(languageCode, "COMMANDS_PIGS_CATEGORIES_ERROR_NOT_EXIST"), null, true);
        }
    }
}

