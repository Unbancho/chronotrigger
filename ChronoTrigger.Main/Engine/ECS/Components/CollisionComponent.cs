using System;
using System.Numerics;
using ModusOperandi.ECS.Components;
using SFML.Graphics;

// ReSharper disable UnassignedField.Global


namespace ChronoTrigger.Engine.ECS.Components
{
    public interface ISizeableComponent
    {
        public Vector2 Size { get; set; }
    }
    
    [Component]
    public struct CollisionComponent : ISizeableComponent, IDebugDrawableComponent
    {
        public Vector2 Hitbox;
        public Vector2 Offset;
        
        public Vector2 Size
        {
            get => Hitbox;
            set => Hitbox = value;
        }

        private float _rotation;
        public float Rotation
        {
            get => _rotation;
            set => _rotation = value * 0.0174533f;
        }

        public Drawable DebugDrawable => new Sprite(null, new(0, 0,
            (int) Hitbox.X, (int) Hitbox.Y))
        {
            Color = new(255, 0, 0, 128),
            Rotation = Rotation / 0.0174533f
        };
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
            set => Position = new(value, Position.Y);
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
            _sin = 0;
            Rotation = rotation;
        }

        public bool IntersectsOrTouches(RotatingRect r, out RotatingRect overlap)
        {
            if (Rotation != 0 || r.Rotation != 0) return IntersectsOrTouchesAngled(r, out overlap);
            var i = _floatRect.IntersectsOrTouches(r._floatRect, out var innerOverlap);
            overlap = new(innerOverlap);
            return i;
        }

        private readonly ref struct Points4
        {
            public readonly Vector2 A;
            public readonly Vector2 B;
            public readonly Vector2 C;
            public readonly Vector2 D;

            public Points4(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
            {
                A = a;
                B = b;
                C = c;
                D = d;
            }
        }

        public bool IntersectsOrTouchesAngled(RotatingRect r, out RotatingRect overlap)
        {
            overlap = default;
            var obb1 = AnglePoints(this);
            var obb2 = AnglePoints(r);

            Span<Vector2> axes = stackalloc Vector2[4]{
                new Vector2(obb1.B.X-obb1.A.X,
                    obb1.B.Y-obb1.A.Y),
                new Vector2(obb1.B.X-obb1.C.X,
                    obb1.B.Y-obb1.C.Y),
                new Vector2(obb2.A.X-obb2.D.X,
                    obb2.A.Y-obb2.D.Y),
                new Vector2(obb2.A.X-obb2.B.X,
                    obb2.A.Y-obb2.B.Y)
            };

            for (var i = 0; i<4; i++)
            {
                ProjectOntoAxis(obb1, axes[i], out var minObb1, out var maxObb1);
                ProjectOntoAxis(obb2, axes[i], out var minObb2, out var maxObb2);
                if (!(minObb2<=maxObb1 && maxObb2>=minObb1))
                    return false;
            }
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


        private static void ProjectOntoAxis(in Points4 points4, Vector2 axis, out float min, out float max)
        {
            Span<Vector2> points = stackalloc Vector2[4] {points4.A, points4.B, points4.C, points4.D};
            min = points[0].X * axis.X + points[0].Y * axis.Y;
            max = min;
            for (var j = 1; j < 4; j++)
            {
                var projection = points[j].X * axis.X + points[j].Y * axis.Y;

                if (projection < min)
                    min = projection;
                if (projection > max)
                    max = projection;
            }
        }
        

        private static Points4 AnglePoints(RotatingRect r)
        {
            Span<Vector2> points = stackalloc Vector2[4]
            {
                r.Position, new Vector2(r.Size.X, 0),
                new Vector2(r.Size.X, r.Size.Y),
                new Vector2(0, r.Size.Y)
            };
            
            for (var i = 1; i < points.Length; i++)
            {
                ref var v = ref points[i];
                var (x, y) = (v.X, v.Y);
                v.X = x * r._cos - y * r._sin + r.Position.X;
                v.Y = x * r._sin + y * r._cos + r.Position.Y;
            }

            return new (points[0], points[1], points[2], points[3]);
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