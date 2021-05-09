using System.Collections.Generic;

namespace ChronoTrigger.Engine.ECS.Systems.UpdateSystems
{
    public class AISystem
    {
        private List<int> list;

        public AISystem()
        {
            list.Add((int)Actions.Walk);
        }
        
        public struct Node
        {
            private unsafe Node* Next;
        }

        private enum Actions
        {
            Walk
        }
    }
}