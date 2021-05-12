using ChronoTrigger.Engine.ECS.Components;
using ModusOperandi.ECS;
using ModusOperandi.ECS.Entities;
using ModusOperandi.ECS.Systems;
using ModusOperandi.ECS.Systems.SystemAttributes;

namespace ChronoTrigger.Engine.ECS.Systems.UpdateSystems
{
    [UpdateSystem]
    [Include(typeof(PartyMemberComponent))]
    [Exclude(typeof(LeaderComponent))]
    public sealed class PartySystem : UpdateEntitySystem<GameLoop.GameState>
    {
        public static Entity Leader { get; private set; }
        
        public PartySystem()
        {
            ComplementarySystems.Add(new PartyFollowSystem());
            ComplementarySystems.Add(new PartyLookSystem());
            Sort();
        }

        public static void Sort()
        {
            var partyManager = Ecs.GetComponentManager<PartyMemberComponent>();
            var partyMembers = partyManager.Components.ComponentArray;
            for (var i = 0; i < partyMembers.Length; i++)
            {
                var spot = (byte) i;
                if (spot < i)
                    spot = byte.MaxValue;
                partyMembers[i].Spot = spot;
            }
        }

        public override void PreExecution()
        {
            Leader = Ecs.GetComponentManager<PartyMemberComponent>().ReverseLookUp(1);
        }


        [UpdateSystem]
        [Include(typeof(PartyMemberComponent))]
        [Include(typeof(FollowerComponent))]
        [Exclude(typeof(LeaderComponent))]
        private sealed class PartyFollowSystem : UpdateEntitySystem<GameLoop.GameState>
        {
            public override void ActOnEntity(Entity entity, GameLoop.GameState gameState)
            {
                var partyMember = entity.Get<PartyMemberComponent>();
                // Doing it like this for now. Before we used to keep the distance the same but
                // have the target be the previous party member. Not sure what to pick yet.
                // TODO: Pick.
                ref var followerComponent = ref entity.Get<FollowerComponent>();
                followerComponent.Target = Leader;
                followerComponent.DistanceToKeep = partyMember.Spot * 60;            }
        }
    
        [UpdateSystem]
        private sealed class PartyLookSystem : UpdateComponentSystem<LookComponent, GameLoop.GameState>
        {
            public override void ActOnComponent(ref LookComponent component, GameLoop.GameState gameState)
            {
                component.Target = Leader;
            }
        }

        public override void ActOnEntity(Entity entity, GameLoop.GameState gameState)
        {
        }
    }
}