using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimberWorld : MonoBehaviour {
    public static readonly TimberSeason WET_SEASON = new TimberSeason(
        name: "WET",
        durationIsSeconds: 10,
        providesWater: true
    );

    public static readonly TimberSeason DRY_SEASON = new TimberSeason(
        name: "DRY",
        durationIsSeconds: 10,
        providesWater: false
    );

    private TimberSeason currentSeason = WET_SEASON;
    private int currentSeasonStartedAt = 0;
    public int currentSeasonDurationInSeconds;

    public float secondsClock = 0;

    public Material mapMaterial;

    private TimberMap map;

    void Start() {
        this.map = new TimberMap(
            groundHeights: new int[]{
                0, 0, 0, 1,
                0, 1, 1, 0
            },
            width: 4,
            height: 2,
            meshMaterial: this.mapMaterial
        );
        this.map.computeVerticesAndTriangles();
        this.map.CreateMesh();
        // StartCoroutine(this.map.BuildDebugMesh());
    }

    void Update() {
        this.secondsClock += Time.deltaTime;

        int timeInSeason = Mathf.RoundToInt(this.secondsClock - currentSeasonStartedAt);

        if (timeInSeason >= this.currentSeason.getDurationInSeconds()) {
            string currentSeasonName = this.currentSeason.getName();
            if (TimberWorld.WET_SEASON.getName() == currentSeasonName) {
                this.currentSeason = TimberWorld.DRY_SEASON;
            } else if (TimberWorld.DRY_SEASON.getName() == currentSeasonName) {
                this.currentSeason = TimberWorld.WET_SEASON;
            } else {
                throw new System.Exception(currentSeasonName);
            }
            this.currentSeasonStartedAt = Mathf.RoundToInt(this.secondsClock);
            // Debug.Log("Changed to season: " + this.currentSeason.getName());
        }
    }

    private void startSeason() {
        this.currentSeasonDurationInSeconds = 60;
    }
}
