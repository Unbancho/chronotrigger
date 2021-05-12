using ChronoTrigger.Engine.ECS.Components;
using ModusOperandi.ECS.Systems.SystemAttributes;

namespace ChronoTrigger.Engine.ECS.Systems.BattleSystems
{
    [UpdateSystem]
    public class BattleAnimationSystem
    {
        private static void SetAnimation(ref AnimationComponent animationComponent, ref BattleComponent battleComponent)
        {
            var battleStarted = animationComponent.AnimationType == AnimationType.BattleStart
                                || animationComponent.AnimationType == AnimationType.BattleIdle;
            animationComponent.AnimationSpeed =
                battleStarted ? battleComponent.IsSelected ? (byte) 4f : (byte) 0f : (byte) 3f;
            animationComponent.AnimationType = battleStarted ? AnimationType.BattleIdle : AnimationType.BattleStart;
        }
    }
}