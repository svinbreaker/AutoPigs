using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using СrossAppBot.Commands;
using СrossAppBot;

namespace AutoPigs.Commands.ArgumentParsers
{
    public class CategoryArgumentParser : IArgumentParser
    {
        public CategoryArgumentParser() : base(typeof(Category)) { }

        public override object Parse(string value, CommandContext context)
        {
            Category category = null;
            if (context != null)
            {
                category = AutoPigs.DatabaseHandler.GetGuildCategories(context.Guild).Result.FirstOrDefault(c => c.Name == value);
            }

            return category;
        }
    }
}