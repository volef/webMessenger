using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Server.Models;

namespace Server.Services
{
    public class MessageRepository
    {
        private readonly string _connString;
        private readonly ILogger<MessageRepository> _logger;
        private readonly string tablename;
        private readonly string basename;

        public MessageRepository(ILogger<MessageRepository> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connString = configuration.GetConnectionString("SQL");

            // пробуем коннектиться
            tablename = "Messages";
            basename = "testMessenger";

            Connect();
        }

        public async Task CreateBase()
        {
            await using (var connection = new SqlConnection(_connString))
            {
                var command = new SqlCommand("CREATE DATABASE " + basename, connection);
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    command.CommandText = "USE [" + basename + "]";
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    _logger.LogCritical("Ошибка бд: " + e.Message);
                    throw;
                }
            }
        }

        public async Task CreateTable()
        {
            await using (var connection = new SqlConnection(_connString))
            {
                var command = new SqlCommand(
                    $"CREATE TABLE [dbo].[{tablename}]( [Id] [int] NOT NULL, [Text] [nvarchar](256) NULL, [SendTime] [datetime] NULL, CONSTRAINT [PK_Messages] PRIMARY KEY CLUSTERED ([Id] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]) ON [PRIMARY]",
                    connection);
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (SqlException e)
                {
                    _logger.LogCritical("Ошибка бд: " + e.Message);
                    throw;
                }
            }
        }

        public async Task Connect()
        {
            var exist = false;
            await using (var connection = new SqlConnection(_connString))
            {
                var command = new SqlCommand($"SELECT database_id FROM sys.databases WHERE Name = '{basename}'",
                    connection);
                try
                {
                    connection.Open();
                    var k = command.ExecuteScalar();
                    exist = k != null;
                }
                catch (SqlException e)
                {
                    _logger.LogCritical("Ошибка бд: " + e.Message);
                    throw;
                }
            }

            if (!exist)
            {
                await CreateBase();
                await CreateTable();
            }
        }

        public async Task<Message> GetMessageAsync(int id)
        {
            await using (var connection = new SqlConnection(_connString))
            {
                var command = new SqlCommand($"SELECT 1 Id,Text,SendTime FROM {tablename} WHERE Id = @id", connection);
                command.Parameters.AddWithValue("@id", id);
                try
                {
                    connection.Open();
                    await using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (!reader.HasRows)
                        {
                            _logger.LogWarning($"Сообщение {id.ToString()} при sql запросе не найдено.");
                            return null;
                        }

                        await reader.ReadAsync();
                        return new Message
                            {Id = reader.GetInt32(0), Text = reader.GetString(1), SendTime = reader.GetDateTime(2)};
                    }
                }
                catch (SqlException e)
                {
                    _logger.LogError("Ошибка бд: " + e.Message);
                }

                return null;
            }
        }

        public async Task<List<Message>> GetMessageFromRangeAsync(DateTime from, DateTime to)
        {
            await using (var connection = new SqlConnection(_connString))
            {
                var command = new SqlCommand($"SELECT * FROM {tablename} WHERE (SendTime BETWEEN @from AND @to)",
                    connection);
                command.Parameters.AddWithValue("@from", from);
                command.Parameters.AddWithValue("@to", to);
                try
                {
                    connection.Open();
                    var result = new List<Message>();
                    await using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                            result.Add(new Message
                            {
                                Id = reader.GetInt32(0), Text = reader.GetString(1), SendTime = reader.GetDateTime(2)
                            });

                        return result;
                    }
                }
                catch (SqlException e)
                {
                    _logger.LogError("Ошибка бд: " + e.Message);
                }

                return null;
            }
        }

        public async Task<Message> AddMessageAsync(string text, int id)
        {
            return await AddMessageAsync(new Message {Id = id, Text = text});
        }

        public async Task<Message> AddMessageAsync(Message message)
        {
            await using (var connection = new SqlConnection(_connString))
            {
                var command = new SqlCommand($"INSERT INTO {tablename} (Id,Text,SendTime) values (@id,@text,@sendtime)",
                    connection);
                command.Parameters.AddWithValue("@id", message.Id);
                command.Parameters.AddWithValue("@text", message.Text);
                var time = DateTime.Now;
                command.Parameters.AddWithValue("@sendtime", time);
                try
                {
                    connection.Open();
                    await using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.RecordsAffected <= 0)
                        {
                            _logger.LogWarning($"Сообщение \"{message.Text}\" при sql запросе не добавлено.");
                            return null;
                        }

                        return new Message
                        {
                            Id = message.Id,
                            Text = message.Text,
                            SendTime = time
                        };
                    }
                }
                catch (SqlException e)
                {
                    _logger.LogError("Ошибка бд: " + e.Message);
                }

                return null;
            }
        }
    }
}