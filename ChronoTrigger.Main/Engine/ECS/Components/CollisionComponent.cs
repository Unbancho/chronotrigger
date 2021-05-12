using System;
using System.Numerics;
using ChronoTrigger.Engine.ECS.Systems.DrawSystems;
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
            var spriteA = new Sprite(null, new(0, 0,
                (int) Width, (int) Height))
            {
                Rotation = Rotation / 0.0174533f,
                Position = Position.ToVector2f()
            };

            var spriteB = new Sprite(null, new(0, 0,
                (int) r.Width, (int) r.Height))
            {
                Rotation = r.Rotation / 0.0174533f,
                Position = r.Position.ToVector2f()
            };
            overlap = default;
            return BoundingBoxTest(spriteA, spriteB);
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
        

        public readonly struct OrientedBoundingBox // Used in the BoundingBoxTest
		{
			public OrientedBoundingBox (Sprite @object) // Calculate the four points of the OBB from a transformed (scaled, rotated...) sprite
			{
				var trans = @object.Transform;
				var local = @object.TextureRect;
                Points = new Vector2f[4];
                Points[0] = trans.TransformPoint(0f, 0f);
				Points[1] = trans.TransformPoint(local.Width, 0f);
				Points[2] = trans.TransformPoint(local.Width, local.Height);
				Points[3] = trans.TransformPoint(0f, local.Height);
			}

            public readonly Vector2f[] Points;

            public void ProjectOntoAxis (Vector2f axis, ref float min, ref float max) // Project all four points of the OBB onto the given axis and return the dotproducts of the two outermost points
			{
				min = (Points[0].X*axis.X+Points[0].Y*axis.Y);
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
        }

		bool BoundingBoxTest(Sprite object1, Sprite object2) 
        {
			OrientedBoundingBox obb1 = new(object1);
			OrientedBoundingBox obb2 = new(object2);

			// Create the four distinct axes that are perpendicular to the edges of the two rectangles
			Vector2f[] axes = {
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
				float minObb1 = 0f, maxObb1 = 0f, minObb2 = 0f, maxObb2 = 0f;

				// ... project the points of both OBBs onto the axis ...
				obb1.ProjectOntoAxis(axes[i], ref minObb1, ref maxObb1);
				obb2.ProjectOntoAxis(axes[i], ref minObb2, ref maxObb2);

				// ... and check whether the outermost projected points of both OBBs overlap.
				// If this is not the case, the Separating Axis Theorem states that there can be no collision between the rectangles
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