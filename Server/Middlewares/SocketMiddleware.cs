using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Server.Services;

namespace Server.Middlewares
{
    public class SocketMiddleware
    {
        private RequestDelegate _next;
        private readonly SocketHandler _socketHandler;

        public SocketMiddleware(RequestDelegate next, SocketHandler socketHandler)
        {
            _next = next;
            _socketHandler = socketHandler;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
                if (context.Request.Path == "/2ndclient")
                {
                    var socket = await context.WebSockets.AcceptWebSocketAsync();
                    _socketHandler.OnConnected(socket);
                    await Receive(socket, async (result, buffer) =>
                    {
                        if (result.MessageType == WebSocketMessageType.Close)
                            await _socketHandler.OnDisconnected(socket);
                    });
                }
        }

        private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 20];

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer),
                    CancellationToken.None);

                handleMessage(result, buffer);
            }
        }
    }
}