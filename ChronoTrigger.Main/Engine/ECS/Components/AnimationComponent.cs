using System;
using System.Collections.Generic;
using System.Linq;
using ChronoTrigger.Engine.Movement;
using ChronoTrigger.Engine.ResourceManagement;
using JetBrains.Annotations;
using ModusOperandi.ECS.Components;
using ModusOperandi.Utils.YAML;
using SFML.Graphics;

namespace ChronoTrigger.Engine.ECS.Components
{
    public enum AnimationType : byte
    {
        Idle,
        Walking,
        Running,
        BattleStart,
        BattleIdle
    }

    internal readonly struct AnimationStruct
    {
        public IntRect[] Frames { get; init; }
        public bool Mirrored { get; init; }
    }

    [Component]
    public struct AnimationComponent
    {
        private short _id;

        public AnimationType AnimationType
        {
            get => (AnimationType) ((_id & ~0b11) >> 2);
            set => _id = (short) (((short) value << 2) | (_id & 0b11));
        }

        public Direction Direction
        {
            get => (Direction) (_id & 0b11);
            set => _id = (short) ((ushort) value | (_id & ~0b11));
        }

        private ushort _sheetId;
        public long AnimationKey => _id + (_sheetId << 10);

        public float AnimationSpeed;
        public float CurrentIndex;

        [UsedImplicitly]
        internal class AnimationYaml
        {
#pragma warning disable 649
            public bool Mirrored;
            public string Direction;
            public string Type;
            public IntRect[] Rects;
#pragma warning restore 649
        }

        [UsedImplicitly]
        public string Animations
        {
            set
            {
                var anim = value;
                var animations = Yaml.Deserialize<object, List<AnimationYaml>>(
                    $"{GameDirectories.AnimationsDirectory}/Yaml/{anim}.yaml").First().Value;
                if (LoadedAnimations.Contains(anim))
                {
                    _sheetId = (ushort) LoadedAnimations.IndexOf(anim);
                }
                else
                {
                    _sheetId = (ushort) LoadedAnimations.Count;
                    LoadedAnimations.Add(anim);
                }

                foreach (var animation in animations)
                {
                    (Direction, AnimationType) = (Enum.Parse<Direction>(animation.Direction),
                        Enum.Parse<AnimationType>(animation.Type));
                    ResourceManager<long, AnimationStruct>.Store(new()
                    {
                        Frames = animation.Rects,
                        Mirrored = animation.Mirrored
                    }, AnimationKey);
                }

                Direction = default;
                AnimationType = default;
            }
            get => "";
        }

        private static readonly List<string> LoadedAnimations = new();
    }
}