using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPigs
{
    class BattlePicture
    {
        [PrimaryKey, AutoIncrement, NotNull]
        public int Id { get; set; }

        [NotNull, ForeignKey(typeof(Category))]
        public int CategoryId { get; set; }

        [NotNull]
        public string FileLocation { get; set; }

        public BattlePicture() {}

        public BattlePicture(string FileLocation, Category category) 
        {
            this.CategoryId = category.Id;
            this.FileLocation = FileLocation;
        }
    }
}
