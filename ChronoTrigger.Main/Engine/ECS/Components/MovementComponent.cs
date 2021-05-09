using System.Numerics;
using ModusOperandi.ECS.Components;

namespace ChronoTrigger.Engine.ECS.Components
{
    [Component]
    public struct MovementComponent
    {
        public Vector2 Direction => Velocity / Speed;
        public float Speed => Velocity.Length();
        public Vector2 Velocity;
    }
}