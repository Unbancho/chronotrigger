using ModusOperandi.ECS.Components;

namespace ChronoTrigger.Engine.ECS.Components
{
    [Component]
    public struct PartyMemberComponent
    {
        public byte Spot;
        public bool IsLeader => Spot == 0;
    }
}