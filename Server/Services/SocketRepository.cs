using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Server.Services
{
    public class SocketRepository
    {
        private readonly ILogger<SocketRepository> _logger;

        private readonly ConcurrentDictionary<string, WebSocket> _sockets =
            new ConcurrentDictionary<string, WebSocket>();

        public SocketRepository(ILogger<SocketRepository> logger)
        {
            _logger = logger;
        }

        public ConcurrentDictionary<string, WebSocket> GetAll()
        {
            return _sockets;
        }

        public string GetId(WebSocket socket)
        {
            return _sockets.FirstOrDefault(p => p.Value == socket).Key;
        }

        public void AddSocket(WebSocket socket)
        {
            var id = CreateConnectionId();
            _sockets.TryAdd(id, socket);
            _logger.LogInformation($"Добавлен сокет {id}");
        }

        public async Task RemoveSocket(string id)
        {
            WebSocket socket;
            _sockets.TryRemove(id, out socket);
            if (socket == null)
            {
                _logger.LogError($"Попытка закрыть несуществующий сокет {id}");
            }
            else
            {
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                    "Closed by the Server",
                    CancellationToken.None);
                _logger.LogInformation($"Сокет {id} закрыт и удален");
            }
        }

        private string CreateConnectionId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}