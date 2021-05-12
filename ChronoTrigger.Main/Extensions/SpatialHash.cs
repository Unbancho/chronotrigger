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

        private readonly Dictionary<float, List<T>> _contents = new();

        public List<T> Elements { get; } = new();
        
        public SpatialHash(int cellSize)
        {
            _cellSize = cellSize;
        }

        private (int, int) Hash(Vector2 point) => new ((int)point.X / _cellSize, (int)point.Y / _cellSize);

        public void AddBox(T component)
        {
            Elements.Add(component);
            var (min, max) = (Hash(component.TransformPosition), 
                Hash(component.TransformPosition+component.Size));
            for (var i = min.Item1; i < max.Item1 + 1; i++)
            for (var j = min.Item2;  j < max.Item2 + 1; j++)
            {
                //TODO: Pretty sure this is bad because of more than 1 digit numbers.
                var key = i + j * 0.1f; 
                if(_contents.TryGetValue(key, out var list))
                    list.Add(component);
                else
                {
                    var newList = new List<T>(Ecs.GetComponentManager<T>().AssignedComponents){component};
                    _contents.Add(key, newList);
                }
            }
        }

        public void AddRotatedBox(T component)
        {
            Elements.Add(component);
            var maxSize = component.Size.X > component.Size.Y ? component.Size.X : component.Size.Y;
            var (min, max) = (Hash(component.TransformPosition), 
                Hash(component.TransformPosition+new Vector2(maxSize, maxSize)));
            for (var i = min.Item1; i < max.Item1 + 1; i++)
            for (var j = min.Item2;  j < max.Item2 + 1; j++)
            {
                var key = i + j * 0.1f; 
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
                var key = i+j*0.1f;
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

        public void Clear<T1>() where T1 : unmanaged
        {
            _contents.Clear();
            Elements.Clear();
            Elements.Capacity = Ecs.GetComponentManager<T1>().AssignedComponents;
        }
    }
}