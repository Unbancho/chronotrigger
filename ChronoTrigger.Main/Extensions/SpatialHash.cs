using System;
using System.Collections.Generic;
using System.Numerics;
using ChronoTrigger.Engine.ECS.Components;
using ModusOperandi.ECS;

namespace ChronoTrigger.Extensions
{
    public class SpatialHash<T> where T: unmanaged, ITransformableComponent, ISizeableComponent, IEquatable<T>
    {
        private readonly int _cellSize;

        private readonly Dictionary<(int, int), List<T>> _contents = new();

        public int Count { get; private set; }
        
        public SpatialHash(int cellSize)
        {
            _cellSize = cellSize;
        }

        private (int, int) Hash(Vector2 point) => new ((int)point.X / _cellSize, (int)point.Y / _cellSize);

        public void Add(T component)
        {
            var key = Hash(component.TransformPosition);
            if(_contents.ContainsKey(key))
                _contents[key].Add(component);
            else
            {
                _contents.Add(key, new(){component});
            }

            Count++;
        }

        public void AddBox(T component)
        {
            var (min, max) = (Hash(component.TransformPosition), 
                Hash(component.TransformPosition+component.Size));
            for (var i = min.Item1; i < max.Item1 + 1; i++)
            for (var j = min.Item2;  j < max.Item2 + 1; j++)
            {
                var key = (i, j); 
                if(_contents.TryGetValue(key, out var list))
                    list.Add(component);
                else
                {
                    var newList = new List<T>(Ecs.GetComponentManager<T>().AssignedComponents){component};
                    _contents.Add(key, newList);
                }
            }
        }

        public T[] GetNearby(T t)
        {
            var nearby = new List<T>();
            var buckets = new List<List<T>>();
            var (min, max) = (Hash(t.TransformPosition), 
                Hash(t.TransformPosition+t.Size));
            for (var i = min.Item1; i < max.Item1 + 1; i++)
            for (var j = min.Item2; j < max.Item2 + 1; j++)
            {
                var key = (i, j);
                buckets.Add(_contents[key]);
            }

            for (var i = 0; i < buckets.Count; i++)
            {
                var bucket = buckets[i];
                for (var index = 0; index < bucket.Count; index++)
                {
                    var c = bucket[index];
                    if (!c.Equals(t))
                        nearby.Add(c);
                }
            }

            return nearby.ToArray();
        }

        public void Clear()
        {
            _contents.Clear();
            Count = 0;
        }

        public void CopyTo(List<T>[] list) => _contents.Values.CopyTo(list, 0);

        public Dictionary<(int, int), List<T>>.ValueCollection Buckets => _contents.Values;
    }
}