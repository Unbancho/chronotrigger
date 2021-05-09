using ChronoTrigger.Engine.ECS.Components;
using ChronoTrigger.Engine.ECS.Systems.UpdateSystems;
using ChronoTrigger.Engine.ResourceManagement;
using ModusOperandi.ECS.Entities;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace ChronoTrigger.Engine.ECS.Systems.SfmlSystems
{
    [SfmlSystem]
    public class MouserHoverDebugSystem : SfmlSystemBase
    {
        
        private readonly Text _text = new()
        {
            Font = ResourceManager<string, Font>.Get("goblin.ttf"),
            CharacterSize = 16,
            FillColor = Color.White,
        };

        public override void Run(GameLoop.GameState state)
        {
            var mousePosition = Mouse.GetPosition(state.Window);
            var realMousePosition = state.Window.MapPixelToCoords(mousePosition);
            if (PositionMapSystem.EntityScreenPositions.TryGetValue(
                new((int) realMousePosition.X, (int) realMousePosition.Y), out var entity))
            {
                _text.Position = realMousePosition + new Vector2f(20, 0);
                _text.DisplayedString = entity.Get<NameComponent>().Name;
            }
            else
            {
                _text.DisplayedString = string.Empty;
            }
            state.Window.Draw(_text);
        }
    }
}