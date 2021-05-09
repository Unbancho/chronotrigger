using ChronoTrigger.Engine.ECS.Components;
using ChronoTrigger.Engine.ECS.Systems.DrawSystems;
using ChronoTrigger.Game;
using ModusOperandi.ECS.Entities;
using SFML.Graphics;

namespace ChronoTrigger.Engine.ECS.Systems.SfmlSystems
{
    [SfmlSystem]
    public class EntityDebugSystem : SfmlSystemBase
    {
        private readonly RectangleShape _rectangle = new()
        {
            FillColor = Color.Transparent,
            OutlineColor = Color.Blue,
            OutlineThickness = 2
        };
        public override void Run(GameLoop.GameState state)
        {
            var entity = ChronoTriggerGame.SelectedEntity;
            var rect = entity.Get<TextureComponent>().TextureRect;
            _rectangle.Size = new(rect.Width, rect.Height);
            _rectangle.Position = entity.Get<TransformComponent>().Position.ToVector2f() - _rectangle.Size / 2;;
            state.Window.Draw(_rectangle);
        }
    }
}