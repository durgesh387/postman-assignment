using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PostmanAssignment.Entities;
using PostmanAssignment.QueryModels;

namespace PostmanAssignment.Services
{
    public interface ISlotService
    {
        Task<Slot> CreateAsync(Slot slot);
        Task<Slot> GetAsync(string id);
        Task<IEnumerable<Duration>> GetAvailableSlotDurationsAsync(SlotSearchQuery query);
    }
}