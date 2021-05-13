using System;
using System.Collections.Generic;
using System.Numerics;
using ChronoTrigger.Engine.ECS.Components;
using ChronoTrigger.Engine.ECS.Systems.UpdateSystems;

namespace ChronoTrigger.Extensions
{
    public class SpatialHash<T> where T: unmanaged, ITransformableComponent, ISizeableComponent, IEquatable<T>, MoveCollisionSystem.IEntityIdentified
    {
        private readonly int _cellSize;

        private readonly Dictionary<Vector2, List<T>> _contents = new();

        public List<T> Elements { get; } = new();
        
        public SpatialHash(int cellSize)
        {
            _cellSize = cellSize;
        }

        private (int, int) Hash(Vector2 point) => new ((int)point.X / _cellSize, (int)point.Y / _cellSize);

        public void AddBox(T component, bool rotated)
        {
            Elements.Add(component);
            var min = Hash(component.TransformPosition);
            var max = !rotated 
                ? Hash(component.TransformPosition+component.Size) 
                : Hash(component.TransformPosition+Vector2.One
                    *(component.Size.X > component.Size.Y ? component.Size.X : component.Size.Y));

            for (var i = min.Item1; i < max.Item1 + 1; i++)
            for (var j = min.Item2;  j < max.Item2 + 1; j++)
            {
                var key = new Vector2(i, j);
                if(_contents.TryGetValue(key, out var list))
                    list.Add(component);
                else
                {
                    var newList = new List<T>{component};
                    _contents.Add(key, newList);
                }
                if (component.Entity.ID > Max) Max = component.Entity.ID;
            }
        }

        public uint Max { get; private set; }
        public Span<T> GetNearby(T t, bool rotated)
        {
            var min = Hash(t.TransformPosition);
            var max = !rotated 
                ? Hash(t.TransformPosition+t.Size) 
                : Hash(t.TransformPosition+Vector2.One*(t.Size.X > t.Size.Y ? t.Size.X : t.Size.Y));
            var cache = new bool[Max+1];
            Span<T> nearby = new T[Elements.Count];
            var count = 0;
            for (var i = min.Item1; i < max.Item1 + 1; i++)
            for (var j = min.Item2; j < max.Item2 + 1; j++)
            {
                var key = new Vector2(i, j);
                var bucket = _contents[key];
                for (var index = 0; index < bucket.Count; index++)
                {
                    var c = bucket[index];
                    if (!c.Equals(t) && !cache[c.Entity.ID])
                    {
                        nearby[count++] = c;
                        cache[c.Entity.ID] = true;
                    }
                }
            }

            return nearby.Slice(0, count);
        }

        public void Clear()
        {
            Max = 0;
            _contents.Clear();
            Elements.Clear();
        }
    }
}