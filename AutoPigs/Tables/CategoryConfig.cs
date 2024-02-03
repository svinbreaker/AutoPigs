using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPigs
{
    class CategoryConfig
    {
        [NotNull, PrimaryKey, ForeignKey(typeof(Category))]
        public int CategoryId { get; set; }
        [NotNull]
        public int ReactionChance { get; set; }
        [NotNull]
        public int PictureChance { get; set; }


        public CategoryConfig() { }

        public CategoryConfig(Category category) 
        {
            CategoryId = category.Id;
            ReactionChance = 100;
            PictureChance = 10;
        }
    }
}
