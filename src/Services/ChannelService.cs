using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Thiskord_Back.Models.Channel;

namespace Thiskord_Back.Services
{
    public interface IChannelService
    {
        Task<Channel> Create(string channel_name, string channel_desc, int projectId, int userId);
        Task DeleteById(int channelId);
        Task<Channel> Update(int channel_id, string channel_name, string channel_desc);
        List<Channel> GetChannelsByProjectId(int projectId);
        Task<List<Channel>> GetChannelsByProjectIdPerUser(int projectId, int userId);
    }
    
    public class ChannelService : IChannelService
    {
        private readonly IDbConnectionService _dbService;
        private readonly ILogService logService;

        public ChannelService(IDbConnectionService dbService, ILogService logService)
        {
            this._dbService = dbService;
            this.logService = logService;
        }
        public async Task<Channel> Create(string channel_name, string channel_desc, int projectId, int userId)
        {

            if (string.IsNullOrWhiteSpace(channel_name))
                throw new ArgumentException("Le nom du canal ne peut pas être vide.", nameof(channel_name));

            var channel = new Channel
            {
                name = channel_name,
                description = channel_desc
            };

            try
            {
                await using (var connection = _dbService.CreateConnection())
                {
                    connection.Open();

                    string query = @"INSERT INTO Channel (channel_name, channel_desc, id_project) 
                                     VALUES (@Name, @Description, @ProjectId); 
                                     SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    await using var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Name", channel_name);
                    command.Parameters.AddWithValue("@Description", channel_desc);
                    command.Parameters.AddWithValue("@ProjectId", projectId);

                    channel.id = (int)await command.ExecuteScalarAsync();
                }

                await using (var connection = _dbService.CreateConnection())
                {
                    connection.Open();
                    string query = @"INSERT INTO ALLOW (is_visible, is_writable, id_channel_user, id_user) 
                                     VALUES (1, 1,  @channelId, @userId);";
                    await using var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@channelId", channel.id);
                    command.Parameters.AddWithValue("@userId", userId);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                logService.CreateLog($"Error in Create: {ex.Message}");
                throw;
            }

            return channel;
        }

        public async Task DeleteById(int channelId)
        {
            try
            {
                await using (var connection = _dbService.CreateConnection())
                {
                    connection.Open();
                    string deleteQuery = "DELETE FROM Channel WHERE channel_id = @Id";
                    await using var deleteCommand = new SqlCommand(deleteQuery, connection);
                    deleteCommand.Parameters.AddWithValue("@Id", channelId);
                    deleteCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                logService.CreateLog($"Error in DeleteById: {ex.Message}");
                throw;
            }
        }

        public async Task<Channel> Update(int channel_id, string channel_name, string channel_desc)
        {

            if (string.IsNullOrWhiteSpace(channel_name))
                throw new ArgumentException("Le nom du canal ne peut pas être vide.", nameof(channel_name));

            var channel = new Channel
            {
                name = channel_name,
                description = channel_desc
            };

            try
            {
                await using (var connection = _dbService.CreateConnection())
                {
                    connection.Open();

                    string query = @"UPDATE Channel SET channel_name = @Name , channel_desc = @Description, modified_at = @date WHERE channel_id = @Id";

                    await using var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Id", channel_id);
                    command.Parameters.AddWithValue("@Name", channel_name);
                    command.Parameters.AddWithValue("@Description", channel_desc);
                    command.Parameters.AddWithValue("@date", DateTime.UtcNow);

                    command.ExecuteNonQuery();
                    channel.id = channel_id;
                }
            }
            catch (Exception ex)
            {
                logService.CreateLog($"Error in Update: {ex.Message}");
                throw;
            }

            return channel;
        }
        public List<Channel> GetChannelsByProjectId(int projectId)
        {
            var channels = new List<Channel>();

            try
            {
                using (var connection = _dbService.CreateConnection())
                {
                    connection.Open();

                    string query = @"SELECT channel_id, channel_name, channel_desc 
                                     FROM Channel 
                                     WHERE id_project = @ProjectId";

                    using var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ProjectId", projectId);

                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var channel = new Channel
                        {
                            id = (int)reader["channel_id"],
                            name = reader["channel_name"].ToString(),
                            description = reader["channel_desc"].ToString()
                        };
                        channels.Add(channel);
                    }
                }
            }
            catch (Exception ex)
            {
                logService.CreateLog($"Error in GetChannelsByProjectId for projectId {projectId}: {ex.Message}");
                throw;
            }

            return channels;
        }
        public async Task<List<Channel>> GetChannelsByProjectIdPerUser(int projectId, int userId)
        {
            var channels = new List<Channel>();

            try
            {
                await using (var connection = _dbService.CreateConnection())
                {
                    await connection.OpenAsync();

                    string query = @"SELECT c.channel_id, c.channel_name, c.channel_desc 
                                     FROM Channel c 
                                     INNER JOIN ALLOW a ON a.id_channel_user = c.channel_id
                                     WHERE a.id_user = @userId
                                     AND a.is_visible = 1
                                     AND c.id_project = @projectId";

                    await using var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@projectId", projectId);
                    command.Parameters.AddWithValue("@userId", userId);

                    await using var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var channel = new Channel
                        {
                            id = (int)reader["channel_id"],
                            name = reader["channel_name"].ToString(),
                            description = reader["channel_desc"].ToString()
                        };
                        channels.Add(channel);
                    }
                }
            }
            catch (Exception ex)
            {
                logService.CreateLog($"Error in GetChannelsByProjectId for projectId {projectId}: {ex.Message}");
                throw;
            }

            return channels;
        }
    }
}