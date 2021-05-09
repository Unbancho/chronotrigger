using System.Numerics;
using ChronoTrigger.Engine.ECS.Components;
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

            StartSystem<BoundsCollisionSystem>();
            
            StartSystem<CollisionSystem>();

            StartSystem<WarpEventSystem>();
            
            StartSystem<PushSystem>();
            StartSystem<RepulsionSystem2>();
            StartSystem<InteractionSystem>();
            StartSystem<LookSystem>();
            StartSystem<MovementAnimationSystem>();
            StartSystem<AnimationSystem>();
            
            StartSystem<InertiaSystem>();
            
            StartSystem<TextureLayerDraw<BackgroundTextureComponent>>();
            StartSystem<SpriteDrawSystem>();

            StartSystem<UpdateViewSystemBase>();
            StartSystem<FpsSystem>();
        }
    }
}