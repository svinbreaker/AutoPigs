using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using СrossAppBot.Commands;

namespace AutoPigs.Commands.CommandConditions
{
    public class CommandConditionAttribute : Attribute
    {
        public Type Condition { get; internal set; }
        public string AttributeName { get; internal set; }
        public CommandConditionAttribute(Type condition, string attributeName) 
        {
            Condition = condition;
            AttributeName = attributeName;
        }
    }
}
