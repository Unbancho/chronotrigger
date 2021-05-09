using System.Numerics;
using ModusOperandi.ECS.Components;

namespace ChronoTrigger.Engine.ECS.Components
{
    [Component]
    public struct RepulsionComponent
    {
        public Vector2 Force;
    }
}