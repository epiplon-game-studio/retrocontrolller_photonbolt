using UnityEngine;

namespace Epiplon.Samples.Network
{
    public class SpawnPoint : MonoBehaviour
    {
        public static SpawnPoint[] spawnPoints;
        public static int spawnIndex = 0;

        public static SpawnPoint Get()
        {
            if (spawnPoints == null)
                spawnPoints = FindObjectsOfType<SpawnPoint>();

            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                var sp = spawnPoints[spawnIndex];
                spawnIndex++;
                if (spawnIndex >= spawnPoints.Length)
                    spawnIndex = 0;

                return sp;
            }

            throw new System.Exception("No Spawn Point found");
        }
    }
}
