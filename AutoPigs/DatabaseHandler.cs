using AutoPigs.Commands;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using СrossAppBot;
using СrossAppBot;
using СrossAppBot.Entities;
using СrossAppBot.Entities.Files;

namespace AutoPigs
{
    class DatabaseHandler
    {
        public SQLiteConnection Database { get; set; }
        private string FilePath;


        public DatabaseHandler(string filePath)
        {
            FilePath = filePath;
            ConnectToDatabase();
        }

        public void ConnectToDatabase()
        {
            //Создание базы данных и таблиц при их отсутствии
            //System.IO.File.Delete(FilePath);
            if (!System.IO.File.Exists(FilePath))
            {
                Database = new SQLite.SQLiteConnection(FilePath, SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.ReadWrite);
                Database.CreateTable<Pig>();
                Database.CreateTable<Guild>();
                Database.CreateTable<GuildConfig>();
                Database.CreateTable<BattlePicture>();
                Database.CreateTable<Category>();
                Database.CreateTable<CategoryConfig>();
                Database.CreateTable<PigsCategories>();
                Database.Close();
            }
            //Подключение к базе данных
            Database = new SQLite.SQLiteConnection(FilePath);

        }

        public bool UserIsPig(ChatUser user, ChatGuild guild)
        {
            return Database.Query<Pig>($"SELECT * FROM Pig WHERE UserId = '{user.Id}' AND ClientName = '{user.Client.Name}' AND GuildId = '{guild.Id}'").Count > 0;
        }

        public void AddPig(string userId, string clientName, string guildId)
        {
            Pig pig = new Pig(userId, clientName, guildId);
            Database.Insert(pig);
        }

        public void AddPig(Pig pig)
        {
            Database.Insert(pig);
        }

        public void RemovePig(Pig pig)
        {
            Database.Delete(pig);
        }

        public Pig GetUserAsPig(ChatUser user, ChatGuild guild)
        {
            return Database.Query<Pig>($"SELECT * FROM Pig WHERE UserId = '{user.Id}' AND ClientName = '{user.Client.Name}' AND GuildId = '{guild.Id}'").FirstOrDefault();
        }

        public List<Pig> GetPigs(string guildId, string clientName)
        {
            return Database.Query<Pig>($"SELECT * FROM Pig WHERE GuildId = '{guildId}' AND ClientName = '{clientName}'").ToList();
        }

        public void CreateGuildConfig(ChatGuild guild)
        {
            Guild guildObject = new Guild(guild);
            GuildConfig config = new GuildConfig(guildObject);
            Database.Insert(guildObject);
            Database.Insert(config);

            AddDefaultCategory(guild);
        }

        public bool GuildConfigExists(ChatGuild guild)
        {
            return Database.Query<Pig>($"SELECT * FROM Guild WHERE Id = '{guild.Id}' AND ClientName = '{guild.Client.Name}'").Count > 0;
        }

        public GuildConfig GetGuildConfig(ChatGuild guild)
        {   
            return Database.Query<GuildConfig>($"SELECT * FROM GuildConfig WHERE GuildUniqueId = '{GetGuildUniqueId(guild)}'").FirstOrDefault();
        }

        public List<Category> GetGuildCategories(ChatGuild guild)
        {
            return Database.Query<Category>($"SELECT * FROM Category WHERE GuildUniqueId = '{GetGuildUniqueId(guild)}'");
        }

        public void AddGuildCategory(string categoryName, ChatGuild guild)
        {
            Category category = new Category(categoryName, guild);
            Database.Insert(category);
            CategoryConfig config = new CategoryConfig(category);       
            Database.Insert(config);
        }

        public void RemoveGuildCategory(Category category)
        {
            Database.Delete(GetCategoryConfig(category));
            foreach(Pig pig in GetPigsOfCategory(category)) 
            {
                RemovePigCategory(pig, category);
            }
            Database.Delete(category);         
        }

        public void SetPigCategory(Pig pig, Category category)
        {
            Database.Insert(new PigsCategories(pig.Id, category.Id));
        }

        public void RemovePigCategory(Pig pig, Category category)
        {
            PigsCategories pigCategory = Database.Query<PigsCategories>($"SELECT * FROM PigsCategories WHERE PigId = {pig.Id} AND CategoryId = {category.Id}").FirstOrDefault();
            Database.Delete(pigCategory);
        }

        public bool PigHasCategory(Pig pig, Category category) 
        {
            return Database.Query<PigsCategories>($"SELECT * FROM PigsCategories WHERE PigId = {pig.Id} AND CategoryId = {category.Id}").ToList().Count > 0;
        }

        public List<Pig> GetPigsOfCategory(Category category) 
        {
            return Database.Query<Pig>($"SELECT * FROM PIG JOIN PigsCategories ON Pig.Id = PigsCategories.PigId WHERE PigsCategories.CategoryId = '{category.Id}'");
        }

        public List<Category> GetCategoriesOfPig(Pig pig)
        {
            return Database.Query<Category>($"SELECT * FROM Category JOIN PigsCategories ON Category.Id = PigsCategories.CategoryId WHERE PigsCategories.PigId = {pig.Id}").ToList();
        }

