using System.Collections.Generic;
using ModusOperandi.ECS.Components;
using ModusOperandi.ECS.Entities;

namespace ChronoTrigger.Engine.ECS.Components
{
    public static class Inventory
    {
        private static readonly Dictionary<Entity, List<ItemDataComponent>> InventoryDictionary = new();

        public static ItemDataComponent GetItem(this Entity entity)
        {
            return InventoryDictionary[entity][0];
        }

        [Component]
        public struct InventoryComponent
        {
            public ushort Gold;
        }

        [Component]
        public struct ItemDataComponent
        {
            public ushort Count;
            public Entity Item;
        }
    }
}