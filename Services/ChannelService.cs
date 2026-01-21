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
                Name = channel_name,
                Description = channel_desc
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

                    channel.Id = (int)command.ExecuteScalar();

                }
            } catch (Exception ex)
            {
                logService.CreateLog(ex.Message);
                
            };
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
                logService.CreateLog(ex.Message);
            }
        }
    }
}