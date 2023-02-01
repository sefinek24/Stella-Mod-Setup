using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using JNogueira.Discord.Webhook.Client;

namespace Genshin_Impact_MP_Installer.Scripts
{
    internal abstract class WebHook
    {
        public static async void Installing()
        {
            var client = new DiscordWebhookClient(Config.MainWebHookUrl);
            var message = new DiscordMessage(embeds: new[]
            {
                new DiscordMessageEmbed(
                    color: 5744610,
                    author: new DiscordMessageEmbedAuthor($"📥 {Os.Region}: Installing mod on new machine..."),
                    fields: new[]
                    {
                        new DiscordMessageEmbedField("» OS", $"> {Os.Name} {Os.Version}", true),
                        new DiscordMessageEmbedField("» Build", $"> {Os.Build}", true),
                        new DiscordMessageEmbedField("» Setup", $"> v{Program.AppVersion}", true)
                    },
                    footer: new DiscordMessageEmbedFooter($"📅 {DateTime.Now:HH:mm:ss, dd.MM.yyyy} • {Os.TimeZone}")
                )
            });

            try
            {
                await client.SendToDiscord(message);
            }
            catch (Exception ex)
            {
                Log.ErrorAuditLog(new Exception($"• Telemetry error:\n{ex}"), false);
            }
        }

        public static async void Installed()
        {
            var client = new DiscordWebhookClient(Config.MainWebHookUrl);
            var message = new DiscordMessage(embeds: new[]
            {
                new DiscordMessageEmbed(
                    color: 1492492,
                    author: new DiscordMessageEmbedAuthor($"✅ {Os.Region}: Successfully installed on new PC"),
                    fields: new[]
                    {
                        new DiscordMessageEmbedField("» OS", $"> {Os.Name} {Os.Version}", true),
                        new DiscordMessageEmbedField("» Build", $"> {Os.Build}", true),
                        new DiscordMessageEmbedField("» Setup", $"> v{Program.AppVersion}", true)
                    },
                    footer: new DiscordMessageEmbedFooter($"📅 {DateTime.Now:HH:mm:ss, dd.MM.yyyy} • {Os.TimeZone}")
                )
            });

            try
            {
                await client.SendToDiscord(message);
            }
            catch (Exception ex)
            {
                Log.ErrorAuditLog(new Exception($"• Telemetry error:\n{ex}"), false);
            }
        }

        // -------------------- VCLibs --------------------
        public static async void InstalledVcLibs()
        {
            var client = new DiscordWebhookClient(Config.MainWebHookUrl);
            var message = new DiscordMessage(embeds: new[]
            {
                new DiscordMessageEmbed(
                    color: 2263772,
                    author: new DiscordMessageEmbedAuthor($"🌠 Installed VcLibs on {Os.Name} {Os.Build}"),
                    footer: new DiscordMessageEmbedFooter(
                        $"📅 {DateTime.Now:HH:mm:ss, dd.MM.yyyy} • {Os.TimeZone} 🏠 {Os.Region}")
                )
            });

            try
            {
                await client.SendToDiscord(message);
            }
            catch (Exception ex)
            {
                Log.ErrorAuditLog(new Exception($"• Telemetry error:\n{ex}"), false);
            }
        }

        // -------------------- Reboot --------------------
        public static async void RebootIsRequired()
        {
            var client = new DiscordWebhookClient(Config.MainWebHookUrl);
            var message = new DiscordMessage(embeds: new[]
            {
                new DiscordMessageEmbed(
                    color: 4868682,
                    author: new DiscordMessageEmbedAuthor($"💻 Reboot is required on {Os.Name} {Os.Build}"),
                    footer: new DiscordMessageEmbedFooter(
                        $"📅 {DateTime.Now:HH:mm:ss, dd.MM.yyyy} • {Os.TimeZone} • {Os.Region}")
                )
            });

            try
            {
                await client.SendToDiscord(message);
            }
            catch (Exception ex)
            {
                Log.ErrorAuditLog(new Exception($"• Telemetry error:\n{ex}"), false);
            }
        }

        public static async void RebootIsScheduled()
        {
            var client = new DiscordWebhookClient(Config.MainWebHookUrl);
            var message = new DiscordMessage(embeds: new[]
            {
                new DiscordMessageEmbed(
                    color: 3065320,
                    author: new DiscordMessageEmbedAuthor($"✨ Reboot was scheduled on {Os.Name} {Os.Build}"),
                    footer: new DiscordMessageEmbedFooter(
                        $"📅 {DateTime.Now:HH:mm:ss, dd.MM.yyyy} • {Os.TimeZone} • {Os.Region}")
                )
            });

            try
            {
                await client.SendToDiscord(message);
            }
            catch (Exception ex)
            {
                Log.ErrorAuditLog(new Exception($"• Telemetry error:\n{ex}"), false);
            }
        }

        // -------------------- Error --------------------
        public static async void Error(Exception ex1)
        {
            var client = new DiscordWebhookClient(Config.MainWebHookUrl);
            var message = new DiscordMessage(embeds: new[]
            {
                new DiscordMessageEmbed(
                    color: 15743511,
                    author: new DiscordMessageEmbedAuthor(
                        $"❌ {Os.Region}: An error occurred during installation"),
                    description: $"```js\n{ex1.ToString().Replace("System.Exception: ", "")}```",
                    fields: new[]
                    {
                        new DiscordMessageEmbedField("» OS", $"> {Os.Name} {Os.Version}", true),
                        new DiscordMessageEmbedField("» Build", $"> {Os.Build}", true),
                        new DiscordMessageEmbedField("» Setup", $"> v{Program.AppVersion}", true)
                    },
                    footer: new DiscordMessageEmbedFooter(
                        $"📅 {DateTime.Now:HH:mm:ss, dd.MM.yyyy} • {Os.TimeZone}")
                )
            });

            try
            {
                await client.SendToDiscord(message);
            }
            catch (Exception ex2)
            {
                Log.ErrorAuditLog(new Exception($"• Telemetry error:\n{ex2}"), false);
            }
        }

        // -------------------- Send log files --------------------
        public static async Task<bool> SendLogFiles()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("Sending... ");

            try
            {
                var client = new DiscordWebhookClient(Config.LogsWebHookUrl);

                // Content
                var message =
                    new DiscordMessage(
                        $"\\💻 {Os.AllInfos}\n\\🏠 {Os.Region}\n\\📆 {DateTime.Now:HH:mm:ss, dd.MM.yyyy} • {Os.TimeZone}");

                // Files
                var content1 = "File installer.output.log is empty.";
                if (File.Exists(Log.OutputFile)) content1 = File.ReadAllText(Log.OutputFile);
                var attachment1 = new DiscordFile("installer.output.log", Encoding.UTF8.GetBytes(content1));

                var content2 = "File installer.error.log is empty.";
                if (File.Exists(Log.ErrorFile)) content2 = File.ReadAllText(Log.ErrorFile);
                var attachment2 = new DiscordFile("installer.error.log", Encoding.UTF8.GetBytes(content2));

                var content3 = "File mod_installation.log is empty.";
                if (File.Exists(Log.ModInstFile)) content3 = File.ReadAllText(Log.ModInstFile);
                var gitLog = new DiscordFile("mod_installation.log", Encoding.UTF8.GetBytes(content3));

                // Send
                await client.SendToDiscord(message, new[] { attachment1, attachment2, gitLog });

                // Debug log
                Log.Output("Log files was sent to developer.");

                // Console
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Done");
                Console.ResetColor();
                return true;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error\n");
                Console.ResetColor();

                Log.Error(ex, false);
                return false;
            }
        }
    }
}