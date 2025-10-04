using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorServerApp1.Services;

public sealed class ChatDispatcher
{
    public delegate Task ChatMessageHandler(string message);

    public event ChatMessageHandler? MessageReceived;

    private readonly Agent1 _agent1;
    private readonly Agent2 _agent2;

    public ChatDispatcher(Agent1 agent1, Agent2 agent2)
    {
        _agent1 = agent1 ?? throw new ArgumentNullException(nameof(agent1));
        _agent2 = agent2 ?? throw new ArgumentNullException(nameof(agent2));
    }

    public async Task<IReadOnlyList<string>> ChatDispatch(string prompt, string agent, int nrOfIterations)
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            throw new ArgumentException("Prompt cannot be empty.", nameof(prompt));
        }

        if (string.IsNullOrWhiteSpace(agent))
        {
            throw new ArgumentException("Agent cannot be empty.", nameof(agent));
        }

        if (nrOfIterations < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(nrOfIterations), "Number of iterations must be at least 1.");
        }

        var responses = new List<string>(nrOfIterations);
        var currentPrompt = prompt;
        var currentAgent = ResolveStartingAgent(agent);

        for (var iteration = 0; iteration < nrOfIterations; iteration++)
        {
            var response = currentAgent switch
            {
                AgentSelector.Agent1 => await _agent1.Chat1(currentPrompt),
                AgentSelector.Agent2 => await _agent2.Chat1(currentPrompt),
                _ => throw new InvalidOperationException("Unknown agent selector.")
            };

            responses.Add(response);
            await NotifyMessageAsync(response);

            currentPrompt = PrepareNextPrompt(response);
            currentAgent = Flip(currentAgent);
        }

        return responses;
    }

    private async Task NotifyMessageAsync(string message)
    {
        if (MessageReceived is null)
        {
            return;
        }

        foreach (var handler in MessageReceived.GetInvocationList())
        {
            if (handler is ChatMessageHandler typedHandler)
            {
                await typedHandler.Invoke(message);
            }
        }
    }

    private static AgentSelector ResolveStartingAgent(string agent) =>
        agent.Equals("agent1", StringComparison.OrdinalIgnoreCase)
            ? AgentSelector.Agent1
            : agent.Equals("agent2", StringComparison.OrdinalIgnoreCase)
                ? AgentSelector.Agent2
                : throw new ArgumentException("Agent must be either 'agent1' or 'agent2'.", nameof(agent));

    private static AgentSelector Flip(AgentSelector current) =>
        current == AgentSelector.Agent1 ? AgentSelector.Agent2 : AgentSelector.Agent1;

    private static string PrepareNextPrompt(string response)
    {
        if (string.IsNullOrWhiteSpace(response))
        {
            return string.Empty;
        }

        var span = response.AsSpan();
        var colonIndex = span.IndexOf(':');
        if (colonIndex >= 0 && colonIndex + 1 < span.Length)
        {
            span = span[(colonIndex + 1)..];
        }

        var cleaned = span.ToString().Replace("**", string.Empty, StringComparison.Ordinal);
        return cleaned.Trim();
    }

    private enum AgentSelector
    {
        Agent1,
        Agent2
    }
}