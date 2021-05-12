using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security;
using ChronoTrigger.Engine.ECS.Components;
using ModusOperandi.ECS;
using ModusOperandi.ECS.Entities;
using ModusOperandi.ECS.Systems;
using ModusOperandi.ECS.Systems.SystemInterfaces;
using SFML.System;

namespace ChronoTrigger.Engine.ECS.Systems.SfmlSystems
{
    [SfmlSystem]
    public class UpdateViewSystemBase : UniqueSystem, ISystem<GameLoop.GameState>
    {
        public void Run(GameLoop.GameState state)
        {
            var leader = Ecs.GetComponentManager<LeaderComponent>().EntitiesWithComponent[1];
            var ptr = sfRenderWindow_getView(state.Window.CPointer);
            sfView_setCenter(ptr, leader.Get<TransformComponent>().Position);
            sfRenderWindow_setView(state.Window.CPointer, ptr);        
        }

        [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern IntPtr sfRenderWindow_getView(IntPtr CPointer);
        
        [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern void sfView_setCenter(IntPtr View, Vector2 center);
        
        [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern void sfRenderWindow_setView(IntPtr CPointer, IntPtr View);
    }
}