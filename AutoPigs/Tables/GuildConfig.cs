using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoPigs;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace AutoPigs
{
    class GuildConfig
    {
        [NotNull, PrimaryKey, ForeignKey(typeof(Guild))]
        public string GuildUniqueId { get; set; }

        [NotNull]
        public string Language { get; set; }
        [NotNull]
        public int ReactionChance { get; set; }
        [NotNull]
        public int PictureChance { get; set; }

        public string ReactionEmoji { get; set; }

        public GuildConfig() {}

        public GuildConfig(Guild guild) 
        {
            ReactionChance = 100;
            Language = "ru";
            PictureChance = 5;
            GuildUniqueId = guild.UniqueId;
            ReactionEmoji = "🐷";
        }
    }
}
