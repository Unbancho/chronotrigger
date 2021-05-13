using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ChronoTrigger.Engine;
using ChronoTrigger.Engine.Controls;
using ChronoTrigger.Engine.ECS.Components;
using ChronoTrigger.Engine.ECS.Systems.DrawSystems;
using ChronoTrigger.Engine.ECS.Systems.SfmlSystems;
using ChronoTrigger.Engine.ECS.Systems.UpdateSystems;
using ChronoTrigger.Engine.ResourceManagement;
using ChronoTrigger.Extensions;
using JetBrains.Annotations;
using ModusOperandi.ECS;
using ModusOperandi.ECS.Entities;
using ModusOperandi.ECS.Scenes;
using ModusOperandi.ECS.Systems.SystemInterfaces;
using ModusOperandi.Utils.YAML;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Color = SFML.Graphics.Color;

namespace ChronoTrigger.Game
{
    public class ChronoTriggerGame : GameLoop
    {
        private const string WindowTitle = "Chrono Trigger .NET";

        // ReSharper disable once MemberCanBePrivate.Global
        public readonly RealSceneManager SceneManager = new();

        public ChronoTriggerGame() : base(
            VideoMode.DesktopMode.Width / 2,
            VideoMode.DesktopMode.Height / 2,
            WindowTitle,
            Color.Black)
        {
            TargetFps = 144;
            GameTime.TimeScale = 120;
            // ReSharper disable once HeapView.DelegateAllocation
            Window.KeyPressed += OnKeyPressed;
            Window.MouseButtonPressed += MouseButtonPressed;

            ConsoleStuff();
        }

        private static async void ConsoleStuff()
        {
            static async Task<string> GetInputAsync()
            {
                return await Task.Run(Console.ReadLine);
            }

            while (true)
            {
                var input = await GetInputAsync();
                ConsoleEngine.ConsoleEngine.EvaluateScript(input);
            }
        }

