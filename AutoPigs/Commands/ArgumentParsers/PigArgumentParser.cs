using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using СrossAppBot.Commands;

namespace AutoPigs.Commands.ArgumentParsers
{
    public class PigArgumentParser : IArgumentParser
    {
        public PigArgumentParser() : base(typeof(Pig)) { }

        public override object Parse(string value, CommandContext context = null)
        {
            Pig pig = null;
            if (context != null)
            {
                pig = AutoPigs.DatabaseHandler.GetUserAsPig(context.Client.GetUserByMention(context.Message, value).Result, context.ChatGroup).Result;
            }

            return pig;
        }
    }
}