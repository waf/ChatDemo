# Blazor WASM WebSocket chat demo

Demo on how to use `System.Net.WebSockets` to build simple real-time applications with Blazor WASM. This does not use SignalR, but instead shows how use only System.Net.WebSockets, which is natively supported in Blazor WASM.

To run this:

1. Navigate to ChatDemo.Server and execute `dotnet run`. This will start up the websocket server listening on http://localhost:5236 and ws://localhost:5236/ws
2. Navigate to ChatDemo.Client and execute `dotnet run`. This will start up a server that hosts the Blazor WASM client on http://localhost:5250
3. In your browser, open the URL that contains ChatDemo.Client and send a chat message.

The client will send this message to the server started in Step 1, and the server will echo it back to the client, which will then render it to the page.
