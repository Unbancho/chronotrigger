using System.Collections.Generic;
using ChronoTrigger.Engine.ECS.Components;
using ModusOperandi.ECS.Entities;
using ModusOperandi.ECS.Systems;
using ModusOperandi.ECS.Systems.SystemAttributes;
using SFML.System;

namespace ChronoTrigger.Engine.ECS.Systems.UpdateSystems
{
    [UpdateSystem]
    [Include(typeof(TransformComponent))]
    public class PositionMapSystem : UpdateEntitySystem
    {
        public static readonly Dictionary<Vector2i, Entity> EntityScreenPositions = new();

        public override void Execute(float deltaTime)
        {
            EntityScreenPositions.Clear();
            base.Execute(deltaTime);
        }

        public override void ActOnEntity(Entity entity, float deltaTime)
        {
            var t = entity.Get<TransformComponent>();
            var p = t.Position;
            for (var i = 0; i < 5; i++)
            for (var j = 0; j < 5; j++)
                EntityScreenPositions[new((int) p.X + i, (int) p.Y + j)] = entity;
        }
    }
}