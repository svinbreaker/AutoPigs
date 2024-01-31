
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
    public class Pig
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [NotNull, ForeignKey(typeof(Guild))]
        public string GuildId { get; set; }
        [NotNull]
        public string UserId { get; set; }
        [NotNull, ForeignKey(typeof(Client))]
        public string ClientName { get; set; }

        public Pig() {}
        
        public Pig(string userId, string clientName, string guildId) 
        {
            GuildId = guildId;
            UserId = userId;
            ClientName = clientName;
        }

        public Pig(ChatUser user, ChatGuild guild) 
        {
            GuildId = guild.Id;
            UserId = user.Id;
            ClientName = user.Client.Name;
        }

        /*public static bool UserIsMarked(AppUser user, AppGuild server) 
        {
            return DatabaseHandler.Database.Query<Pig>($"SELECT * FROM MarkedUser WHERE ServerId = '{server.CategoryId}' AND UserLocalId = '{user.LocalId}'").SingleOrDefault() != null;
        }

        public static List<Category> GetUserCategories(AppUser user, AppGuild server) 
        {
            return DatabaseHandler.Database.Query<Category>($@"SELECT c.*
          FROM Category c
          INNER JOIN UsersCategories uc ON uc.CategoryId = c.CategoryId
          INNER JOIN MarkedUser mu ON mu.UserId = uc.UserId
          INNER JOIN Server s ON s.CategoryId = mu.ServerId
          WHERE mu.UserId = '{server.AppId}_{user.LocalId}' AND s.CategoryId = '{server.CategoryId}'").ToList();
        }*/
    }
}

