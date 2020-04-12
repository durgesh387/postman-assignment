using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PostmanAssignment.Entities;
using PostmanAssignment.QueryModels;

namespace PostmanAssignment.Repositories
{
    public interface ISlotRepository
    {
        Task<Slot> GetAsync(string id);
        Task<string> CreateAsync(Slot slot);
        Task<IEnumerable<Duration>> GetOccupiedSlotDurations(SlotSearchQuery query);
    }
}