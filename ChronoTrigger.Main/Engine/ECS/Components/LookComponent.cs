using ModusOperandi.ECS.Components;
using ModusOperandi.ECS.Entities;

namespace ChronoTrigger.Engine.ECS.Components
{
    [Component]
    public struct LookComponent
    {
        public Entity Target;
    }
}