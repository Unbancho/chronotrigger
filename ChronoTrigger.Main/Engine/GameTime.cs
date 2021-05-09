namespace ChronoTrigger.Engine
{
    public class GameTime
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public float TimeScale { get; set; }

        public float DeltaTime => DeltaTimeUnscaled * TimeScale;

        // ReSharper disable once InconsistentNaming
        public float FPS => 1f / DeltaTimeUnscaled;

        private float DeltaTimeUnscaled { get; set; }

        // ReSharper disable once MemberCanBePrivate.Global
        public float TotalTimeElapsed { get; private set; }

        public void Update(float deltaTime)
        {
            DeltaTimeUnscaled = deltaTime;
            TotalTimeElapsed += deltaTime;
        }
    }
}