        public static Entity SelectedEntity { get; private set; }
        private void MouseButtonPressed(object sender, MouseButtonEventArgs args)
        {
            var mousePosition = Mouse.GetPosition(Window);
            var realMousePosition = Window.MapPixelToCoords(mousePosition);
            switch (args.Button)
            {
                case Mouse.Button.Left:
                    DoLeftClick(realMousePosition);
                    break;
                case Mouse.Button.Right:
                    DoRightClick(mousePosition, realMousePosition);
                    break;
                case Mouse.Button.Middle:
                    break;
                case Mouse.Button.XButton1:
                    break;
                case Mouse.Button.XButton2:
                    break;
                case Mouse.Button.ButtonCount:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void DoLeftClick(Vector2f mousePos)
        {
            SelectedEntity = PositionMapSystem.EntityScreenPositions.TryGetValue(
                new((int) mousePos.X, (int) mousePos.Y), out var entity)
                ? entity
                : default;
        }

        private void DoRightClick(Vector2i mousePosition, Vector2f realMousePosition)
        {
            var option = Window.ShowContextMenu(RightClickMenu, mousePosition);
            switch (option)
            {
                case 1:
                    break;
                case 2:
                    ref var pos = ref SelectedEntity.Get<TransformComponent>().Position; 
                    pos = new (realMousePosition.X, realMousePosition.Y);
                    break;
                case 3:
                    ref var sprite = ref SelectedEntity.Get<SpriteComponent>();
                    sprite.Scale = Vector2.Zero;
                    break;
                default:
                    break;
            }
        }

        private static readonly IntPtr RightClickMenu = WindowExtensions.CreateContextMenu(new []
        {
            "Option1 ", "Move", "Hide", "Option 4", "Option 5"
        });
        

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        public static Dictionary<Buttons, Keyboard.Key> KeyBindings { get; } = new();
        private static void ReadKeyBindings()
        {
            var keybindings = Yaml.Deserialize<string, string>($"{Directories.BaseDirectory}keybindings.yaml");
            foreach (var (control, key) in keybindings)
                KeyBindings[Enum.Parse<Buttons>(control)] = Enum.Parse<Keyboard.Key>(key);
        }

        private static void InitializedYaml()
        {
            Yaml.RegisterTagMapping<AnimationComponent.AnimationYaml>("!AnimationYaml");
            Yaml.RegisterTagMapping<IntRect>("!IntRect");
            Yaml.RegisterTagMapping<Vector2>("!vector2");
            Yaml.DeserializerBuilder.WithNamingConvention(PascalCaseNamingConvention.Instance);
            Yaml.RegisterComponentMappings();
            Yaml.BuildDeserializer();
        }

        public override void LoadContent(params object[] args)
        {
            InitializedYaml();
            ReadKeyBindings();
            ResourceManager.LoadFonts();
            var sceneFolders = Directory.EnumerateDirectories(Directories.ScenesDirectory)
                .Select(Path.GetFileName).ToList();
            SceneNames = sceneFolders.ToArray();
            if (args.Length < 1) return;
            var sceneName = args[0];
            var scene = new SceneBuilder().BuildScene(
                Yaml.Deserialize<object, object>($"{Directories.ScenesDirectory}{sceneName}/{sceneName}.yaml"));
            SceneManager.ActivateScene(scene);
        }
        
        public static string[] SceneNames { get; private set; }

        public static void Save()
        {
            var cms = Ecs.GetComponentManagers();
            var sb = new SerializerBuilder();
            var s = sb.Build();
            var result = "";
            foreach (var cm in cms)
            //foreach (var component in cm.Components)
            //    result += s.Serialize(component);
            File.WriteAllText($@"{Directories.ScenesDirectory}/result.yaml", result);
        }

        public override void Initialize(params object[] args)
        {
            ShowWindow(Window.SystemHandle, 3);
        }

        protected override void Update(GameState gameState)
        {
            foreach (var scene in SceneManager.ActiveScenes) scene.Update(gameState);
            Joystick.Update();
        }

        private void OnKeyPressed(object sender, KeyEventArgs args)
        {
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (args.Code)
            {
                case Keyboard.Key.Pause:
                    GameTime.TimeScale = 0f;
                    break;
                case Keyboard.Key.PageUp:
                    TargetFps *= 2;
                    break;
                case Keyboard.Key.PageDown:
                    TargetFps /= 2;
                    break;
                case Keyboard.Key.F2:
                    foreach (var scene in SceneManager.ActiveScenes)
                    {
                        scene.PlaceEntity("Robo", new(0, 0));
                    }
                    break;
                case Keyboard.Key.F3:
                    foreach (var scene in SceneManager.ActiveScenes)
                    {
                        scene.ToggleSystem<MouserHoverDebugSystem>();
                        scene.ToggleSystem<DebugDrawCollisionSystem>();
                        scene.ToggleSystem<DebugDrawSystem<TransformComponent>>();
                        scene.ToggleSystem<PositionMapSystem>();
                        scene.ToggleSystem<EntityDebugSystem>();
                    }

                    break;
                case Keyboard.Key.F4:
                    break;
                case Keyboard.Key.F5:
                    Save();
                    break;
            }
        }
        
        protected override void Draw(GameState state)
        {
            foreach (var scene in SceneManager.ActiveScenes) Window.Draw(scene);

            RunSfmlSystems(state);
        }

        private static void RunSfmlSystems(GameState state)
        {
            foreach (var system in Scene.GetSystems<SfmlSystemAttribute>())
            {
                ((ISystem<GameState>)system).Run(state);
            }
        }

        [UsedImplicitly]
        public void CloseWindow(object o)
        {
            Window.Close();
        }
        
        [DllImport("User32.dll")]
        private static extern bool ShowWindow(
            IntPtr hWnd,
            int  nCmdShow
        );
    }
}