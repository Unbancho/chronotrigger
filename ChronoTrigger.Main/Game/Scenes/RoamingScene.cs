using ChronoTrigger.Engine.ECS.Systems.DrawSystems;
using ChronoTrigger.Engine.ECS.Systems.SfmlSystems;
using ChronoTrigger.Engine.ECS.Systems.UpdateSystems;
using JetBrains.Annotations;
using ModusOperandi.ECS.Scenes;

namespace ChronoTrigger.Game.Scenes
{
    [UsedImplicitly]
    public class RoamingScene : Scene
    {
        public void StartBattle() { }
        
        public override void Initialize()
        {
            StartSystem<PartySystem>();
            StartSystem<FollowSystem>();
            StartSystem<LeaderInputSystem>();
            StartSystem<MovementSystem>();

            StartSystem<MoveChildSystem>();

            //StartSystem<BoundsCollisionSystem>();
            
            StartSystem<CollisionSystem>();

            StartSystem<WarpEventSystem>();
            
            StartSystem<RepulsionSystem2>();
            StartSystem<PushSystem>();
            
            StartSystem<InteractionSystem>();
            StartSystem<LookSystem>();
            StartSystem<MovementAnimationSystem>();
            StartSystem<AnimationSystem>();
            
            StartSystem<InertiaSystem>();
            
            StartSystem<SpriteDrawSystem>();

            StartSystem<UpdateViewSystemBase>();
            StartSystem<FpsSystem>();
        }
    }
}