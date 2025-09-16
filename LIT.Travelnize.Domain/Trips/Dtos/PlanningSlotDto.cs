using LIT.Travelnize.Domain.Trips.ValueObjects;

namespace LIT.Travelnize.Domain.Trips.Dtos
{
    public record PlanningSlotDto(int Order, DateTime? Start, DateTime? End)
    {
        public static PlanningSlotDto From(PlanningSlot slot) =>
            new(slot.Order, slot.Start, slot.End);

        public string ToShortDateString() =>  Start?.ToShortDateString() + " - " + End?.ToShortDateString();
    }
}
