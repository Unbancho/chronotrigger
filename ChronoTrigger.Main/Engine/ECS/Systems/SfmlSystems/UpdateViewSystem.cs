using System;
using System.Runtime.InteropServices;
using System.Security;
using ChronoTrigger.Engine.ECS.Components;
using ChronoTrigger.Engine.ECS.Systems.DrawSystems;
using ModusOperandi.ECS;
using ModusOperandi.ECS.Entities;
using SFML.System;

namespace ChronoTrigger.Engine.ECS.Systems.SfmlSystems
{
    [SfmlSystem]
    public class UpdateViewSystemBase : SfmlSystemBase
    {
        public override void Run(GameLoop.GameState state)
        {
            var leader = Ecs.GetComponentManager<LeaderComponent>().EntitiesWithComponent[1];
            //var view = state.Window.GetView();
            //view.Center = leader.Get<TransformComponent>().Position.ToVector2f();
            //state.Window.SetView(view);
            var ptr = sfRenderWindow_getView(state.Window.CPointer);
            sfView_setCenter(ptr, leader.Get<TransformComponent>().Position.ToVector2f());
            sfRenderWindow_setView(state.Window.CPointer, ptr);
        }
        
            
        [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern IntPtr sfRenderWindow_getView(IntPtr CPointer);
        
        [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern void sfView_setCenter(IntPtr View, Vector2f center);
        
        [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern void sfRenderWindow_setView(IntPtr CPointer, IntPtr View);
    }
}