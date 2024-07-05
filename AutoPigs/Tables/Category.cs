using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using СrossAppBot.Entities;
using СrossAppBot;

namespace AutoPigs
{
    public class Category
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [NotNull]
        public string Name { get; set; }
        [NotNull, ForeignKey(typeof(Guild))]
        public string GuildUniqueId { get; set; }

        public Category() { }
        public Category(string Name, string GuildUniqueId)
        {
            this.Name = Name;
            this.GuildUniqueId = GuildUniqueId;
        }

        public Category(string name, ChatGroup guild) 
        {
            this.Name = name;
            this.GuildUniqueId = AutoPigs.DatabaseHandler.GetGuildUniqueId(guild).Result;
        }

        /*public List<string> GetPictureLocations() 
        {
            List<BattlePicture> battlePictures = DatabaseHandler.Database.Query<BattlePicture>($"SELECT * FROM MarkingPicture WHERE CategoryId = {CategoryId}");
            if (battlePictures == null) 
            {
                return null;
            }
            else 
            {
                List<string> pictureLocations = new List<string>();
                foreach(BattlePicture battlePicture in battlePictures) 
                {
                    pictureLocations.Add(battlePicture.FileLocation);
                }
                return pictureLocations;
            }
        }

        public static Category GetById(int id) 
        {
            return DatabaseHandler.Database.Query<Category>($"SELECT * FROM Category WHERE CategoryId = {id}").SingleOrDefault();
        }*/
    }
}
