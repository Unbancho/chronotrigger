using JetBrains.Annotations;
using ModusOperandi.ECS.Components;

namespace ChronoTrigger.Engine.ECS.Components
{
    public interface IStatComponent
    {
        ushort Cap { get; }
        ushort Max { get; set; }
        ushort Current { get; set; }
    }

    [Component]
    [UsedImplicitly]
    public struct HpComponent : IStatComponent
    {
        public ushort Cap { get; }
        public ushort Max { get; set; }
        public ushort Current { get; set; }

        public HpComponent(int max)
        {
            Cap = 999;
            Max = (ushort) max;
            Current = Max;
        }
    }

    [Component]
    [UsedImplicitly]
    public struct MpComponent : IStatComponent
    {
        public ushort Cap { get; }
        public ushort Max { get; set; }
        public ushort Current { get; set; }

        public MpComponent(int max)
        {
            Cap = 99;
            Max = (ushort) max;
            Current = Max;
        }
    }

    [Component]
    [UsedImplicitly]
    public struct DefComponent : IStatComponent
    {
        public ushort Cap { get; }
        public ushort Max { get; set; }
        public ushort Current { get; set; }

        public DefComponent(int max)
        {
            Cap = 256;
            Max = (ushort) max;
            Current = Max;
        }
    }

    [Component]
    [UsedImplicitly]
    public struct MdefComponent : IStatComponent
    {
        public ushort Cap { get; }
        public ushort Max { get; set; }
        public ushort Current { get; set; }

        public MdefComponent(int max)
        {
            Cap = 100;
            Max = (ushort) max;
            Current = Max;
        }
    }

    [Component]
    [UsedImplicitly]
    public struct SpdComponent : IStatComponent
    {
        public ushort Cap { get; }
        public ushort Max { get; set; }
        public ushort Current { get; set; }

        public SpdComponent(int max)
        {
            Cap = 16;
            Max = (ushort) max;
            Current = Max;
        }
    }

    public enum BattleStyle
    {
        Male,
        Female,
        Ayla
    }

    [Component]
    public struct BattleComponent
    {
        public bool IsSelected;

        public BattleStyle BattleStyle;

        public readonly byte Pow;
        public readonly byte Weapon;
        public float CriticalRate;
        public readonly byte Hit;

        public BattleComponent(int battleStyle)
        {
            IsSelected = false;
            BattleStyle = (BattleStyle) battleStyle;
            Pow = 0;
            Weapon = 0;
            CriticalRate = 0;
            Hit = 0;
        }

        public byte MaxRandomDamageBonus => BattleStyle == BattleStyle.Male ? (byte) (Pow / 3) : (byte) (Hit * 3);
        // techs
    }
}