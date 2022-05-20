using System.Net.WebSockets;

var app = WebApplication.CreateBuilder(args).Build();
app.UseWebSockets();
app.Use(
    async (context, next) =>
    {
        if (context.Request.Path == "/ws")
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                await Echo(webSocket);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
            return;
        }
        await next(context);
    }
);
await app.RunAsync();

// simple websocket handler, just echo back whatever input it receives.
static async Task Echo(WebSocket webSocket)
{
    var buffer = new byte[1024 * 4];
    var received = await webSocket.ReceiveAsync(
        new ArraySegment<byte>(buffer),
        CancellationToken.None
    );

    while (!received.CloseStatus.HasValue)
    {
        await webSocket.SendAsync(
            new ArraySegment<byte>(buffer, 0, received.Count),
            received.MessageType,
            received.EndOfMessage,
            CancellationToken.None
        );

        received = await webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer),
            CancellationToken.None
        );
    }

    await webSocket.CloseAsync(
        received.CloseStatus.Value,
        received.CloseStatusDescription,
        CancellationToken.None
    );
}
