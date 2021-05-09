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
    public sealed class PartySystem : UpdateEntitySystem
    {
        private static Entity _leader;
        
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

        public override void Execute(float deltaTime)
        {
            _leader = Ecs.GetComponentManager<PartyMemberComponent>().ReverseLookUp(1);
        }

        public override void ActOnEntity(Entity entity, float deltaTime)
        {

        }
        
        [UpdateSystem]
        [Include(typeof(PartyMemberComponent))]
        [Include(typeof(FollowerComponent))]
        [Exclude(typeof(LeaderComponent))]
        private sealed class PartyFollowSystem : UpdateEntitySystem
        {
            public override void ActOnEntity(Entity entity, float deltaTime)
            {
                var partyMember = entity.Get<PartyMemberComponent>();
                // Doing it like this for now. Before we used to keep the distance the same but
                // have the target be the previous party member. Not sure what to pick yet.
                // TODO: Pick.
                ref var followerComponent = ref entity.Get<FollowerComponent>();
                followerComponent.Target = _leader;
                followerComponent.DistanceToKeep = partyMember.Spot * 60;
            }
        }
    
        [UpdateSystem]
        private sealed class PartyLookSystem : UpdateComponentSystem<LookComponent>
        {
            public override void ActOnComponent(ref LookComponent component, float deltaTime)
            {
                component.Target = _leader;
            }
        }
    }
}