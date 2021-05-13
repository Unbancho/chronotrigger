using ChronoTrigger.Engine.ECS.Components;
using ChronoTrigger.Engine.ECS.Systems.DrawSystems;
using ChronoTrigger.Game;
using ModusOperandi.ECS.Entities;
using ModusOperandi.ECS.Systems;
using ModusOperandi.ECS.Systems.SystemInterfaces;
using SFML.Graphics;

namespace ChronoTrigger.Engine.ECS.Systems.SfmlSystems
{
    [SfmlSystem]
    public class EntityDebugSystem : UniqueSystem, ISystem<GameLoop.GameState>
    {
        private readonly RectangleShape _rectangle = new()
        {
            FillColor = Color.Transparent,
            OutlineColor = Color.Blue,
            OutlineThickness = 2
        };

        public void Run(GameLoop.GameState gameState)
        {
            var entity = ChronoTriggerGame.SelectedEntity;
            if(entity.IsNullEntity()) return;
            var rect = entity.Get<SpriteComponent>().TextureRect;
            _rectangle.Size = new(rect.Width, rect.Height);
            _rectangle.Position = entity.Get<TransformComponent>().Position.ToVector2f() - _rectangle.Size / 2;;
            gameState.Window.Draw(_rectangle);
        }
    }
}