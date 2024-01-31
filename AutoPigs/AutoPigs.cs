using AutoPigs.Commands;
using AutoPigs.Commands.ArgumentParsers;
using AutoPigs.Commands.Configuration;
using AutoPigs.Commands.General;
using AutoPigs.Commands.Pigs;
using AutoPigs.Commands.Pigs.Categories;
using BulbulatorLocalization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VkNet.Model;
using СrossAppBot;
using СrossAppBot.Commands;
using СrossAppBot.Commands.Parameters;
using СrossAppBot.Entities;
using СrossAppBot.Entities.Files;

namespace AutoPigs
{
    class AutoPigs
    {
        public static List<AbstractBotClient> bots { get; set; }
        public static string ProjectPath
        {
            get
            {
                return System.AppDomain.CurrentDomain.BaseDirectory.Replace(@"\bin\Debug\net8.0", "");
            }
        }

        public static DatabaseHandler DatabaseHandler
        {
            get
            {
                return new DatabaseHandler((AutoPigs.ProjectPath) + "Data/database.db3");
            }

        }

        public static Localizer Localizer
        {
            get
            {
                return new Localizer(Directory.GetFiles($@"{ProjectPath}\Localization\Resources", "*.json").ToList());
            }
        }

        public static string DataPath
        {
            get
            {
                return ProjectPath + @"Data\";
            }
        }

        private static string _botDataFile = DataPath + $@"tokens.json";

        public static Task Main(string[] args) => new AutoPigs().MainAsync();

        async Task MainAsync()
        {
            dynamic botsData = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(_botDataFile));

            /*DiscordBotClient discordBot = new DiscordBotClient("MTE4NDY4Mzg4NDcyNjcyMjY1MA.GBNxlL.giM6svLjvDIzG4ZZxslBxcNwDNc6gYIpH40R1w");
            TelegramBotClient telegramBot = new TelegramBotClient("6342451970:AAE-eVaK-nbkJ7FUmipVCxNhmJWHzm-Fh_w");
            VkBotClient vkBot = new VkBotClient("vk1.a.xC_UrO3rEUnqig0QE4XU0d7qMi-GU0LaGl2b8KDBpzBhW476NXJ4KIgddJ2qQ9sQ43O0pT9-_JKLjqfU65d4mufk5GeaEGJS9EVUlr4WcBpEEyu8lgd7WR_SpJac8dl2ssIpEte7PV3nH_DiKoq_C5JHGW4ZdI9NoAtWBVDpXh92SjJUpgb8xmIO2zmF40feqGarO3ICa1AX7C4yJxmxWA", 214675765);*/

            DiscordBotClient discordBot = new DiscordBotClient(Convert.ToString(botsData.discord.token));
            TelegramBotClient telegramBot = new TelegramBotClient(Convert.ToString(botsData.telegram.token));
            VkBotClient vkBot = new VkBotClient(Convert.ToString(botsData.vk.token), ulong.Parse(Convert.ToString(botsData.vk.groupId)));
            List<AbstractBotClient> bots = new List<AbstractBotClient> { vkBot };

            List<AbstractCommand> universalCommands = new List<AbstractCommand>
            {
                new HelpCommand(), new TestCommand(), new SetLanguageCommand(),
                new PigsAddCommand(), new PigsRemoveCommand(), new PigsListCommand(),
                new CreateCategoryCommand(), new DeleteCategoryCommand(), new CategoriesListCommand(),
                new SetPigCategoryCommand(), new RemovePigCategory(),
                new AddBattlePictureCommand(), new ClearBattlePicturesCommand(),
                new SetPictureChanceCommand()
            };
            List<AbstractCommand> emojiCommands = new List<AbstractCommand> { new SetReactionChanceCommand(), new SetDefaultEmojiCommand() };

            List<IArgumentParser> parsers = new List<IArgumentParser>() { new PigArgumentParser(), new CategoryArgumentParser() };
            foreach (AbstractBotClient bot in bots)
            {
                bot.OnMessageReceived += OnMessageReceived;
                bot.TextCommandProcessor = new TextCommandProcessor("p.", universalCommands, parsers);

                if (bot is IAddReaction)
                {
                    bot.TextCommandProcessor.AddCommands(emojiCommands);
                }
                try
                {
                    await bot.StartAsync();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.ToString());
                    await bot.StartAsync();
                }
            }

            await Task.Delay(-1);
        }

        private static void StartBot(AbstractBotClient bot)
        {
            bot.StartAsync();
        }

        private static async Task OnMessageReceived(ChatMessage message)
        {
            AbstractBotClient client = message.Client;

            ChatGuild guild = message.Guild;
            await Task.Run(() =>
            {
                if (!DatabaseHandler.GuildConfigExists(guild))
                {
                    try
                    {
                        DatabaseHandler.CreateGuildConfig(guild);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception.ToString());
                    }
                }
            });
            await CheckPigs(message);

            await client.TextCommandProcessor.ProcessCommand(message.Text, message.GetAsCommandContext());
        }

        public static Task onMessageEdited(ChatMessage message)
        {
            return Task.CompletedTask;
        }

        private static async Task CheckPigs(ChatMessage message)
        {
            ChatGuild guild = message.Guild;
            AbstractBotClient client = message.Client;
            ChatUser author = message.Author;

            if (DatabaseHandler.UserIsPig(author, guild))
            {
                Pig pig = DatabaseHandler.GetUserAsPig(author, guild);
                GuildConfig config = DatabaseHandler.GetGuildConfig(guild);

                List<Category> categories = DatabaseHandler.GetCategoriesOfPig(pig);
                Category category = null;
                Random random = new Random();

                if (categories.Count > 0)
                {
                    if (categories.Count == 1)
                    {
                        category = categories[0];
                    }
                    else
                    {
                        category = categories[random.Next(0, categories.Count - 1)];
                    }


                    BattlePicture picture;
                    CategoryConfig categoryConfig = DatabaseHandler.GetCategoryConfig(category);
                    int battlePictureChance = categoryConfig.PictureChance;
                    if (battlePictureChance > 0)
                    {
                        if (random.Next(100) <= battlePictureChance)
                        {
                            List<BattlePicture> pictures = DatabaseHandler.GetBattlePictures(category);
                            if (pictures.Count > 0)
                            {
                                if (pictures.Count == 1)
                                {
                                    picture = pictures[0];
                                }
                                else
                                {
                                    picture = pictures[random.Next(0, pictures.Count - 1)];
                                }
                                List<string> files = new List<string>() { picture.FileLocation };
                                await client.SendMessageAsync(message.Channel.Id, text: null, messageReferenceId: message.Id, files: files);
                            }
                        }
                    }

                    if (client is IAddReaction reactableClient)
                    {
                        foreach (Category c in categories)
                        {
                            CategoryConfig cConfig = DatabaseHandler.GetCategoryConfig(c);
                            string emoji = cConfig.ReactionEmoji;
                            int reactionChance = cConfig.ReactionChance;

                            if (emoji != null && reactionChance > 0)
                            {
                                if (random.Next(100) <= reactionChance)
                                {
                                    try
                                    {
                                        await reactableClient.AddReaction(message, emoji);
                                    }
                                    catch (Exception exception)
                                    {
                                        Console.WriteLine(exception.ToString());
                                    }
                                }
                            }
                        }
                    }
                }

            }
        }
    }
}

