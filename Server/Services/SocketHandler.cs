using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Server.Models;

namespace Server.Services
{
    public class SocketHandler
    {
        private readonly ILogger<SocketHandler> _logger;

        public SocketHandler(ILogger<SocketHandler> logger, SocketRepository socketRepository)
        {
            _logger = logger;
            SocketRepository = socketRepository;
        }

        private SocketRepository SocketRepository { get; }

        public void OnConnected(WebSocket socket)
        {
            SocketRepository.AddSocket(socket);
        }

        public async Task OnDisconnected(WebSocket socket)
        {
            await SocketRepository.RemoveSocket(SocketRepository.GetId(socket));
        }

        public async Task SendMessageAsync(WebSocket socket, Message message)
        {
            if (socket.State != WebSocketState.Open)
                return;
            var tosend = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            await socket.SendAsync(new ArraySegment<byte>(tosend,
                    0,
                    tosend.Length),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
            _logger.LogInformation("Успешная передача по вебсокету");
        }

        public async Task SendMessageToAllAsync(Message message)
        {
            foreach (var pair in SocketRepository.GetAll())
                if (pair.Value.State == WebSocketState.Open)
                    await SendMessageAsync(pair.Value, message);
        }
    }
}