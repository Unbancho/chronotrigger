using ModusOperandi.ECS.Components;
using ModusOperandi.ECS.Entities;

namespace ChronoTrigger.Engine.ECS.Components
{
    [Component]
    public struct SelectorComponent
    {
        public Entity Target;
    }
}