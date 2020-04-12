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
            return this.StartTime == other.StartTime && this.EndTime == other.EndTime;
        }
        public override int GetHashCode()
        {
            int hashStartTime = this.StartTime != DateTime.MinValue ? this.StartTime.GetHashCode() : 0;
            int hashEndTime = this.EndTime != DateTime.MinValue ? this.EndTime.GetHashCode() : 0;
            return hashStartTime ^ hashEndTime;
        }
    }
}