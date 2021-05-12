using System.Collections.Generic;
using ChronoTrigger.Engine.ECS.Components;
using ChronoTrigger.Engine.ECS.Systems.UpdateSystems;
using ChronoTrigger.Engine.ResourceManagement;
using ModusOperandi.ECS;
using ModusOperandi.ECS.Entities;
using ModusOperandi.ECS.Systems;
using ModusOperandi.ECS.Systems.SystemInterfaces;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace ChronoTrigger.Engine.ECS.Systems.SfmlSystems
{
    [SfmlSystem]
    public class MouserHoverDebugSystem : UniqueSystem, ISystem<GameLoop.GameState>
    {
        
        private readonly Text _text = new()
        {
            Font = ResourceManager<string, Font>.Get("goblin.ttf"),
            CharacterSize = 16,
            FillColor = Color.White,
        };
        

        public void Run(GameLoop.GameState gameState)
        {
            var mousePosition = Mouse.GetPosition(gameState.Window);
            var realMousePosition = gameState.Window.MapPixelToCoords(mousePosition);
            if (PositionMapSystem.EntityScreenPositions.TryGetValue(
                new((int) realMousePosition.X, (int) realMousePosition.Y), out var entity))
            {
                if((Ecs.GetEntityArchetype(entity) & Ecs.GetSignature<NameComponent>()) == 0)
                    return;
                _text.Position = realMousePosition + new Vector2f(20, 0);
                _text.DisplayedString = entity.Get<NameComponent>().Name;
            }
            else
            {
                _text.DisplayedString = string.Empty;
            }

            gameState.Window.Draw(_text);
        }
    }
}