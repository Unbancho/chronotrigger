using System.Numerics;
using ModusOperandi.ECS.Components;
using ModusOperandi.ECS.Entities;

namespace ChronoTrigger.Engine.ECS.Components
{
    [Component]
    public struct FollowerComponent
    {
        public Entity Target;

        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        //TODO: Turn into Vector2?
        public float DistanceToKeep;
    }
}