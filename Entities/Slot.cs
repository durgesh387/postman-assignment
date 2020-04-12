using System;

namespace PostmanAssignment.Entities
{
    public class Slot : Duration
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string ScheduledBy { get; set; }
        public string InviteeEmail { get; set; }
    }
}