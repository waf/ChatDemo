using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;

namespace ChatDemo.Client;

/// <remarks>https://fuqua.io/blog/2020/02/native-websockets-with-blazor-webassembly/</remarks>
public class ChatClientConnection
{
    private readonly ClientWebSocket websocket;
    private readonly Uri websocketUrl;

    public ChatClientConnection(ClientWebSocket websocket, Uri websocketUrl)
    {
        this.websocket = websocket;
        this.websocketUrl = websocketUrl;
    }

    /// <summary>
    /// Connect to the websocket and begin yielding messages
    /// received from the connection.
    /// </summary>
    public async IAsyncEnumerable<string> ConnectAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        await websocket.ConnectAsync(websocketUrl, cancellationToken);
        var buffer = new ArraySegment<byte>(new byte[2048]);
        while (!cancellationToken.IsCancellationRequested)
        {
            WebSocketReceiveResult result;
            using var ms = new MemoryStream();
            do
            {
                result = await websocket.ReceiveAsync(buffer, cancellationToken);
                ms.Write(buffer.Array!, buffer.Offset, result.Count);
            } while (!result.EndOfMessage);

            ms.Seek(0, SeekOrigin.Begin);

            yield return Encoding.UTF8.GetString(ms.ToArray());

            if (result.MessageType == WebSocketMessageType.Close)
                break;
        }
    }

    /// <summary>
    /// Send a message on the websocket.
    /// This method assumes you've already connected via ConnectAsync
    /// </summary>
    public Task SendStringAsync(string data, CancellationToken cancellation)
    {
        var encoded = Encoding.UTF8.GetBytes(data);
        var buffer = new ArraySegment<byte>(encoded, 0, encoded.Length);
        return websocket.SendAsync(
            buffer,
            WebSocketMessageType.Text,
            endOfMessage: true,
            cancellation
        );
    }
}
