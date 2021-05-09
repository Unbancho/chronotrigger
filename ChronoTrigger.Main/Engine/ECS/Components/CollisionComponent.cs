using System;
using System.Numerics;
using ModusOperandi.ECS.Components;
using ModusOperandi.ECS.Entities;
using SFML.Graphics;

// ReSharper disable UnassignedField.Global


namespace ChronoTrigger.Engine.ECS.Components
{
    public interface ISizeableComponent
    {
        public Vector2 Size { get; set; }
    }
    
    [Component]
    public struct CollisionComponent : ITransformableComponent, ISizeableComponent, IDebugDrawableComponent, IEquatable<CollisionComponent>
    {
        //TODO Decouple Position from this.
        public RotatingRect Hitbox;
        public Vector2 Offset;
        public bool Solid;

        public Entity Entity;

        public Vector2 Size
        {
            get => new(Hitbox.Width, Hitbox.Height);
            set => (Hitbox.Width, Hitbox.Height) = (value.X, value.Y);
        }

        public Vector2 TransformPosition
        {
            get => Hitbox.Position;
            set => (Hitbox.Left, Hitbox.Top) = (value.X + Offset.X, value.Y + Offset.Y);
        }

        public float Rotation
        {
            get => Hitbox.Rotation;
            set => Hitbox.Rotation = value * 0.0174533f;
        }

        public Drawable DebugDrawable => new Sprite(null, new(0, 0,
            (int) Hitbox.Width, (int) Hitbox.Height))
        {
            Position = new(Hitbox.Left, Hitbox.Top),
            Color = new(255, 0, 0, 128),
            Rotation = Rotation / 0.0174533f
        };

        public bool Equals(CollisionComponent other)
        {
            return Entity == other.Entity;
        }
    }

    public struct RotatingRect
    {
        private FloatRect _floatRect;
        private float _sin;
        private float _cos => MathF.Sqrt(1 - _sin * _sin);

        public float Rotation
        {
            get => MathF.Asin(_sin);
            set
            {
                var angle = value;
                if (Math.Abs(angle - MathF.PI / 2) < 0.05f)
                {
                    angle = 0;
                    (_floatRect.Width, _floatRect.Height) = (_floatRect.Height, _floatRect.Width);
                }

                _sin = MathF.Sin(angle);
            }
        }

        public float Left
        {
            get => Position.X;
            set => Position = new(value, Position.Y); //value*_cos + _floatRect.Top*_sin;
        }

        public float Top
        {
            get => Position.Y;
            set => Position = new(Position.X, value);
        }

        public float Width
        {
            get => _floatRect.Width;
            set => _floatRect.Width = value;
        }

        public float Height
        {
            get => _floatRect.Height;
            set => _floatRect.Height = value;
        }

        public RotatingRect(FloatRect fr, float rotation = 0)
        {
            _floatRect = fr;
            //_cos = 0;
            _sin = 0;
            Rotation = rotation;
        }

        public bool Intersects(RotatingRect r, out RotatingRect overlap)
        {
            if (Rotation != 0 || r.Rotation != 0) return IntersectsOrTouchesAngled(r, out overlap);
            var i = _floatRect.Intersects(r._floatRect, out var innerOverlap);
            overlap = new(innerOverlap);
            return i;
        }

        public bool IntersectsOrTouches(RotatingRect r, out RotatingRect overlap)
        {
            if (Rotation != 0 || r.Rotation != 0) return IntersectsOrTouchesAngled(r, out overlap);
            var i = _floatRect.IntersectsOrTouches(r._floatRect, out var innerOverlap);
            overlap = new(innerOverlap);
            return i;
        }


        private static Vector2[] AnglePoints(RotatingRect r)
        {
            var points = new[]
            {
                r.Position, new Vector2(r.Size.X, 0),
                new Vector2(0, r.Size.Y),
                new Vector2(r.Size.X, r.Size.Y)
            };
            for (var i = 1; i < points.Length; i++)
            {
                ref var v = ref points[i];
                var (x, y) = (v.X, v.Y);
                v.X = x * r._cos - y * r._sin + r.Position.X;
                v.Y = x * r._sin + y * r._cos + r.Position.Y;
            }

            return points;
        }

        //TODO: Make this work.
        public bool IntersectsOrTouchesAngled(RotatingRect r, out RotatingRect overlap)
        {
            var a = AnglePoints(this);
            var b = AnglePoints(r);

            overlap = default;

            for (var i = 0; i < a.Length; ++i)
            {
                var current = a[i];
                var next = a[(i + 1) % a.Length];
                var edge = next - current;

                var axis = new Vector2(-edge.Y, edge.X);

                var aMax = float.NegativeInfinity;
                var aMin = float.PositiveInfinity;
                var bMax = float.NegativeInfinity;
                var bMin = float.PositiveInfinity;

                foreach (var v in a)
                {
                    var proj = Vector2.Dot(axis, v);
                    if (proj < aMin) aMin = proj;
                    if (proj > aMax) aMax = proj;
                }

                foreach (var v in b)
                {
                    var proj = Vector2.Dot(axis, v);
                    if (proj < bMin) bMin = proj;
                    if (proj > bMax) bMax = proj;
                }

                if (aMax <= bMin || aMin >= bMax)
                    return false;
            }

            overlap.Width = Position.X > b[^1].X ? 1 : -1;
            overlap.Height = Position.Y > b[^1].Y ? -1 : 1;
            return true;
        }

        public Vector2 Size
        {
            get => new(_floatRect.Width, _floatRect.Height);
            set => (_floatRect.Width, _floatRect.Height) = (value.X, value.Y);
        }

        public Vector2 Position
        {
            get => new(_floatRect.Left, _floatRect.Top);
            set => (_floatRect.Left, _floatRect.Top) = (value.X, value.Y);
        }
    }

    public static class CollisionExtensions
    {
        public static bool IntersectsOrTouches(this FloatRect r1, FloatRect rect, out FloatRect overlap)
        {
            var r1MinX = Math.Min(r1.Left, r1.Left + r1.Width);
            var r1MaxX = Math.Max(r1.Left, r1.Left + r1.Width);

            var r1MinY = Math.Min(r1.Top, r1.Top + r1.Height);
            var r1MaxY = Math.Max(r1.Top, r1.Top + r1.Height);

            var r2MinX = Math.Min(rect.Left, rect.Left + rect.Width);
            var r2MaxX = Math.Max(rect.Left, rect.Left + rect.Width);

            var r2MinY = Math.Min(rect.Top, rect.Top + rect.Height);
            var r2MaxY = Math.Max(rect.Top, rect.Top + rect.Height);
            var interLeft = Math.Max(r1MinX, r2MinX);
            var interTop = Math.Max(r1MinY, r2MinY);
            var interRight = Math.Min(r1MaxX, r2MaxX);
            var interBottom = Math.Min(r1MaxY, r2MaxY);

            if (interLeft <= interRight && interTop <= interBottom)
            {
                overlap.Left = interLeft;
                overlap.Top = interTop;
                overlap.Width = interRight - interLeft;
                overlap.Height = interBottom - interTop;
                return true;
            }

            overlap.Left = 0;
            overlap.Top = 0;
            overlap.Width = 0;
            overlap.Height = 0;
            return false;
        }
    }
}