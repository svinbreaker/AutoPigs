using AutoPigs.Commands;
using AutoPigs.Tables;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using СrossAppBot;
using СrossAppBot.Entities;
using СrossAppBot.Entities.Files;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Runtime.InteropServices.Marshalling.IIUnknownCacheStrategy;

namespace AutoPigs
{
    class DatabaseHandler
    {
        public SQLiteAsyncConnection Database { get; set; }
        private string FilePath;


        public DatabaseHandler(string filePath)
        {
            FilePath = filePath;
            ConnectToDatabase();
        }

        public void ConnectToDatabase()
        {
            if (!System.IO.File.Exists(FilePath))
            {
                var connection = new SQLite.SQLiteConnection(FilePath, SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.ReadWrite);
                connection.CreateTable<Pig>();
                connection.CreateTable<Guild>();
                connection.CreateTable<GuildConfig>();
                connection.CreateTable<BattlePicture>();
                connection.CreateTable<BattleReaction>();
                connection.CreateTable<Category>();
                connection.CreateTable<CategoryConfig>();
                connection.CreateTable<PigsCategories>();
                connection.Close();
            }
            Database = new SQLite.SQLiteAsyncConnection(FilePath);
        }

        public async Task<bool> UserIsPig(ChatUser user, ChatGroup guild)
        {
            //return (await Database.QueryAsync<Pig>($"SELECT * FROM Pig WHERE UserId = '{user.Id}' AND ClientName = '{user.Client.Name}' AND GuildId = '{guild.Id}'")).Count > 0;
            Pig pig = await GetUserAsPig(user, guild);
            return (pig != null);
        }

        public async Task AddPig(string userId, string clientName, string guildId)
        {
            Pig pig = new Pig(userId, clientName, guildId);
            await Database.InsertAsync(pig);
        }

        public async Task AddPig(Pig pig)
        {
            await Database.InsertAsync(pig);
        }

        public async Task RemovePig(Pig pig)
        {
            await Database.DeleteAsync(pig);
        }

        public async Task<Pig> GetUserAsPig(ChatUser user, ChatGroup guild)
        {
            return (await Database.QueryAsync<Pig>($"SELECT * FROM Pig WHERE UserId = '{user.Id}' AND ClientName = '{user.Client.Name}' AND GuildId = '{guild.Id}'")).FirstOrDefault();
        }

        public async Task<List<Pig>> GetPigs(string guildId, string clientName)
        {
            return (await Database.QueryAsync<Pig>($"SELECT * FROM Pig WHERE GuildId = '{guildId}' AND ClientName = '{clientName}'")).ToList();
        }

        public async Task CreateGuildConfig(ChatGroup guild)
        {
            Guild guildObject = new Guild(guild);
            await Database.InsertAsync(guildObject);
            GuildConfig config = new GuildConfig(guildObject);
            await Database.InsertAsync(config);
            await AddDefaultCategory(guild);
        }

        public async Task<bool> GuildConfigExists(ChatGroup guild)
        {
            try
            {
                return (await Database.QueryAsync<GuildConfig>($"SELECT * FROM Guild WHERE Id = '{guild.Id}' AND ClientName = '{guild.Client.Name}'")).Count > 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString() + " " + e.Message);
                return false;
            }
        }

        public async Task<string> GetGuildLanguage(ChatGroup guild)
        {
            GuildConfig config = await GetGuildConfig(guild);
            return config.Language;
        }

        public async Task<GuildConfig> GetGuildConfig(ChatGroup guild)
        {
            string guildUniqueId = await GetGuildUniqueId(guild);
            return (await Database.QueryAsync<GuildConfig>($"SELECT * FROM GuildConfig WHERE GuildUniqueId = '{guildUniqueId}'")).FirstOrDefault();
        }

        public async Task<List<Category>> GetGuildCategories(ChatGroup guild)
        {
            string guildUniqueId = await GetGuildUniqueId(guild);
            return await Database.QueryAsync<Category>($"SELECT * FROM Category WHERE GuildUniqueId = '{guildUniqueId}'");
        }

        public async Task AddGuildCategory(string categoryName, ChatGroup guild)
        {
            Category category = new Category(categoryName, guild);

            await Database.InsertAsync(category);
            CategoryConfig config = new CategoryConfig(category);
            await Database.InsertAsync(config);

        }

        public async Task RemoveGuildCategory(Category category)
        {
            try
            {
                CategoryConfig config = await GetCategoryConfig(category);
                await Database.DeleteAsync(config);
                foreach (Pig pig in await GetPigsOfCategory(category))
                {
                    await RemovePigCategory(pig, category);
                }

                foreach (BattlePicture picture in await GetBattlePictures(category))
                {
                    await RemoveBattlePicture(picture);
                }

                foreach (BattleReaction reaction in await GetBattleEmojis(category))
                {
                    await RemoveBattleReaction(reaction.Emoji, category);
                }

                await Database.DeleteAsync(category);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred while removing guild category: {ex.Message} + " + ex.ToString());
            }
        }

        public async Task SetPigCategory(Pig pig, Category category)
        {
            await Database.InsertAsync(new PigsCategories(pig.Id, category.Id));
        }

        public async Task RemovePigCategory(Pig pig, Category category)
        {
            PigsCategories pigCategory = (await Database.QueryAsync<PigsCategories>($"SELECT * FROM PigsCategories WHERE PigId = {pig.Id} AND CategoryId = {category.Id}")).FirstOrDefault();
            await Database.DeleteAsync(pigCategory);
        }

        public async Task<bool> PigHasCategory(Pig pig, Category category)
        {
            return (await Database.QueryAsync<PigsCategories>($"SELECT * FROM PigsCategories WHERE PigId = {pig.Id} AND CategoryId = {category.Id}")).ToList().Count > 0;
        }

