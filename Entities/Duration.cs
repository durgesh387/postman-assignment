using System;
using System.Diagnostics.CodeAnalysis;

namespace PostmanAssignment.Entities
{
    public class Duration : IEquatable<Duration>
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public bool Equals(Duration other)
        {
            return StartTime == other.StartTime && EndTime == other.EndTime;
        }
    }
}