using TMPro;
using UnityEngine;

public class BusStopController : MonoBehaviour
{
    public TextMeshProUGUI text;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("Player")) return;

        if (GameProgressController.CanProgress())
        {
            text.SetText("Press E to Drive Away");
        }
        else
        {
            text.SetText("You need at least 200 coins to use the bus");
        }
        
        text.gameObject.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(!other.CompareTag("Player")) return;
        
        text.gameObject.SetActive(false);
    }

    private void Update()
    {
        if(!text.gameObject.activeSelf) return;
        if(MenuController.IsGamePaused()) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            EnterBus();
        }
    }

    private void EnterBus()
    {
        Debug.Log("Entering Bus");
    }
}
