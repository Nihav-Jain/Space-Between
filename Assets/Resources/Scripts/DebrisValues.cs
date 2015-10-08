using UnityEngine;
using System.Collections;

public static class DebrisValues
{
    public static Vector2 debrisAreaXLimits = new Vector2(0.0f, 100.0f); //Box that debris can spawn in
    public static Vector2 debrisAreaYLimits = new Vector2(-50.0f, 50.0f);
    public static Vector2 debrisAreaZLimits = new Vector2(-50.0f, 50.0f);
    public static int debrisCountMax = 500; //How much debris in the scene at any point in time?
    public static string[] debrisMeshes = { "Meshes/Debris07", "Meshes/Debris08", 
        "Meshes/Debris10", "Meshes/Debris11", "Meshes/Debris12", "Meshes/Debris13", "Meshes/Debris14",
        "Meshes/Debris15", "Meshes/Debris16" }; //list of all meshes to load at any time
    public static float debrisRespawnTimeReductionRate = 0.7f; //respawn time goes down with each iteration
    public static float debrisSpeedGain = 0.0f; //Debris slowly gets faster over time
    public static int fastDebrisCountMax = 10;
    public static float fastRespawnTime = 1.0f;
    public static Vector3 fastOrbitVelocity = new Vector3(-500.0f, 0.0f, 0.0f);
    public static Vector2 orbitVelocity = new Vector3(-0.1f, -5.0f); //General bearing of all debris toward a certain direction
    public static float respawnTime = 90.0f; //How long to wait before debris comes back with greater speed (in seconds)
    public static Vector2 sizeMinMax = new Vector2(3.0f, 5.0f); //Range for size of debris
    public static Vector2 speedMinMax = new Vector2(0.1f, 3.0f); //Speeds of debris
    public static Vector2 turbulenceMinMax = new Vector2(1.0f, 3.0f); //Range for spin turbulence of debris
}
