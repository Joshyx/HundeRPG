using UnityEngine;

public class LandMine : MonoBehaviour
{
    public float explosionRadius = 1.5f;
    public AudioClip explosionAudio;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("NPC")) return;
        
        var npcs = Physics2D.OverlapCircleAll(other.transform.position, 0.2f, LayerMask.GetMask("NPC"));
        float xp = 0f;
        foreach (var npc in npcs)
        {
            var contr = npc.GetComponent<NPCController>();
            contr.TakeDamage(10f, false);
            xp += 5f;
        }
        
        GameProgressController.AddXP(xp);
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
