using TMPro;
using UnityEngine;

public class BusStopController : MonoBehaviour
{
    public TextMeshProUGUI text;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("Player")) return;
        
        text.SetText("Press E to Level Up");
        
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
