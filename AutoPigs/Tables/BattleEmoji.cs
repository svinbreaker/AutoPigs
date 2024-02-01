using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPigs.Tables
{
    public class BattleEmoji
    {
        [PrimaryKey, AutoIncrement, NotNull]
        public int Id { get; set; }

        [NotNull, ForeignKey(typeof(Category))]
        public int CategoryId { get; set; }

        [NotNull]
        public string Emoji { get; set; }

        public BattleEmoji() { }

        public BattleEmoji(string Emoji, Category category)
        {
            this.CategoryId = category.Id;
            this.Emoji = Emoji;
        }
    }
}
