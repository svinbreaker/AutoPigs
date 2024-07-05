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
    public class CategoryNotExistCommandCondition : AbstractCommandCondition
    {
        DatabaseHandler _databaseHandler = AutoPigs.DatabaseHandler;
        private CommandContext Context { get; }
        private string CategoryName { get; }

        public CategoryNotExistCommandCondition(CommandContext context, string categoryName)
        {
            Context = context;
            CategoryName = categoryName;
        }

        public override async Task<bool> Condition()
        {
            ChatGroup guild = Context.ChatGroup;

            if (guild == null)
            {
                return false; //Probably will not work in non-guild discord groups
            }

            return (await _databaseHandler.GetGuildCategory(CategoryName, Context.ChatGroup) != null);
        }

        public override async Task Action()
        {
            Localizer localizer = AutoPigs.Localizer;
            string languageCode = await _databaseHandler.GetGuildLanguage(Context.ChatGroup);

            await Context.AnswerAsync(localizer.GetLocalizedString(languageCode, "COMMANDS_PIGS_CATEGORIES_ERROR_NOT_EXIST"), null, true);
        }
    }
}
