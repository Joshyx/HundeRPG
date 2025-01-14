using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NPCSpawner : MonoBehaviour
{
    public List<GameObject> earlyGameNPCs;
    public List<GameObject> lateGameNPCs;
    public float spawnRate = 3f;
    public float spawnRadius = 50f;
    
    private float timeSinceLastSpawn;

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
        } while (IsPointInView(spawnPos) || IsPointOnCollider(spawnPos));
        
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
            <= 1 => true,
            <= 4 => Random.value < 0.8f,
            <= 7 => Random.value < 0.6f,
            <= 11 => Random.value < 0.5f,
            _ => Random.value < 0.3f
        };

        return spawnEarlyGame ? earlyGameNPCs[Random.Range(0, earlyGameNPCs.Count)] : lateGameNPCs[Random.Range(0, lateGameNPCs.Count)];
    }

    private bool IsPointOnCollider(Vector2 point)
    {
        return Physics2D.OverlapCircle(point, 1f, LayerMask.GetMask("Environment"));
    }
    private bool IsPointInView(Vector2 pos)
    {
        var viewPos = Camera.main.WorldToViewportPoint(pos);
        return viewPos.x is <= 1 and >= 0 && viewPos.y is <= 1 and >= 0;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
