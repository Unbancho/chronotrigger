using System.Collections.Generic;
using ModusOperandi.ECS.Components;

namespace ChronoTrigger.Engine.ECS.Components
{
    [Component]
    public struct NameComponent
    {
        private static readonly Dictionary<int, string> Names = new() {{0, ""}};

        private int _stringId;

        public string Name
        {
            get => Names[_stringId];
            set
            {
                _stringId = value.GetHashCode();
                Names[_stringId] = value;
            }
        }
    }
}