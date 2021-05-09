using System.Collections.Generic;
using ModusOperandi.ECS.Scenes;
using ModusOperandi.ECS.Systems.SystemAttributes;
using ModusOperandi.ECS.Systems.SystemInterfaces;

namespace ChronoTrigger.Engine.ECS.Systems.SfmlSystems
{
    public class SfmlSystemAttribute : SystemGroupAttribute {}
    
    [SfmlSystem]
    public abstract class SfmlSystemBase : ISystem
    {
        public Scene Scene { get; set; }
        public List<ISystem> ComplementarySystems { get; } = new();
        public bool Parallel { get; set; }

        public abstract void Run(GameLoop.GameState state);
    }
}