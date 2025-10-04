# BlazorServerApp1

Interactive Blazor Server demo featuring two OpenAI-backed agents that can converse individually or in alternating dialogue loops.

## Prerequisites
- [.NET SDK 9.0](https://dotnet.microsoft.com/download)
- OpenAI API key (configured via `appsettings.Development.json` or `OPENAI_API_KEY`)

## Setup
```bash
dotnet restore
```

## Running
```bash
dotnet run
```
App defaults to `http://localhost:5073/`.

## Configuration
Set the OpenAI key in `appsettings.Development.json` under `OpenAI:ApiKey`, or export:
```bash
export OPENAI_API_KEY="your-key"
```

## Key Features
- **Agent 1** / **Agent 2**: Configure name, role, and model from the Conversation page.
- **Prompting**: Send single prompts or let agents alternate for a configurable number of iterations.
- **Live output**: Results stream into a shared conversation panel with basic markup.

## Debugging
Install the VS Code C# extension (`ms-dotnettools.csharp`) and use the provided “Launch Blazor Server” configuration.

## Testing
```bash
dotnet test
```