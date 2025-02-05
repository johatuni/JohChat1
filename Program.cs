using System.Text;
using Microsoft.Extensions.AI;

var chatClient = new OllamaChatClient(new Uri("http://localhost:11434"), "llama3.1");

#region 
//var chatClient = new OllamaChatClient(new Uri("http://localhost:11434"), "phi4");
//var chatClient = new OllamaChatClient(new Uri("http://localhost:11434"), "deepseek-r1:14b");
#endregion

List<ChatMessage> messages = [new(ChatRole.System, """
            You answer any question in a very polite, but friendly, way, but you always keep your answers short. 
            If the user use the word <word>Unimicro</word>, you start to answer in a sarcastic way, 
            like a 'having-a-bad-day' accountant would do, still using as few words as possible.
            Throw in a few accounting terms, like 'debit' and 'credit', to make it sound more authentic.
            """)];

while (true)
{
    // Get input
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write("\nCrew: ");
    var input = Console.ReadLine()!;
    if (string.IsNullOrWhiteSpace(input)) break;
    messages.Add(new(ChatRole.User, input));

    // Get reply
    Console.ForegroundColor = ConsoleColor.Green;
    Console.Write($"HAL 9000: ");
    Console.ForegroundColor = ConsoleColor.DarkGreen;
    var bldr = new StringBuilder();
    var responseStream = chatClient.CompleteStreamingAsync(messages);
    await foreach (var message in responseStream)
    {
        Console.Write(message.Text);
        bldr.Append(message.Text);
    }
    messages.Add(new ChatMessage(ChatRole.Assistant, bldr.ToString()));
}