using System.Collections;
using UnityEngine;

public class LandMine : MonoBehaviour
{
    public float explosionRadius = 1.5f;
    public GameObject explosionEffect;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("NPC")) return;
        
        var npcs = Physics2D.OverlapCircleAll(other.transform.position, 0.2f, LayerMask.GetMask("NPC"));
        foreach (var npc in npcs)
        {
            npc.GetComponent<NPCController>().TakeDamage(10f, false);
        }
        
        explosionEffect.SetActive(true);
        GetComponent<SpriteRenderer>().enabled = false;
        StartCoroutine(nameof(WaitForDestruction));
    }

    IEnumerator WaitForDestruction()
    {
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
