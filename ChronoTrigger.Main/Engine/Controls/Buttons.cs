using System;
using ChronoTrigger.Game;
using SFML.Window;

namespace ChronoTrigger.Engine.Controls
{
    //[Flags]
    public enum Buttons : short
    {
        Left = 0b00,
        Up = 0b01,
        Right = 0b10,
        Down = 0b11,

        // Possibly add diagonal directional buttons.

        A = 0b1000,
        B = 0b1001,
        X = 0b1010,
        Y = 0b1011,
        L = 0b1100,
        R = 0b1101,
        Start = 0b1110,
        Select = 0b1111

        /*
        Right = 1 << 0,
        Left = 1 << 1,
        Up = 1 << 2,
        Down = 1 << 3,

        A = 1 << 4,
        B = 1 << 5,
        X = 1 << 6,
        Y = 1 << 7,
        L = 1 << 8,
        R = 1 << 9,
        Start = 1 << 10,
        Select = 1 << 11
        */
    }

    public static class ControlsExtensions
    {
        public static bool IsPressed(this Buttons button)
        {
            var controllerState = button switch
            {
                Buttons.Left => Joystick.GetAxisPosition(0, Joystick.Axis.PovX) < -50,
                Buttons.Up => Joystick.GetAxisPosition(0, Joystick.Axis.PovY) > 50,
                Buttons.Right => Joystick.GetAxisPosition(0, Joystick.Axis.PovX) > 50,
                Buttons.Down => Joystick.GetAxisPosition(0, Joystick.Axis.PovY) < -50,
                Buttons.A => Joystick.IsButtonPressed(0, 0),
                Buttons.B => Joystick.IsButtonPressed(0, 1),
                Buttons.X => Joystick.IsButtonPressed(0, 2),
                Buttons.Y => Joystick.IsButtonPressed(0, 3),
                Buttons.L => Joystick.IsButtonPressed(0, 4),
                Buttons.R => Joystick.IsButtonPressed(0, 5),
                Buttons.Start => Joystick.IsButtonPressed(0, 7),
                Buttons.Select => Joystick.IsButtonPressed(6, 0),
                _ => throw new ArgumentOutOfRangeException(nameof(button), button, null)
            };
            return Keyboard.IsKeyPressed(ChronoTriggerGame.KeyBindings[button]) || controllerState;
        }
    }
}