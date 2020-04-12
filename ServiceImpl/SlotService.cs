using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PostmanAssignment.Entities;
using PostmanAssignment.Repositories;
using PostmanAssignment.Services;
using PostmanAssignment.Exceptions;
using PostmanAssignment.QueryModels;

namespace PostmanAssignment.ServiceImpl
{
    public class SlotService : ISlotService
    {
        private readonly ILogger<SlotService> _logger;
        private readonly ISlotRepository _slotRepository;
        public SlotService(ILogger<SlotService> logger, ISlotRepository slotRepository)
        {
            _logger = logger;
            _slotRepository = slotRepository;
        }

        public async Task<Slot> CreateAsync(Slot slot)
        {
            ValidateSlot(slot);
            string slotId = await _slotRepository.CreateAsync(slot);
            return await _slotRepository.GetAsync(slotId);
        }

        private void ValidateSlot(Slot slot)
        {
            if (slot == null)
            {
                throw new InvalidArgumentException(nameof(slot), null, "non null slot");
            }
            if (slot.StartTime == DateTime.MinValue)
            {
                throw new InvalidArgumentException(nameof(slot.StartTime), DateTime.MinValue.ToString(), "valid start time");
            }
            if (slot.EndTime == DateTime.MinValue)
            {
                throw new InvalidArgumentException(nameof(slot.EndTime), DateTime.MinValue.ToString(), "valid end time");
            }
            if (slot.InviteeEmail == null)
            {
                throw new InvalidArgumentException(nameof(slot.InviteeEmail), null, "non null invitee email");
            }
            if (slot.ScheduledBy == null)
            {
                throw new InvalidArgumentException(nameof(slot.InviteeEmail), null, "non null invitee email");
            }
        }

        public async Task<Slot> GetAsync(string id)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new InvalidArgumentException(nameof(id), id.ToString(), "non empty id");
            }
            // Call to repository
            var slot = await _slotRepository.GetAsync(id);
            // Entity existence check
            if (slot == null)
            {
                throw new EntityNotFoundException(nameof(Slot), id.ToString());
            }
            return slot;
        }

        public async Task<IEnumerable<Duration>> GetAvailableSlotDurationsAsync(SlotSearchQuery query)
        {
            ValidateSlotSearchQuery(query);
            IEnumerable<Duration> occupiedSlotDurations = await _slotRepository.GetOccupiedSlotDurations(query);
            return ExcludeOccupiedSlotDurations(query.StartTime, query.EndTime, query.SlotDate, query.IntervalType, query.Interval, occupiedSlotDurations);
        }

        private IEnumerable<Duration> ExcludeOccupiedSlotDurations(DateTime startTime, DateTime endTime, DateTime slotDate, IntervalType intervalType, int interval, IEnumerable<Duration> occupiedSlotDurations)
        {
            var allDurations = new List<Duration>();
            DateTime start = startTime == DateTime.MinValue ? new DateTime(slotDate.Year, slotDate.Month, slotDate.Day, 0, 0, 0) : startTime;
            DateTime end = endTime == DateTime.MinValue ? new DateTime(slotDate.Year, slotDate.Month, slotDate.Day, 23, 59, 59) : endTime;
            while (start < end)
            {
                var duration = new Duration();
                if (intervalType == IntervalType.Minute)
                {
                    duration.StartTime = start;
                    var durationEnd = start.AddMinutes(interval);
                    duration.EndTime = durationEnd;
                    start = durationEnd;
                }
                allDurations.Add(duration);
            }
            return allDurations.Except(occupiedSlotDurations);
        }

        private void ValidateSlotSearchQuery(SlotSearchQuery query)
        {
            if (query == null)
            {
                throw new InvalidArgumentException(nameof(SlotSearchQuery), null, "non null query");
            }
            if (string.IsNullOrWhiteSpace(query.UserEmail))
            {
                throw new InvalidArgumentException(nameof(query.UserEmail), null, "non null/empty user email");
            }
            if (query.SlotDate == DateTime.MinValue)
            {
                throw new InvalidArgumentException(nameof(query.SlotDate), DateTime.MinValue.ToString(), "non null slot date");
            }
        }
    }
}