using System;
using PostmanAssignment.Entities;

namespace PostmanAssignment.QueryModels
{
    public class SlotSearchQuery : Duration
    {
        public string UserEmail { get; set; }
        public DateTime SlotDate { get; set; }
        public int Interval { get; set; } = 60; // default 60 minutes slot interval is defined
        public IntervalType IntervalType { get; set; } 
    }
}