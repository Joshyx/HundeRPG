using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BusStopController : MonoBehaviour
{
    public TextMeshProUGUI text;
    public int coinsNeeded = 50;

    private void Start()
    {
        gameObject.SetActive(!MenuController.isInEndlessMode);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("Player")) return;

        if (GameProgressController.GetCoins() >= coinsNeeded)
        {
            text.SetText("Press E to Drive Away");
        }
        else
        {
            text.SetText("You need at least " + coinsNeeded + " coins to use the bus");
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
        if (SceneManager.GetActiveScene().buildIndex < SceneManager.sceneCountInBuildSettings - 1)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            return;
        }

        FindAnyObjectByType<MenuController>().FinishedGame();
    }
}
