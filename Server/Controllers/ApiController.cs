using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Server.Models;
using Server.Services;

namespace Server.Controllers
{
    [ApiController]
    [Route("[controller]/{action}")]
    public class ApiController : ControllerBase
    {
        private readonly ILogger<ApiController> _logger;
        private readonly MessageRepository _msgrep;
        private readonly SocketHandler _socketHandler;

        public ApiController(ILogger<ApiController> logger, MessageRepository msgrep, SocketHandler socketHandler)
        {
            _logger = logger;
            _msgrep = msgrep;
            _socketHandler = socketHandler;
        }

        [HttpPost]
        public async Task<IActionResult> Send([FromBody] Message message)
        {
            var result = await _msgrep.AddMessageAsync(message);
            if (result == null)
            {
                _logger.LogWarning($"Ошибка добавления сообщения: {message.Id} {message.Text}");
                return BadRequest();
            }

            await _socketHandler.SendMessageToAllAsync(result);
            return Accepted();
        }

        [HttpGet]
        public async Task<IActionResult> GetLastMessagesForMinute()
        {
            var now = DateTime.Now;
            var last = now.AddMinutes(-1);
            var result = await _msgrep.GetMessageFromRangeAsync(last, now);
            if (result.Count <= 0)
            {
                _logger.LogInformation(
                    $"Не найдены сообщения за период с {last.ToShortTimeString()} по {now.ToShortTimeString()}");
                return NotFound();
            }

            return new JsonResult(result);
        }
    }
}