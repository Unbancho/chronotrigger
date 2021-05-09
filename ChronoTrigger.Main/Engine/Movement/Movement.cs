using System;
using System.Numerics;
using ChronoTrigger.Engine.Controls;

namespace ChronoTrigger.Engine.Movement
{
    public enum Direction : byte
    {
        Left = 0b00,
        Up = 0b01,
        Right = 0b10,
        Down = 0b11
    }

    public static class DirectionExtensions
    {
        public static Direction ToDirection(this Vector2 directionVector)
        {
            var absX = MathF.Abs(directionVector.X);
            var absY = MathF.Abs(directionVector.Y);
            return absX > absY ? directionVector.X < 0 ? Direction.Left :
                Direction.Right :
                directionVector.Y < 0 ? Direction.Up : Direction.Down;
        }

        public static Vector2 DirectionToVector2(this Direction direction)
        {
            return direction switch
            {
                Direction.Left => -Vector2.UnitX,
                Direction.Up => -Vector2.UnitY,
                Direction.Right => Vector2.UnitX,
                Direction.Down => Vector2.UnitY,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
        }

        public static Buttons ToButton(this Direction d)
        {
            return d switch
            {
                Direction.Left => Buttons.Left,
                Direction.Up => Buttons.Up,
                Direction.Right => Buttons.Right,
                Direction.Down => Buttons.Down,
                _ => throw new ArgumentOutOfRangeException(nameof(d), d, null)
            };
        }
    }
}