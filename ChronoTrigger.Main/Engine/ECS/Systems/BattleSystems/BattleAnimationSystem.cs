using ChronoTrigger.Engine.ECS.Components;
using ModusOperandi.ECS.Archetypes;
using ModusOperandi.ECS.Entities;
using ModusOperandi.ECS.Systems;
using ModusOperandi.ECS.Systems.SystemAttributes;

namespace ChronoTrigger.Engine.ECS.Systems.BattleSystems
{
    [UpdateSystem]
    public class BattleAnimationSystem : UpdateEntitySystem
    {
        private static void SetAnimation(ref AnimationComponent animationComponent, ref BattleComponent battleComponent)
        {
            var battleStarted = animationComponent.AnimationType == AnimationType.BattleStart
                                || animationComponent.AnimationType == AnimationType.BattleIdle;
            animationComponent.AnimationSpeed =
                battleStarted ? battleComponent.IsSelected ? (byte) 4f : (byte) 0f : (byte) 3f;
            animationComponent.AnimationType = battleStarted ? AnimationType.BattleIdle : AnimationType.BattleStart;
        }

        public override void ActOnEntity(Entity entity, float deltaTime)
        {
            SetAnimation(ref entity.Get<AnimationComponent>(), ref entity.Get<BattleComponent>());
        }
    }
}