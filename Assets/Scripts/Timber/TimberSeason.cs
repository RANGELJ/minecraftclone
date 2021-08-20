public class TimberSeason {
    private int durationIsSeconds;
    private string name;

    private bool providesWater;

    public TimberSeason(
        int durationIsSeconds,
        string name,
        bool providesWater
    ) {
        this.durationIsSeconds = durationIsSeconds;
        this.name = name;
        this.providesWater = providesWater;
    }

    public int getDurationInSeconds() {
        return this.durationIsSeconds;
    }

    public string getName() {
        return this.name;
    }

    public bool getProvidesWater() {
        return this.providesWater;
    }
}
