﻿
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Content.Server.GameTicking
{
    public partial class GameTicker
    {
        private readonly HttpClient _httpClient = new();

        private async void SendDiscordNewRoundAlert()
        {
            var sendsWebhook = DiscordWebhook != string.Empty;
            if (sendsWebhook)
            {
                WebhookPayload payload;

                if (DiscordRoleId != string.Empty)
                {
                    payload = new WebhookPayload()
                    {
                        Content = $"<@&{DiscordRoleId}> {Loc.GetString("discord-round-new")}",
                        AllowedMentions = new Dictionary<string, string[]>
                        {
                            { "roles", new []{ DiscordRoleId } }
                        },
                    };
                }
                else
                {
                    payload = new WebhookPayload()
                    {
                        Content = Loc.GetString("discord-round-new"),
                    };
                }

                var request = await _httpClient.PostAsync(DiscordWebhook,
                    new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));

                var content = await request.Content.ReadAsStringAsync();
                if (!request.IsSuccessStatusCode)
                {
                    Logger.ErrorS("ticker", $"Discord returned bad status code when posting message: {request.StatusCode}\nResponse: {content}");
                    return;
                }
            }
        }

        private struct WebhookPayload
        {
            [JsonPropertyName("content")]
            public string Content { get; set; } = "";

            [JsonPropertyName("allowed_mentions")]
            public Dictionary<string, string[]> AllowedMentions { get; set; } =
                new()
                {
                    { "parse", Array.Empty<string>() }
                };
        }
    }
}