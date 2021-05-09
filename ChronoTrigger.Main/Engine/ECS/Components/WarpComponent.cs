using System;
using System.Linq;
using ChronoTrigger.Game;
using ModusOperandi.ECS.Components;

namespace ChronoTrigger.Engine.ECS.Components
{
    //TODO: This.
    [Component]
    public struct WarpComponent
    {
        //public WarpInfo Warp;
        public int SceneIndex;
        public string Scene
        {
            get => ChronoTriggerGame.SceneNames[SceneIndex];
            set => SceneIndex = ChronoTriggerGame.SceneNames.ToList().IndexOf(value);
        }
        
        public unsafe struct WarpInfo
        {
            public int SceneIndex;
        }
    }
}