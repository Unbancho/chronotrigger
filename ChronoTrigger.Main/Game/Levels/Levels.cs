namespace ChronoTrigger.Game.Levels
{
    public class TimePeriod
    {
        public string Name { get; set; }

        public Region[] Regions { get; private set; }
    }

    public class Region
    {
        public string Name { get; set; }

        public Location[] Locations { get; private set; }
    }

    public class Location
    {
    }
}