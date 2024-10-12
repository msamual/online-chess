using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// Добавляем поддержку WebSocket
app.UseWebSockets();

app.Use(async (context, next) =>
{
	if (context.WebSockets.IsWebSocketRequest)
	{
		WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
		await HandleWebSocketAsync(webSocket);
	}
	else
	{
		await next();
	}
});

app.MapGet("/", () => "WebSocket Server is running...");

app.Run();

static async Task HandleWebSocketAsync(WebSocket webSocket)
{
	var buffer = new byte[1024 * 4];
	WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

	while (!result.CloseStatus.HasValue)
	{
		var serverMsg = Encoding.UTF8.GetBytes("d7-d5");
		await webSocket.SendAsync(new ArraySegment<byte>(serverMsg, 0, serverMsg.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);

		result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
	}

	await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
}
