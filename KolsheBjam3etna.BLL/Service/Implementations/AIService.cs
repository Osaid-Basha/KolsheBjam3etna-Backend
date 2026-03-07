using KolsheBjam3etna.BLL.Service.Interface;
using KolsheBjam3etna.DAL.DTOs.Request;
using KolsheBjam3etna.DAL.DTOs.Response;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace KolsheBjam3etna.BLL.Service.Implementations
{
    public class AIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public AIService(
            HttpClient httpClient,
            IConfiguration configuration,
            IWebHostEnvironment env)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _env = env;
        }

        public async Task<ChatResponse> AskAsync(ChatRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Message))
            {
                return new ChatResponse
                {
                    Reply = "الرجاء كتابة سؤالك أولاً."
                };
            }

            var apiKey = _configuration["Gemini:ApiKey"];
            var model = _configuration["Gemini:Model"] ?? "gemini-2.5-flash";

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return new ChatResponse
                {
                    Reply = "مفتاح Gemini غير موجود في الإعدادات."
                };
            }

            var knowledgePath = Path.Combine(_env.ContentRootPath, "Data", "app-knowledge.json");
            var appKnowledge = File.Exists(knowledgePath)
                ? await File.ReadAllTextAsync(knowledgePath)
                : "{}";

            var systemPrompt = $"""
You are the AI assistant for the "Kolshi Bjam3etna" university mobile app.

Your job:
- Help students understand and use the app
- Answer only based on the app knowledge below
- Keep answers short, practical, and friendly
- If the answer is not found in the app knowledge, say clearly that your help is limited to app-related information

App knowledge:
{appKnowledge}
""";

            var fullPrompt = $"""
{systemPrompt}

User question:
{request.Message}
""";

            var payload = new
            {
                contents = new object[]
                {
                    new
                    {
                        parts = new object[]
                        {
                            new { text = fullPrompt }
                        }
                    }
                }
            };

            var url =
                $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);
            httpRequest.Content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.SendAsync(httpRequest);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new ChatResponse
                {
                    Reply = $"صار خطأ أثناء الاتصال بالذكاء الاصطناعي: {response.StatusCode} - {responseBody}"
                };
            }

            try
            {
                using var jsonDoc = JsonDocument.Parse(responseBody);

                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("candidates", out var candidates) &&
                    candidates.ValueKind == JsonValueKind.Array &&
                    candidates.GetArrayLength() > 0)
                {
                    var firstCandidate = candidates[0];

                    if (firstCandidate.TryGetProperty("content", out var content) &&
                        content.TryGetProperty("parts", out var parts) &&
                        parts.ValueKind == JsonValueKind.Array &&
                        parts.GetArrayLength() > 0)
                    {
                        var firstPart = parts[0];

                        if (firstPart.TryGetProperty("text", out var textElement))
                        {
                            var replyText = textElement.GetString();

                            return new ChatResponse
                            {
                                Reply = string.IsNullOrWhiteSpace(replyText)
                                    ? "لم أتمكن من توليد رد."
                                    : replyText
                            };
                        }
                    }
                }

                return new ChatResponse
                {
                    Reply = "لم أتمكن من استخراج الرد من Gemini."
                };
            }
            catch (Exception ex)
            {
                return new ChatResponse
                {
                    Reply = $"تم استلام رد لكن فشل تحليله: {ex.Message}"
                };
            }
        }
    }
}

