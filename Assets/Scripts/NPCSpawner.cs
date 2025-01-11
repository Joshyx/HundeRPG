using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NPCSpawner : MonoBehaviour
{
    public List<GameObject> earlyGameNPCs;
    public List<GameObject> lateGameNPCs;
    public float spawnRate = 3f;
    public float spawnRadius = 30f;
    
    private float timeSinceLastSpawn;
    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (MenuController.IsGamePaused()) return;
        
        TrySpawnNPC();
    }

    private void TrySpawnNPC()
    {
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn < spawnRate) return;
        
        var npcCount = GameObject.FindGameObjectsWithTag("NPC").Length;
        if (npcCount >= MaxNPCsForLevel()) return;
        
        timeSinceLastSpawn = 0f;
        var npc = GetNPCToSpawn();

        Vector2 spawnPos;
        do
        { 
            spawnPos = Random.insideUnitSphere * spawnRadius + transform.position;
        } while (Vector2.Distance(spawnPos, player.transform.position) < 10f);
        
        Instantiate(npc, spawnPos, Quaternion.identity);
    }

    private int MaxNPCsForLevel()
    {
        var level = GameProgressController.GetLevel();
        return Mathf.RoundToInt(level * 0.3f + 7);
    }
    private GameObject GetNPCToSpawn()
    {
        var level = GameProgressController.GetLevel();

        var spawnEarlyGame = level switch
        {
            <= 4 => Random.value < 0.7f,
            <= 7 => Random.value < 0.5f,
            <= 11 => Random.value < 0.4f,
            _ => Random.value < 0.3f
        };

        return spawnEarlyGame ? earlyGameNPCs[Random.Range(0, earlyGameNPCs.Count)] : lateGameNPCs[Random.Range(0, lateGameNPCs.Count)];
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
