using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Thiskord_Back.Models.Channel;

namespace Thiskord_Back.Services
{
    public class ChannelService
    {
        private readonly IDbConnectionService _dbService;

        private readonly LogService logService;

        public ChannelService(IDbConnectionService dbService, LogService logService)
        {
            this._dbService = dbService;
            this.logService = logService;
        }
        public Channel Create(string channel_name, string channel_desc)
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
                using (var connection = _dbService.CreateConnection())
                {
                    connection.Open();

                    string query = @"INSERT INTO Channel (channel_name, channel_desc) 
                                     VALUES (@Name, @Description); 
                                     SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    using var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Name", channel_name);
                    command.Parameters.AddWithValue("@Description", channel_desc);

                    channel.id = (int)command.ExecuteScalar();

                }
            }
            catch (Exception ex)
            {
                logService.CreateLog($"Error in Create: {ex.Message}");
                throw;
            }

            return channel;
        }
        public void DeleteById(int channelId)
        {
            try
            {
                using (var connection = _dbService.CreateConnection())
                {
                    connection.Open();
                    string deleteQuery = "DELETE FROM Channel WHERE channel_id = @Id";
                    using var deleteCommand = new SqlCommand(deleteQuery, connection);
                    deleteCommand.Parameters.AddWithValue("@Id", channelId);
                    deleteCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                logService.CreateLog($"Error in DeleteById: {ex.Message}");
                throw;
            }
        public Channel Update(int channel_id, string channel_name, string channel_desc)
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
                using (var connection = _dbService.CreateConnection())
                {
                    connection.Open();

                    string query = @"UPDATE Channel SET channel_name = @Name , channel_desc = @Description WHERE channel_id = @Id";

                    using var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Id", channel_id);
                    command.Parameters.AddWithValue("@Name", channel_name);
                    command.Parameters.AddWithValue("@Description", channel_desc);

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
                                     WHERE project_id = @ProjectId";

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
    }
}