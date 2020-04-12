using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PostmanAssignment.Commands;
using PostmanAssignment.Entities;
using PostmanAssignment.QueryModels;

namespace PostmanAssignment.Services
{
    public interface ISlotService
    {
        Task<Slot> CreateAsync(Slot slot);
        Task<Slot> GetAsync(Guid id);
        Task<IEnumerable<Duration>> GetAvailableSlotDurationsAsync(SlotSearchQuery query);
    }
}