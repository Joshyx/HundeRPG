using UnityEngine;

public class GameProgressController : MonoBehaviour
{
    private PlayerController player;
    
    public GameObject animalCatcher;
    public float spawnRadius = 30f;
    public Wave[] waves =
    {
        new Wave(0.2f, 1),
        new Wave(0.5f, 3),
        new Wave(0.8f, 4),
        new Wave(1f, 4),
    };

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    private void Update()
    {
        if(MenuController.IsGamePaused()) return;
        
        foreach (var wave in waves)
        {
            if(wave.spawned || player.GetProgress() < wave.neededProgress) continue;

            for (int i = 0; i < wave.spawnCount; i++)
            {
                SpawnObjectInRandomLocation(animalCatcher, spawnRadius);
            }
            wave.spawned = true;
        }
    }

    private void SpawnObjectInRandomLocation(GameObject obj, float radius)
    {
        Vector2 spawnPos;
        do
        { 
            spawnPos = Random.insideUnitSphere * radius + transform.position;
        } while (Vector2.Distance(spawnPos, player.transform.position) < 10f);
            
        Instantiate(obj, spawnPos, Quaternion.identity);
    }
    
    public class Wave
    {
        public float neededProgress;
        public int spawnCount;
        public bool spawned;

        public Wave(float neededProgress, int spawnCount)
        {
            this.spawnCount = spawnCount;
            this.neededProgress = neededProgress;
        }
    }
}
