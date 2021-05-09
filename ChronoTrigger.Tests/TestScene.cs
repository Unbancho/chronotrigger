using System;
using ModusOperandi.ECS.Scenes;
using ModusOperandi.ECS.Systems.SystemInterfaces;

namespace ChronoTrigger.Tests
{
    public class TestScene : Scene
    {
        public override void Initialize()
        {
        }
    }

    public class TestScene<T> : TestScene where T: ISystem, new()
    {
        public TestScene()
        {
            ToggleSystem<T>();
        }
    }
}