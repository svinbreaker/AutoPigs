using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPigs
{
    class PigsCategories
    {
        [NotNull, PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [ForeignKey(typeof(Pig))]
        public int PigId { get; set; }
        [ForeignKey(typeof(Category))]
        public int CategoryId { get; set; }

        public PigsCategories() { }

        public PigsCategories(int PigId, int CategoryId) 
        {
            this.PigId = PigId;
            this.CategoryId = CategoryId;
        }
    }
}
