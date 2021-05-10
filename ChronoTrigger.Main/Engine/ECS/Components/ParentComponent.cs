using System.Numerics;
using ModusOperandi.ECS.Components;
using ModusOperandi.ECS.Entities;
using ModusOperandi.ECS.Scenes;

namespace ChronoTrigger.Engine.ECS.Components
{
    [Component]
    public struct ParentComponent : IParentComponent
    {
        public Entity Parent { get; set; }
        public Vector2 Offset;
    }
}