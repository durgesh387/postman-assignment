using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using PostmanAssignment.Entities;
using PostmanAssignment.Repositories;
using PostmanAssignment.QueryModels;
using PostmanAssignment.Utilities;
using System.Data.Common;
using PostmanAssignment.Commands;
using Microsoft.Extensions.Options;

namespace PostmanAssignment.RepositoryImpl
{
    public class SlotRepository : ISlotRepository
    {
        private readonly ApplicationSettings _appSettings;
        public SlotRepository(IOptions<ApplicationSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }
        public async Task<Guid> CreateAsync(SlotCreateCommand slot)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (MySqlConnection connection = new MySqlConnection(_appSettings.ConnectionString))
            {
                await connection.OpenAsync();
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = Routines.GetOccupiedSlotDurations;
                    AddSlotParams(command, slot);
                    var slotId = await command.ExecuteScalarAsync();
                    return (Guid)(slotId);
                }
            }
        }

        private void AddSlotParams(MySqlCommand command, SlotCreateCommand slot)
        {
            command.Parameters.AddWithValue("vId", Guid.NewGuid().ToString());
            command.Parameters.AddWithValue("vTitle", slot.Title);
            command.Parameters.AddWithValue("vStartTime", slot.EndTime == DateTime.MinValue ? null : (DateTime?)slot.StartTime);
            command.Parameters.AddWithValue("vEndTime", slot.EndTime == DateTime.MinValue ? null : (DateTime?)slot.EndTime);
            command.Parameters.AddWithValue("vCreatedBy", slot.CreatedBy);
            command.Parameters.AddWithValue("vInviteeEmail", slot.InviteeEmail);
        }

        public async Task<Slot> GetAsync(Guid id)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (MySqlConnection connection = new MySqlConnection(_appSettings.ConnectionString))
            {
                await connection.OpenAsync();
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = Routines.GetSlot;
                    command.Parameters.AddWithValue("vId", id);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        return await ReadSlotAsync(reader);
                    }
                }
            }
        }

        private async Task<Slot> ReadSlotAsync(DbDataReader dr)
        {
            Slot slot = null;
            if (await dr.ReadAsync())
            {
                slot = new Slot();
                slot.Id = (Guid)(dr["id"]);
                slot.Title = Convert.ToString(dr["title"]);
                slot.StartTime = Convert.ToDateTime(dr["start_time"]);
                slot.EndTime = Convert.ToDateTime(dr["end_time"]);
            }
            return slot;
        }

        public async Task<IEnumerable<Duration>> GetOccupiedSlotDurations(SlotSearchQuery query)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (MySqlConnection connection = new MySqlConnection(_appSettings.ConnectionString))
            {
                await connection.OpenAsync();
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "v1_get_occupied_slot_durations";

                    AddSlotSearchQueryParams(command, query);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        return await ReadOccupiedDurationsAsync(reader);
                    }
                }
            }
        }

        private void AddSlotSearchQueryParams(MySqlCommand command, SlotSearchQuery query)
        {
            command.Parameters.AddWithValue("vUserEmail", query.UserEmail);
            command.Parameters.AddWithValue("vSlotDate", query.SlotDate);
            command.Parameters.AddWithValue("vStartTime", query.StartTime == DateTime.MinValue ? null : (DateTime?)query.StartTime);
            command.Parameters.AddWithValue("vEndTime", query.EndTime == DateTime.MinValue ? null : (DateTime?)query.EndTime);
        }

        private async Task<IEnumerable<Duration>> ReadOccupiedDurationsAsync(DbDataReader reader)
        {
            var occupiedDurations = new List<Duration>();
            while (await reader.ReadAsync())
            {
                var duration = new Duration();
                duration.StartTime = Convert.ToDateTime(reader["start_time"]);
                duration.EndTime = Convert.ToDateTime(reader["end_time"]);
                occupiedDurations.Add(duration);
            }
            return occupiedDurations;
        }
    }
}