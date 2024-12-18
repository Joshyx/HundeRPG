using TMPro;
using UnityEngine;

public class BusStopController : MonoBehaviour
{
    public TextMeshProUGUI text;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("Player")) return;
        var player = other.GetComponent<PlayerController>();
        if (player.CanLevelUp())
        {
            text.SetText("Press R to Level Up");
        }
        else
        {
            text.SetText("Not enough XP to Level Up");
        }
        
        text.gameObject.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(!other.CompareTag("Player")) return;
        
        text.gameObject.SetActive(false);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(!other.CompareTag("Player")) return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            EnterBus();
        }
    }

    private void EnterBus()
    {
        
    }
}
