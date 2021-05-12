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
    public sealed class LeaderInputSystem : UpdateEntitySystem<GameLoop.GameState>
    {
        private static readonly Direction[] Directions = (Direction[]) Enum.GetValues(typeof(Direction));
        public int WalkingSpeed { get; set; } = 1;
        public int RunningSpeed { get; set; } = 2;

        private void UpdateComponent(LeaderComponent leaderComponent,ref MovementComponent movementComponent,
            GameLoop.GameState gameState)
        {
            if ((leaderComponent.Device & LeaderComponent.InputDevice.Analog) != 0)
            {
                Analogs(ref movementComponent, gameState);
                if(Vector2.Zero != movementComponent.Velocity) return;
            }
            if ((leaderComponent.Device & LeaderComponent.InputDevice.DPad) != 0)
            {
                DPad(ref movementComponent, gameState);
                if(Vector2.Zero != movementComponent.Velocity) return;
            }
            if ((leaderComponent.Device & LeaderComponent.InputDevice.Keyboard) != 0)
            {
                Keyboard(ref movementComponent, gameState);
            }
        }

        private void Keyboard(ref MovementComponent movementComponent, GameLoop.GameState gameState)
        {
            var finalDirection = default(Vector2);
            foreach (var direction in Directions)
            {
                if ((direction.ToButton() & gameState.InputState) == 0) continue;
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

            movementComponent.Velocity = finalDirection * ((Buttons.B & gameState.InputState) != 0 ? RunningSpeed : WalkingSpeed)
                                                        * gameState.DeltaTime;
        }

        private void DPad(ref MovementComponent movementComponent, GameLoop.GameState gameState)
        {
            movementComponent.Velocity = new Vector2(Joystick.GetAxisPosition(0, Joystick.Axis.PovX),
                -Joystick.GetAxisPosition(0, Joystick.Axis.PovY))/100 
                                         * gameState.DeltaTime 
                                         * ((Buttons.B & gameState.InputState) != 0  ? RunningSpeed : WalkingSpeed);
        }

        private void Analogs(ref MovementComponent movementComponent, GameLoop.GameState gameState)
        {
            var x = Joystick.GetAxisPosition(0, Joystick.Axis.X);
            var y = Joystick.GetAxisPosition(0, Joystick.Axis.Y);
            if(Math.Abs(x) < 25f && Math.Abs(y) < 25f)
                movementComponent.Velocity = Vector2.Zero;
            else
            {
                movementComponent.Velocity = new Vector2(x, y)/100 * gameState.DeltaTime 
                        * ((Buttons.B & gameState.InputState) != 0  ? RunningSpeed : WalkingSpeed);
            }
        }

        public override void ActOnEntity(Entity entity, GameLoop.GameState gameState)
        {
            UpdateComponent(entity.Get<LeaderComponent>(), 
                ref entity.Get<MovementComponent>(), gameState);
        }
    }
}