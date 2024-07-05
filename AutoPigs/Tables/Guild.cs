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
    public class Guild
    {
        [NotNull, PrimaryKey]
        public string UniqueId { get; set; }
        [NotNull]
        public string Id { get; set; }

        [NotNull, ForeignKey(typeof(Client))]
        public string ClientName { get; set; }

        public Guild() { }

        public Guild(ChatGroup guild)
        {
            Id = guild.Id;
            ClientName = guild.Client.Name;
            UniqueId = $"{ClientName}_{Id}";
        }
    }
}