        public async Task<List<Pig>> GetPigsOfCategory(Category category)
        {
            return await Database.QueryAsync<Pig>($"SELECT * FROM PIG JOIN PigsCategories ON Pig.Id = PigsCategories.PigId WHERE PigsCategories.CategoryId = '{category.Id}'");
        }

        public async Task<List<Category>> GetCategoriesOfPig(Pig pig)
        {
            return await Database.QueryAsync<Category>($"SELECT * FROM Category JOIN PigsCategories ON Category.Id = PigsCategories.CategoryId WHERE PigsCategories.PigId = {pig.Id}");
        }

        public async Task AddBattlePicture(ChatPicture picture, Category category, ChatGroup guild)
        {
            string pictureLocation = null;
            try
            {
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

                await Database.InsertAsync(new BattlePicture(pictureLocation, category));
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Exception occurred while adding battle picture: {exception.Message}");
            }
        }

        public async Task RemoveBattlePicture(BattlePicture picture)
        {
            await Task.Run(() => File.Delete(picture.FileLocation));
            await Database.DeleteAsync(picture);
        }

        public async Task ClearBattlePictures(Category category)
        {
            List<BattlePicture> pictures = await GetBattlePictures(category);

            try
            {
                foreach (BattlePicture picture in pictures)
                {
                    await RemoveBattlePicture(picture);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred while clearing battle pictures: {ex.Message}");
            }

        }

        public async Task AddBattleReaction(string emoji, Category category)
        {
            await Database.InsertAsync(new BattleReaction(emoji, category));
        }

        public async Task RemoveBattleReaction(string emoji, Category category)
        {
            List<BattleReaction> emojis = await GetBattleEmojis(category);
            BattleReaction emojiToDelete = emojis.Find(e => e.Emoji == emoji);
            if (emojiToDelete == null)
            {
                throw new ArgumentException("Entered string is not emoji");
            }

            await Database.DeleteAsync(emojiToDelete);

        }

        public async Task<Guild> GetGuild(ChatGroup guild)
        {
            return (await Database.QueryAsync<Guild>($"SELECT * FROM Guild WHERE Id = '{guild.Id}' AND ClientName = '{guild.Client.Name}'")).FirstOrDefault();
        }

        public async Task<Guild> GetGuild(string uniqueId)
        {
            return (await Database.QueryAsync<Guild>($"SELECT * FROM Guild WHERE UniqueId = '{uniqueId}'")).FirstOrDefault();
        }

        public async Task<string> GetGuildUniqueId(ChatGroup guild)
        {
            string uniqueId = (await Database.QueryAsync<Guild>($"SELECT * FROM Guild WHERE Id = '{guild.Id}' AND ClientName = '{guild.Client.Name}'")).FirstOrDefault().UniqueId;
            return uniqueId;
        }

        public async Task<Category> GetGuildCategory(string name, ChatGroup guild)
        {
            string uniqueId = (await Database.QueryAsync<Guild>($"SELECT * FROM Guild WHERE Id = '{guild.Id}' AND ClientName = '{guild.Client.Name}'")).FirstOrDefault().UniqueId;
            return (await Database.QueryAsync<Category>($"SELECT * FROM Category WHERE GuildUniqueId = '{uniqueId}'")).FirstOrDefault();
        }

        public async Task<Category> GetDefaultCategory(ChatGroup guild)
        {
            string uniqueId = (await Database.QueryAsync<Guild>($"SELECT * FROM Guild WHERE Id = '{guild.Id}' AND ClientName = '{guild.Client.Name}'")).FirstOrDefault().UniqueId;
            return (await Database.QueryAsync<Category>($"SELECT * FROM Category WHERE GuildUniqueId = '{uniqueId}' AND Name = 'Default'")).FirstOrDefault();
        }

        public async Task AddDefaultCategory(ChatGroup guild)
        {
            Category category = new Category("Default", guild);
            List<BattlePicture> pictures = new List<BattlePicture>();
            string defaultBattlePicturesPath = @$"{AutoPigs.DataPath}{guild.Client.Name}\DefaultBattlePictures";
            if (!Directory.Exists(defaultBattlePicturesPath))
            {
                Directory.CreateDirectory(defaultBattlePicturesPath);
            }

            try
            {
                await Database.InsertAsync(category);
                await Database.InsertAsync(new CategoryConfig(category));
                if (guild.Client is IAddReaction && guild.Client.Name != "Vk")
                {
                    await Database.InsertAsync(new BattleReaction("🐷", category));
                }
                foreach (string battlePicture in Directory.GetFiles(defaultBattlePicturesPath, "*", SearchOption.TopDirectoryOnly))
                {
                    await Database.InsertAsync(new BattlePicture(battlePicture, category));
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Exception occurred while creating default category: {ex.Message}");
            }


        }

        public async Task<CategoryConfig> GetCategoryConfig(Category category)
        {
            CategoryConfig config = (await Database.QueryAsync<CategoryConfig>($"SELECT * FROM CategoryConfig WHERE CategoryId = {category.Id}")).FirstOrDefault();
            return config;
        }

        public async Task<List<BattlePicture>> GetBattlePictures(Category category)
        {
            return (await Database.QueryAsync<BattlePicture>($"SELECT * FROM BattlePicture WHERE CategoryId = {category.Id}")).ToList();
        }

        public async Task<List<BattleReaction>> GetBattleEmojis(Category category)
        {
            return (await Database.QueryAsync<BattleReaction>($"SELECT * FROM BattleReaction WHERE CategoryId = {category.Id}")).ToList();
        }
    }
}
