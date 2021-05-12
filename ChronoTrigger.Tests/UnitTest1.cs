using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using ChronoTrigger.Engine.ECS.Components;
using ChronoTrigger.Engine.ECS.Systems.UpdateSystems;
using ChronoTrigger.Engine.Movement;
using ModusOperandi.ECS;
using ModusOperandi.ECS.Components;
using ModusOperandi.ECS.Entities;
using Xunit;
using Xunit.Abstractions;

namespace ChronoTrigger.Tests
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public UnitTest1(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void ComponentSizeTest()
        {
            const int maxSize = 48;
            var failedTypes = new List<(Type, int)>();
            foreach (var componentType in Assembly.LoadFrom("ChronoTrigger.dll").GetTypes().Where(type => Attribute.IsDefined(type, typeof(Component))))
            {
                var size = Marshal.SizeOf(componentType);
                if(size > maxSize) failedTypes.Add((componentType, size));
                _testOutputHelper.WriteLine($"{componentType.Name}: {size}");
            }
            Assert.True(failedTypes.Count < 1, failedTypes.
                Aggregate("", (current, d) => current + $"{d.Item1.Name}: {d.Item2}bytes\n"));
        }
    }

    public class SystemTests
    {
        //private readonly ChronoTriggerGame _game = new ChronoTriggerGame("");
        
        [Theory]
        [InlineData(3, Direction.Right, 60)]
        [InlineData(2, Direction.Left, 60)]
        [InlineData(1.5f, Direction.Down, 60)]
        [InlineData(1, Direction.Down, 60)]
        [InlineData(0, Direction.Up, 60)]
        public void MovementTest(float speed, Direction direction, float frameRate)
        {
            var directionVector = direction.DirectionToVector2();
            var expected = speed * frameRate * directionVector;
            var movingEntity = new Entity();
            Ecs.RegisterComponent(movingEntity, new TransformComponent());
            Ecs.RegisterComponent(movingEntity, new MovementComponent());
            movingEntity.Get<MovementComponent>().Velocity = directionVector * speed;
            var movementSystem = new MovementSystem();
            for (var i = 0; i < frameRate; i++)
            {
                throw new NotImplementedException();
                //movementSystem.ActOnEntity(movingEntity, 1f/frameRate);
            }

            var actual = movingEntity.Get<TransformComponent>().Position;
            Assert.True(actual == expected, $"Expected:{expected}; Actual:{actual}");
        }
        
        /*
        [Theory]
        [InlineData(3, Direction.Right, 60)]
        [InlineData(2, Direction.Left, 60)]
        [InlineData(1.5f, Direction.Down, 60)]
        [InlineData(1, Direction.Down, 60)]
        [InlineData(0, Direction.Up, 60)]
        public void FollowTest(Vector2 targetPosition, float distanceToKeep, float frameRate)
        {
            var expected = distanceToKeep;
            var targetEntity = new Entity();
            Ecs.RegisterComponent(targetEntity, new TransformComponent());
            targetEntity.Get<TransformComponent>().Position = targetPosition;
            var followerEntity = new Entity();
            Ecs.RegisterComponent(followerEntity, new TransformComponent());
            Ecs.RegisterComponent(followerEntity, new MovementComponent());
            Ecs.RegisterComponent(followerEntity, new FollowerComponent());
            followerEntity.Get<MovementComponent>().Speed = speed;
            followerEntity.Get<MovementComponent>().Direction = directionVector;
            var movementSystem = new MovementSystem();
            for (var i = 0; i < frameRate; i++)
            {
                movementSystem.ActOnEntity(followerEntity, 1f/frameRate);
            }

            var actual = followerEntity.Get<TransformComponent>().Position;
            Assert.True(actual == expected, $"Expected:{expected}; Actual:{actual}");
        }
        */
    }
}