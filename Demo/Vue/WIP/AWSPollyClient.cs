using Amazon.Polly;
using Amazon.Polly.Model;
using Amazon.Runtime;
using Microsoft.Extensions.Options;

namespace ChatBot;

public interface IAwsPollyAgent
{
    bool HasCredentials { get; }

    Task<Stream> TextToSpeechStreamAsync(string text, CancellationToken cancellationToken = default);

    Task<string> TextToSpeechFileAsync(string text, CancellationToken cancellationToken = default);
}

public class AwsPollyAgent : IAwsPollyAgent
{
    private readonly AwsCredentials _awsCredentials;

    public bool HasCredentials => !string.IsNullOrWhiteSpace(_awsCredentials?.SecretAccessKey);

    public AwsPollyAgent(IOptions<AwsCredentials> awsCredentialsOptions)
    {
        _awsCredentials = awsCredentialsOptions.Value;
    }

    public async Task<Stream> TextToSpeechStreamAsync(string text, CancellationToken cancellationToken = default)
    {
        var credentials = new BasicAWSCredentials(_awsCredentials.AccessKeyId, _awsCredentials.SecretAccessKey);

        using var client = new AmazonPollyClient(credentials, _awsCredentials.Region);
        var synthesizeSpeechRequest = new SynthesizeSpeechRequest()
        {
            OutputFormat = OutputFormat.Mp3,
            VoiceId = VoiceId.Joanna,
            Text = text,
        };

        var synthesizeSpeechResponse = await client.SynthesizeSpeechAsync(synthesizeSpeechRequest, cancellationToken);
        return synthesizeSpeechResponse.AudioStream;
    }

    public async Task<string> TextToSpeechFileAsync(string text, CancellationToken cancellationToken = default)
    {
        var audioStream = await TextToSpeechStreamAsync(text, cancellationToken);

        var fileName = Path.ChangeExtension(Path.GetTempFileName(), "mp3");
        var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);

        try
        {
            int bufferSize = 2048;
            byte[] buffer = new byte[bufferSize];
            int readBytes;

            while ((readBytes = audioStream.Read(buffer, 0, bufferSize)) > 0)
                fileStream.Write(buffer, 0, readBytes);

            await fileStream.FlushAsync();
        }
        finally
        {
            fileStream.Close();
            await fileStream.DisposeAsync();
        }
                
        return fileName;
    }
}

using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;

namespace ChatBot;

public interface IElevenLabsAgent
{
    bool HasCredentials { get; }

    Task<string> TextToSpeechFileAsync(string text, CancellationToken cancellationToken = default);
}

public class ElevenLabsAgent : IElevenLabsAgent
{
    private const string _baseUrl = "https://api.elevenlabs.io/v1";

    //private const string _voiceId = "EXAVITQu4vr4xnSDxMaL"; // Bella
    private const string _voiceId = "0Zq9S9mzBJRnESqp6yaM"; // Nat

    private readonly HttpClient _httpClient;
    private readonly ElevenLabsCredentials _credentials;

    private class TextToSpeechPayload
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;

        [JsonPropertyName("voice_settings")]
        public VoiceSettings? VoiceSettings { get; set; }
    }

    private class VoiceSettings
    {
        [JsonPropertyName("stability")]
        public float Stability { get; set; }

        [JsonPropertyName("similarity_boost")]
        public float SimilarityBoost { get; set; }
    }

    public bool HasCredentials => !string.IsNullOrWhiteSpace(_credentials?.ApiKey);

    public ElevenLabsAgent(IOptions<ElevenLabsCredentials> credentialsOptions, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _credentials = credentialsOptions.Value;

        _httpClient.DefaultRequestHeaders.Add("xi-api-key", _credentials.ApiKey);
    }

    public async Task<string> TextToSpeechFileAsync(string text, CancellationToken cancellationToken = default)
    {
        var payload = new TextToSpeechPayload { Text = text, VoiceSettings = new VoiceSettings { Stability = 0.2f, SimilarityBoost = 0.8f } };
        var content = JsonContent.Create(payload);

        var response = await _httpClient.PostAsync($"{_baseUrl}/text-to-speech/{_voiceId}", content, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var responseAsString = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"TextToSpeechFileAsync failed with {response.StatusCode}: {responseAsString}", null, response.StatusCode);
        }

        var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var fileName = Path.ChangeExtension(Path.GetTempFileName(), "mp3");
        try
        {
            var fileStream = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write, FileShare.None);

            try
            {
                await responseStream.CopyToAsync(fileStream, cancellationToken);
                await fileStream.FlushAsync(cancellationToken);
            }
            finally
            {
                fileStream.Close();
                await fileStream.DisposeAsync();
            }
        }
        finally
        {
            await responseStream.DisposeAsync();
        }

        return fileName;
    }
}

