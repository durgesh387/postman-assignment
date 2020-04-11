using PostmanAssignment.Entities;

namespace PostmanAssignment.Commands
{
    public class SlotCreateCommand : Slot
    {
        public string InviteeEmail { get; set; }
        public string CreatedBy { get; set; }
    }
}