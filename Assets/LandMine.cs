using UnityEngine;

public class LandMine : MonoBehaviour
{
    public float explosionRadius = 1.5f;
    public AudioClip explosionAudio;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("NPC")) return;
        
        var npcs = Physics2D.OverlapCircleAll(other.transform.position, 0.2f, LayerMask.GetMask("NPC"));
        foreach (var npc in npcs)
        {
            npc.GetComponent<NPCController>().TakeDamage(10f, false);
        }
        
        GetComponentInChildren<ParticleSystem>().Play();
        GetComponent<SpriteRenderer>().enabled = false;
        AudioSource.PlayClipAtPoint(explosionAudio, transform.position);
        
        Destroy(gameObject, 0.2f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