using Microsoft.Extensions.Options;
using OpenAI_API;
using OpenAI_API.Chat;

namespace ChatBot
{
    public interface IOpenAIAgent
    {
        bool HasCredentials { get; }

        IChatSession CreateChatSession(string systemMessage, Dictionary<string, string> examplePromptsAndResponses);

        Task<string> CreateCompletionAsync(string prompt);
    }

    public class OpenAIAgent : IOpenAIAgent
    {
        private readonly OpenAICredentials _credentials;

        public class ChatSession : IChatSession
        {
            private readonly Conversation _conversation;

            public ChatSession(Conversation conversation)
            {
                _conversation = conversation;
            }

            public async Task<string> GetResponseAsync(string prompt)
            {
                _conversation.AppendUserInput(prompt);
                return await _conversation.GetResponseFromChatbotAsync();
            }
        }

        public bool HasCredentials => !string.IsNullOrWhiteSpace(_credentials?.ApiKey);

        public OpenAIAgent(IOptions<OpenAICredentials> credentialsOptions)
        {
            _credentials = credentialsOptions.Value;
        }

        public IChatSession CreateChatSession(string systemMessage, Dictionary<string, string> examplePromptsAndResponses)
        {
            var openApi = new OpenAIAPI(_credentials.ApiKey);
            var conversation = openApi.Chat.CreateConversation();

            conversation.AppendSystemMessage(systemMessage);

            foreach ((string prompt, string response) in examplePromptsAndResponses)
            {
                conversation.AppendUserInput(prompt);
                conversation.AppendExampleChatbotOutput(response);
            }

            return new ChatSession(conversation);
        }

        public async Task<string> CreateCompletionAsync(string prompt)
        {
            var openApi = new OpenAIAPI(_credentials.ApiKey);
            var result = await openApi.Completions.CreateCompletionAsync(prompt);
            return result.ToString();
        }
    }
}

using ChatBot;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.local.json");

var services = builder.Services;

services.AddHttpClient<IElevenLabsAgent, ElevenLabsAgent>();
services.AddHttpClient<IConfluenceAgent, ConfluenceAgent>();

services.AddOptions<AwsCredentials>().Configure<IConfiguration>((options, configuration) => configuration.Bind(AwsCredentials.DefaultConfigurationKey, options));
services.AddOptions<OpenAICredentials>().Configure<IConfiguration>((options, configuration) => configuration.Bind(OpenAICredentials.DefaultConfigurationKey, options));
services.AddOptions<ElevenLabsCredentials>().Configure<IConfiguration>((options, configuration) => configuration.Bind(ElevenLabsCredentials.DefaultConfigurationKey, options));

services
    .AddSingleton<IOpenAIAgent, OpenAIAgent>()
    .AddSingleton<IAwsPollyAgent, AwsPollyAgent>()
    .AddSingleton<IElevenLabsAgent, ElevenLabsAgent>()
    .AddSingleton<IConfluenceAgent, ConfluenceAgent>();

services.AddSingleton<IChatBot, ChatBot.ChatBot>();

var app = builder.Build();

app.MapPost("/prompt", async (HttpRequest request, IChatBot chatBot) =>
{
    var prompt = await new StreamReader(request.Body).ReadToEndAsync();
    return await chatBot.GetResponseAsync(prompt);
});

app.UseStaticFiles();
app.MapFallbackToFile("index.html");
app.Run();
