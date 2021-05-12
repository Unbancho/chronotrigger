using System;
using System.Numerics;
using ModusOperandi.ECS.Components;
using SFML.Graphics;
using SFML.System;

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

        //TODO: Make this work.
        public bool IntersectsOrTouchesAngled(RotatingRect r, out RotatingRect overlap)
        {
            overlap = default;
            return BoundingBoxTest(this, r);
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

        private readonly ref struct OrientedBoundingBox
		{
			public OrientedBoundingBox (RotatingRect rect)
            {
                Points =  AnglePoints(rect);
            }

            public readonly Vector2[] Points;

            public void ProjectOntoAxis (Vector2 axis, out float min, out float max)
			{
				min = Points[0].X*axis.X+Points[0].Y*axis.Y;
				max = min;
				for (var j = 1; j<4; j++)
				{
					var projection = Points[j].X*axis.X+Points[j].Y*axis.Y;

					if (projection<min)
						min=projection;
					if (projection>max)
						max=projection;
				}
			}
            
            private static Vector2[] AnglePoints(RotatingRect r)
            {
                var points = new[]
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

                return points;
            }
        }

        private static bool BoundingBoxTest(RotatingRect object1, RotatingRect object2) 
        {
            OrientedBoundingBox obb1 = new(object1);
			OrientedBoundingBox obb2 = new(object2);

			Vector2[] axes = {
				new (obb1.Points[1].X-obb1.Points[0].X,
				obb1.Points[1].Y-obb1.Points[0].Y),
				new (obb1.Points[1].X-obb1.Points[2].X,
				obb1.Points[1].Y-obb1.Points[2].Y),
				new (obb2.Points[0].X-obb2.Points[3].X,
				obb2.Points[0].Y-obb2.Points[3].Y),
				new (obb2.Points[0].X-obb2.Points[1].X,
				obb2.Points[0].Y-obb2.Points[1].Y)
                
            };

			for (var i = 0; i<4; i++) // For each axis...
			{
				obb1.ProjectOntoAxis(axes[i], out var minObb1, out var maxObb1);
				obb2.ProjectOntoAxis(axes[i], out var minObb2, out var maxObb2);

				if (!(minObb2<=maxObb1 && maxObb2>=minObb1))
					return false;
			}
            return true;
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