        public async Task AddBattlePicture(ChatPicture picture, Category category, ChatGuild guild)
        {
            string pictureLocation = null;
            using (HttpClient client = new HttpClient())
            {
                string pictureFolder = $@"{AutoPigs.DataPath}{guild.Client.Name}\{guild.Name}\{category.Name}\";
                if (!Directory.Exists(pictureFolder))
                {
                    Directory.CreateDirectory(pictureFolder);
                }
                string pictureName = Directory.GetFiles(pictureFolder, ".png", SearchOption.TopDirectoryOnly).Length.ToString();

                pictureLocation = pictureFolder + pictureName + ".png";
                byte[] imageData = await client.GetByteArrayAsync(picture.Url);
               
                File.WriteAllBytes(pictureLocation, imageData);
            }

            Database.Insert(new BattlePicture(pictureLocation, category));
        }

        public void ClearBattlePictures(Category category, ChatGuild guild)
        {
            List<BattlePicture> battlePictures = Database.Query<BattlePicture>($"SELECT * FROM BattlePicture WHERE CategoryId = {category.Id}");
            battlePictures.ForEach(battlePicture => Database.Delete(battlePicture));

            string pictureFolder = $@"{AutoPigs.DataPath}{guild.Client.Name}\{guild.Name}\{category.Name}\";
            if (!Directory.Exists(pictureFolder))
            {
                Directory.CreateDirectory(pictureFolder);
            }
            foreach(string file in Directory.GetFiles(pictureFolder, "*")) 
            {
                File.Delete(file);
            }
        }

        public Guild GetGuild(ChatGuild guild)
        {
            return Database.Query<Guild>($"SELECT * FROM Guild WHERE Id = '{guild.Id}' AND ClientName = '{guild.Client.Name}'").FirstOrDefault();
        }

        public Guild GetGuild(string uniqueId)
        {
            return Database.Query<Guild>($"SELECT * FROM Guild WHERE UniqueId = '{uniqueId}'").FirstOrDefault();
        }

        public string GetGuildUniqueId(ChatGuild guild)
        {
            return Database.Query<Guild>($"SELECT * FROM Guild WHERE Id = '{guild.Id}' AND ClientName = '{guild.Client.Name}'").FirstOrDefault().UniqueId;
        }

        public Category GetGuildCategory(string name, ChatGuild guild)
        {
            return Database.Query<Category>($"SELECT * FROM Category WHERE GuildUniqueId = '{GetGuildUniqueId(guild)}'").FirstOrDefault();
        }

        public Category GetDefaultCategory(ChatGuild guild)
        {
            return Database.Query<Category>($"SELECT * FROM Category WHERE GuildUniqueId = '{GetGuildUniqueId(guild)}' AND Name = 'Default'").FirstOrDefault();
        }

        public void AddDefaultCategory(ChatGuild guild)
        {
            Category category = new Category("Default", guild);            
            Database.Insert(category);
            Database.Insert(new CategoryConfig(category));

            List<BattlePicture> pictures = new List<BattlePicture>();
            string defaultBattlePicturesPath = @$"{AutoPigs.DataPath}{guild.Client.Name}\DefaultBattlePictures";

            if (!Directory.Exists(defaultBattlePicturesPath))
            {
                Directory.CreateDirectory(defaultBattlePicturesPath);
            }

            foreach (string battlePicture in Directory.GetFiles(defaultBattlePicturesPath, "*", SearchOption.TopDirectoryOnly))
            {
                Database.Insert(new BattlePicture(battlePicture, category));
            }
        }

        public CategoryConfig GetCategoryConfig(Category category) 
        {
            return Database.Query<CategoryConfig>($"SELECT * FROM CategoryConfig WHERE CategoryId = {category.Id}").FirstOrDefault();
        } 

        public List<BattlePicture> GetBattlePictures(Category category) 
        {
            return Database.Query<BattlePicture>($"SELECT * FROM BattlePicture WHERE CategoryId = {category.Id}").ToList();
        }


        /*public static Server GetServerByLocalId(string localId)
        {
            return DatabaseHandler.Database.Query<Server>($"SELECT * FROM Server WHERE LocalId = {localId}").SingleOrDefault();
        }
        public static ServerConfig GetServerConfigByServerId(string serverId)
        {
            return DatabaseHandler.Database.Query<ServerConfig>($"SELECT * FROM ServerConfig WHERE ServerId = {serverId}").SingleOrDefault();
        }

        public static string GetLanguageCodeByServerConfig(string serverId)
        {
            ServerConfig config = DatabaseHandler.Database.Query<ServerConfig>($"SELECT * From ServerConfig").SingleOrDefault();
            if (config != null)
            {
                return config.SelectedLanguage;
            }
            else return null;

        }*/


        //


        /*public static ServerConfig GetServerConfig(string serverId)
        {
            foreach (ServerConfig c in
            Database.Query<ServerConfig>($"SELECT * FROM ServerConfig")) 
            {
                Console.WriteLine(c.ServerId);
            }


            ServerConfig config = Database.Query<ServerConfig>($"SELECT * FROM ServerConfig WHERE ServerId = '{serverId}'").SingleOrDefault();
            return config;
        }

        public static string GetServerLanguage(string localId)
        {
            ServerConfig config = GetServerConfig(localId);
            if (config != null)
            {
                return config.SelectedLanguage;
            }
            else return null;
        }*/
    }
}
