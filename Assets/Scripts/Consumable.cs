using System;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

public class Consumable : MonoBehaviour
{
    public string consumableName;
    public float speedMultiplier = 1f;
    public float damageMultiplier = 1f;
    public int coins = 0;
    public int health = 0;
    public float effectDurationSeconds = 5f;
    public TextMeshProUGUI pickupText;
    
    private PlayerController player;
    private float interactDistance = 2f;

    private void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
        pickupText.text = "Press 'E' to pick up " + consumableName;
    }

    private void Update()
    {
        var distance = Vector2.Distance(transform.position, player.transform.position);
        if (distance > interactDistance)
        {
            pickupText.gameObject.SetActive(false);
            return;
        }

        var others = FindObjectsByType<Consumable>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).ToList().FindAll(other 
            => Vector2.Distance(player.transform.position, other.transform.position) < distance);

        if (others.Count > 0)
        {
            pickupText.gameObject.SetActive(false);
            return;
        }
        
        pickupText.gameObject.SetActive(true);

        if (!Input.GetKeyDown(KeyCode.E)) return;
        player.Consume(this);
        GetComponentInChildren<ParticleSystem>().Play();
        GetComponent<SpriteRenderer>().enabled = false;
        Destroy(gameObject, 0.2f);
    }
}
