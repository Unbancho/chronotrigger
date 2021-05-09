namespace ChronoTrigger.Engine.ECS.Systems.BattleSystems
{
    /*
    public class AttackSystem : System<BattleComponent>
    {
        protected override void ActOnComponents(uint entity, uint index, float deltaTime, params object[] dependencies)
        {
            //var attack = Get<BattleComponent>(entity).BattleStyle == BattleStyle.Male
            //    ? (byte) (Get<BattleComponent>(entity).POW * 4 / 3 + Get<BattleComponent>(entity).Weapon * 5 / 9)
            //    : (byte) ((Get<BattleComponent>(entity).Hit + Get<BattleComponent>(entity).Weapon) * 2 / 3);
            var battler = Get<BattleComponent>(entity);
            var attack = Get<BattleComponent>(entity).BattleStyle switch
            {
                BattleStyle.Male => (int) (battler.POW * 4f / 3f + battler.Weapon * 5f / 9f),
                BattleStyle.Female => (int) ((battler.Hit + battler.Weapon) * 2f / 3f),
                BattleStyle.Ayla => (int) (battler.POW * 1.75f + 0 * 0 / 45.5f),
                _ => throw new ArgumentOutOfRangeException()
            };
            var damage = attack * 4;
        }
    }

    public class DefenseSystem : System<BattleComponent>
    {
        protected override void ActOnComponents(uint entity, uint index, float deltaTime, params object[] dependencies)
        {
            throw new NotImplementedException();
        }
    }
    */
}