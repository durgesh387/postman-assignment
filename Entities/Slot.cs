using System;

namespace PostmanAssignment.Entities
{
    public class Slot : Duration
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string ScheduledBy { get; set; }
        public string InviteeEmail { get; set; }
    }
}