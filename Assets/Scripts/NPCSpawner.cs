using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NPCSpawner : MonoBehaviour
{
    public List<GameObject> npcs;
    public float spawnRate = 3f;
    public float spawnRadius = 30f;
    public int maxNpcs = 7;
    
    private float timeSinceLastSpawn;
    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        TrySpawnNPC();
    }

    private void TrySpawnNPC()
    {
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn < spawnRate) return;
        
        var npcCount = GameObject.FindGameObjectsWithTag("NPC").Length;
        if (npcCount >= maxNpcs) return;
        
        timeSinceLastSpawn = 0f;
        var npc = npcs[Random.Range(0, npcs.Count)];

        Vector2 spawnPos;
        do
        { 
            spawnPos = Random.insideUnitSphere * spawnRadius + transform.position;
        } while (Vector2.Distance(spawnPos, player.transform.position) < 10f);
        
        Instantiate(npc, spawnPos, Quaternion.identity);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
