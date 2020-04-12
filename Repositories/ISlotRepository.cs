using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PostmanAssignment.Commands;
using PostmanAssignment.Entities;
using PostmanAssignment.QueryModels;

namespace PostmanAssignment.Repositories
{
    public interface ISlotRepository
    {
        Task<Slot> GetAsync(Guid id);
        Task<Guid> CreateAsync(Slot slot);
        Task<IEnumerable<Duration>> GetOccupiedSlotDurations(SlotSearchQuery query);
    }
}