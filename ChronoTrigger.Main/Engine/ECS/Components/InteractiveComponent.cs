using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using ModusOperandi.ECS.Components;
using ModusOperandi.ECS.Entities;
using ModusOperandi.ECS.Systems;

namespace ChronoTrigger.Engine.ECS.Components
{
    [Component]
    public unsafe struct InteractiveComponent
    {
        private static readonly nuint BasePointer;
        private static readonly nuint LimitPointer;
        private nuint Offset => ScriptID * sizeof(long);
        public nuint ScriptID;

        public void RunScript<T>(T @event) where T : IEntityEvent
        {
            var address = BasePointer + Offset;
            var script = address > LimitPointer
                ? (delegate*<IEntityEvent, void>) BasePointer
                : (delegate*<IEntityEvent, void>) address;
            script(@event);
            if (address != BasePointer) return;
            Console.WriteLine("Error, setting to default interaction.");
        }

        static InteractiveComponent()
        {
            var scripts = typeof(Scripts)
                .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                .Where(m => m.GetCustomAttributes(typeof(Scripts.ScriptAttribute), false).Length > 0);
            var methodInfos = scripts as MethodInfo[] ?? scripts.ToArray();
            BasePointer = (nuint) (delegate*<IEntityEvent, void>) methodInfos[0].MethodHandle.GetFunctionPointer();
            LimitPointer = (nuint) (delegate*<IEntityEvent, void>) methodInfos[^1].MethodHandle.GetFunctionPointer();
        }

        public interface IInteractionEvent : IEntityEvent
        {
            public Entity Target { get; }
        }

        public interface ITalkEvent : IInteractionEvent
        {
            public string Line { get; }
        }
    }

    public static class Scripts
    {
        [Script]
        [UsedImplicitly]
        private static void Test(InteractiveComponent.IInteractionEvent e)
        {
            Console.WriteLine("{0}{1}", $"{e.Target.Get<NameComponent>().Name} ",
                $"interacted with {e.Sender.Get<NameComponent>().Name}");
        }

        [Script]
        [UsedImplicitly]
        private static void Speak(InteractiveComponent.ITalkEvent e)
        {
            Console.WriteLine(e.Line);
        }

        [Script]
        [UsedImplicitly]
        private static void TransferInventory(InteractiveComponent.IInteractionEvent e)
        {
            ref var a = ref e.Sender.Get<Inventory.InventoryComponent>().Gold;
            ref var b = ref e.Target.Get<Inventory.InventoryComponent>().Gold;
            a += b;
            b = 0;
        }

        [AttributeUsage(AttributeTargets.Method)]
        public sealed class ScriptAttribute : Attribute
        {
        }
    }
}