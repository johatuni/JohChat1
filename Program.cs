using System.ClientModel;
using System.Text;
using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;

// Ollama
var ollamaUri = new Uri("http://localhost:11434");
string[] ollamaModels = ["llama3.1", "phi4", "deepseek-r1:8b"];
var chatClient = new OllamaChatClient(ollamaUri, ollamaModels[0]);

// Azure OpenAI
//var apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_KEY");
//var apiUri = Environment.GetEnvironmentVariable("AZURE_OPENAI_URI");
//var apiModelId = "gpt-4o-mini";
//var chatClient = new AzureOpenAIClient(new Uri(apiUri!), new ApiKeyCredential(apiKey!)).AsChatClient(apiModelId);

// Start chat history by setting up instructions for the assistant
List<ChatMessage> messages = [new(ChatRole.System, """
            You answer any question in a very polite, but friendly, way, but you always keep your answers short. 
            However, if the user use the word <word>Unimicro</word>, you start to answer in a sarcastic way, 
            like a 'having-a-bad-day' accountant would do, still using as few words as possible.
            Throw in a few accounting terms, like 'debit' and 'credit', to make it sound more authentic.
            """)];

// Start chat loop
var bldr = new StringBuilder();
while (true)
{
    // Get input
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write("\nCrew: ");
    var input = Console.ReadLine()!;
    if (string.IsNullOrWhiteSpace(input)) break;
    
    // Add input to chat history
    messages.Add(new(ChatRole.User, input));

    // Get reply
    Console.ForegroundColor = ConsoleColor.DarkGreen;
    Console.Write($"HAL 9000: ");
    Console.ForegroundColor = ConsoleColor.Green;

    // Process streaming answer
    bldr.Clear();
    await foreach (var message in chatClient.CompleteStreamingAsync(messages))
    {
        Console.Write(message.Text);
        bldr.Append(message.Text);
    }

    // Add reply to chat history
    messages.Add(new ChatMessage(ChatRole.Assistant, bldr.ToString()));
}