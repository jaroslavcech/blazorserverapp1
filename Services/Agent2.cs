using System.Text;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Responses;

namespace BlazorServerApp1.Services;

public sealed class Agent2
{
    private readonly OpenAIClient _openAIClient;

    private string _agentName = "Agent2";
    private string _systemPrompt = "You are a helpful assistant within a Blazor Server application.";
    private string _model = "gpt-4o-mini";

    public Agent2(OpenAIClient openAIClient)
    {
        _openAIClient = openAIClient ?? throw new ArgumentNullException(nameof(openAIClient));
    }

    public void RoleAndName(string name, string systemPrompt, string model)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Agent name must not be null or whitespace.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(systemPrompt))
        {
            throw new ArgumentException("System prompt must not be null or whitespace.", nameof(systemPrompt));
        }

        if (string.IsNullOrWhiteSpace(model))
        {
            throw new ArgumentException("Model must not be null or whitespace.", nameof(model));
        }

        _agentName = name;
        _systemPrompt = systemPrompt;
        _model = model;
    }

    public async Task<string> Chat1(string prompt)
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            throw new ArgumentException("Prompt must not be null or whitespace.", nameof(prompt));
        }

        var chatClient = _openAIClient.GetChatClient(_model);

        var options = new ChatCompletionOptions
        {
            MaxOutputTokenCount = 512,
            ResponseFormat = ChatResponseFormat.CreateTextFormat()
        };

        ChatCompletion completion = await chatClient.CompleteChatAsync(
            new ChatMessage[]
            {
                new SystemChatMessage(_systemPrompt),
                new UserChatMessage(prompt),
            },
            options
        );

        var responseText = ExtractText(completion).Trim();

        return $"**{_agentName}**: {responseText}";
    }

    private static string ExtractText(ChatCompletion completion)
    {
        if (completion is null)
        {
            return string.Empty;
        }

        var builder = new StringBuilder();

        foreach (var contentItem in completion.Content)
        {
            if (contentItem.Kind != ChatMessageContentPartKind.Text)
            {
                continue;
            }

            if (!string.IsNullOrEmpty(contentItem.Text))
            {
                builder.Append(contentItem.Text);
            }
        }

        return builder.ToString();
    }
}
