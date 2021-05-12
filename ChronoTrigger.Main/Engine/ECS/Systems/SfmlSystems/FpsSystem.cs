using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using ChronoTrigger.Engine.ResourceManagement;
using ModusOperandi.ECS.Systems;
using ModusOperandi.ECS.Systems.SystemInterfaces;
using SFML.Graphics;
using SFML.System;

namespace ChronoTrigger.Engine.ECS.Systems.SfmlSystems
{
    
    [SfmlSystem]
    public class FpsSystem : UniqueSystem, ISystem<GameLoop.GameState>
    {
        private static readonly Text FpsText = new()
        {
            Font = ResourceManager<string, Font>.Get("goblin.ttf"),
            CharacterSize = 16,
            FillColor = Color.Yellow
        };

        public void Run(GameLoop.GameState gameState)
        {
            FpsText.DisplayedString = gameState.GameTime.FPS.ToString(CultureInfo.InvariantCulture);
            var view = sfRenderWindow_getView(gameState.Window.CPointer);
            var center = sfView_getCenter(view);
            var size = sfView_getSize(view);
            FpsText.Position = new (center.X-size.X/2, center.Y-size.Y/2);
            gameState.Window.Draw(FpsText);
        }
        
        [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern Vector2f sfView_getSize(IntPtr View);
        
        [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern Vector2f sfView_getCenter(IntPtr View);
        
        [DllImport(CSFML.graphics, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern IntPtr sfRenderWindow_getView(IntPtr CPointer);
    }
}