using System;
using ModusOperandi.ECS;
using ModusOperandi.ECS.Components;
using ModusOperandi.ECS.Entities;

namespace ChronoTrigger.ConsoleEngine
{
    public enum ConsoleCommands
    {
        AddComponent,
        RemoveComponent,
        GetComponent
    }

    public static class ConsoleEngine
    {
        public static void EvaluateScript(string script)
        {
            var args = script.Split(" ");
            var (entity, command, component) = ((Entity) uint.Parse(args[0]),
                    Enum.Parse<ConsoleCommands>(args[1]), IComponentManager.ComponentSignatures[int.Parse(args[2])]
                );
            switch (command)
            {
                case ConsoleCommands.AddComponent:
                    Ecs.RegisterComponent(entity, (dynamic)Activator.CreateInstance(component));
                    break;
                case ConsoleCommands.RemoveComponent:
                    break;
                case ConsoleCommands.GetComponent:
                    //Console.WriteLine(entity.Get((dynamic) Activator.CreateInstance(component)));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}