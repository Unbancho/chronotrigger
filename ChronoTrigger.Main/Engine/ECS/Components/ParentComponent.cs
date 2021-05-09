using System.Numerics;
using System.Runtime.InteropServices;
using ModusOperandi.ECS.Components;
using ModusOperandi.ECS.Entities;

namespace ChronoTrigger.Engine.ECS.Components
{
    [Component]
    [StructLayout(LayoutKind.Explicit)]
    public struct ParentComponent
    {
        [FieldOffset(0)] public Entity Entity;
        [FieldOffset(4)] public Vector2 Offset;
        [FieldOffset(8)] public Entity PartyLeader;
    }
}