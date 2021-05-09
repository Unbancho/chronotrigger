using System.Numerics;
using ModusOperandi.ECS.Components;
using ModusOperandi.ECS.Entities;

namespace ChronoTrigger.Engine.ECS.Components
{
    [Component]
    public struct AttachComponent
    {
        public Entity Target;
        public Vector2 Offset;
    }
}