using System;
using ModusOperandi.ECS.Components;

namespace ChronoTrigger.Engine.ECS.Components
{
    [Component]
    public struct LeaderComponent
    {
        public InputDevice Device;
        
        [Flags]
        public enum InputDevice : byte
        {
            Keyboard = 1 << 0,
            DPad = 1 << 1,
            Analog = 1 << 2
        }
    }
}