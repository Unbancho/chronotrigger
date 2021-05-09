using System;
using System.Numerics;
using ChronoTrigger.Engine.Controls;
using ChronoTrigger.Engine.ECS.Components;
using ChronoTrigger.Engine.Movement;
using ModusOperandi.ECS.Entities;
using ModusOperandi.ECS.Systems;
using ModusOperandi.ECS.Systems.SystemAttributes;
using SFML.Window;

namespace ChronoTrigger.Engine.ECS.Systems.UpdateSystems
{
    [UpdateSystem]
    [Include(typeof(MovementComponent))]
    [Include(typeof(LeaderComponent))]
    public sealed class LeaderInputSystem : UpdateEntitySystem
    {
        private static readonly Direction[] Directions = (Direction[]) Enum.GetValues(typeof(Direction));
        public int WalkingSpeed { get; set; } = 1;
        public int RunningSpeed { get; set; } = 2;


        public override void ActOnEntity(Entity entity, float deltaTime)
        {
            UpdateComponent(entity.Get<LeaderComponent>(), ref entity.Get<MovementComponent>(), deltaTime);
        }

        private void UpdateComponent(LeaderComponent leaderComponent,ref MovementComponent movementComponent, float deltaTime)
        {
            if ((leaderComponent.Device & LeaderComponent.InputDevice.Analog) != 0)
            {
                Analogs(ref movementComponent, deltaTime);
                if(Vector2.Zero != movementComponent.Velocity) return;
            }
            if ((leaderComponent.Device & LeaderComponent.InputDevice.DPad) != 0)
            {
                DPad(ref movementComponent, deltaTime);
                if(Vector2.Zero != movementComponent.Velocity) return;
            }
            if ((leaderComponent.Device & LeaderComponent.InputDevice.Keyboard) != 0)
            {
                Keyboard(ref movementComponent, deltaTime);
            }
        }

        private void Keyboard(ref MovementComponent movementComponent, float deltaTime)
        {
            var finalDirection = default(Vector2);
            foreach (var direction in Directions)
            {
                if (!direction.ToButton().IsPressed()) continue;
                var d = (byte) direction;
                var p = -1 + (d & 2);
                finalDirection.X += (~d & 1) * p;
                finalDirection.Y += (d & 1) * p;
            }

            if (finalDirection == default)
            {
                movementComponent.Velocity *= 0;
                return;
            }

            movementComponent.Velocity = finalDirection * (Buttons.B.IsPressed() ? RunningSpeed : WalkingSpeed)
                                                        * deltaTime;
        }

        private void DPad(ref MovementComponent movementComponent, float deltaTime)
        {
            movementComponent.Velocity = new Vector2(Joystick.GetAxisPosition(0, Joystick.Axis.PovX),
                -Joystick.GetAxisPosition(0, Joystick.Axis.PovY))/100 
                                         * deltaTime 
                                         * (Buttons.B.IsPressed() ? RunningSpeed : WalkingSpeed);
        }

        private void Analogs(ref MovementComponent movementComponent, float deltaTime)
        {
            var x = Joystick.GetAxisPosition(0, Joystick.Axis.X);
            var y = Joystick.GetAxisPosition(0, Joystick.Axis.Y);
            if(Math.Abs(x) < 25f && Math.Abs(y) < 25f)
                movementComponent.Velocity = Vector2.Zero;
            else
            {
                movementComponent.Velocity = new Vector2(x, y)/100 * deltaTime 
                        * (Buttons.B.IsPressed() ? RunningSpeed : WalkingSpeed);
            }
        }
    }
}