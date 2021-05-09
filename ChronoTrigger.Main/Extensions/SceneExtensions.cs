using System.Numerics;
using ChronoTrigger.Engine.ECS.Components;
using ModusOperandi.ECS.Entities;
using ModusOperandi.ECS.Scenes;

namespace ChronoTrigger.Extensions
{
    public static class SceneExtensions
    {
        public static Entity PlaceEntity(this Scene scene, string type, Vector2 position)
        {
            var entity = scene.PlaceEntity(type);
            entity.Get<TransformComponent>().Position = position;
            return entity;
        }
    }
}