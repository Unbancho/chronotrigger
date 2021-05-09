using System;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace ChronoTrigger.Engine
{
    public abstract class GameLoop
    {
        public readonly ref struct GameState
        {
            public GameTime GameTime { get; init; }
            public RenderWindow Window { get; init; }
        }
        
        protected GameLoop(uint windowWidth, uint windowHeight, string windowTitle, Color windowClearColor)
        {
            WindowClearColor = windowClearColor;
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            Window = new(new(windowWidth, windowHeight), windowTitle, Styles.Default);
            // ReSharper disable once HeapView.DelegateAllocation
            Window.Closed += WindowClosed;
            Window.SetActive(false);
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            GameTime = new();
        }

        // ReSharper disable once MemberCanBeProtected.Global
        public int TargetFps { get; set; }

        private float TimeUntilUpdate => 1f / TargetFps;

        protected RenderWindow Window { get; }

        protected GameTime GameTime { get; }

        private Color WindowClearColor { get; }


        public void Run()
        {
            GC.Collect();
            DoRun();
        }

        private void DoRun()
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            var clock = new Clock();
            var timeSinceUpdate = 0f;
            while (Window.IsOpen)
            {
                Window.DispatchEvents();
                timeSinceUpdate += clock.Restart().AsSeconds();
                if (!(timeSinceUpdate >= TimeUntilUpdate)) continue;
                GameTime.Update(timeSinceUpdate);
                var state = new GameState()
                {
                    GameTime = GameTime,
                    Window = Window
                };
                Update(state);
                timeSinceUpdate = 0f;
                Window.Clear(WindowClearColor);
                Draw(state);
                Window.Display();
            }
        }

        public abstract void LoadContent(params object[] args);
        public abstract void Initialize(params object[] args);
        protected abstract void Update(GameState gameState);
        protected abstract void Draw(GameState gameState);

        private void WindowClosed(object sender, EventArgs e)
        {
            Window.Close();
        }
    }
}