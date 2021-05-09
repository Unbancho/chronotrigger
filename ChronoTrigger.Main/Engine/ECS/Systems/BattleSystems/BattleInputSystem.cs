using ChronoTrigger.Engine.ECS.Components;
using ModusOperandi.ECS.Archetypes;
using ModusOperandi.ECS.Entities;
using ModusOperandi.ECS.Systems;
using ModusOperandi.ECS.Systems.SystemAttributes;
using SFML.Window;

namespace ChronoTrigger.Engine.ECS.Systems.BattleSystems
{
    [UpdateSystem]
    public class BattleInputSystem : UpdateEntitySystem
    {
        private readonly Keyboard.Key[] _actionKeys =
        {
            Keyboard.Key.Num1, Keyboard.Key.Num2,
            Keyboard.Key.Num3
        };

        private readonly Keyboard.Key[] _selectionKeys =
        {
            Keyboard.Key.Numpad0, Keyboard.Key.Numpad1,
            Keyboard.Key.Numpad2
        };

        private uint _selectedEntity;

        public override void ActOnEntity(Entity entity, float deltaTime)
        {
            if (!Keyboard.IsKeyPressed(_selectionKeys[entity.Get<PartyMemberComponent>().Spot])) return;
            ((Entity) _selectedEntity).Get<BattleComponent>().IsSelected = false;
            entity.Get<BattleComponent>().IsSelected = true;
            _selectedEntity = entity;
        }
    }
}