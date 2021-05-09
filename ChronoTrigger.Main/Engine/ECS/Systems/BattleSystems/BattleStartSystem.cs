using System.Numerics;
using ChronoTrigger.Engine.ECS.Components;
using ModusOperandi.ECS.Archetypes;
using ModusOperandi.ECS.Entities;
using ModusOperandi.ECS.Systems;

namespace ChronoTrigger.Engine.ECS.Systems.BattleSystems
{
    [InitializeSystem]
    public class BattleStartSystem : UpdateEntitySystem
    {
        private readonly Vector2[] _posArray =
            {new(650, 150), new(750, 250), new(550, 250)};

        public override void ActOnEntity(Entity entity, float deltaTime)
        {
            entity.Get<TransformComponent>().Position = _posArray[0];
            var temp = _posArray[0];
            _posArray[0] = _posArray[1];
            _posArray[1] = _posArray[2];
            _posArray[2] = temp;
            entity.Get<BattleComponent>().IsSelected = entity.Get<PartyMemberComponent>().IsLeader;
        }
    }
}