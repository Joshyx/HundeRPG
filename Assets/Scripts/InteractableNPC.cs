using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class InteractableNPC : MonoBehaviour
{
    public TextMeshProUGUI dialogText;
    [TextArea]
    public string[] sentences;
    
    private float interactDistance = 2f;
    private GameObject player;
    private NPCMovement movement;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        movement = GetComponent<NPCMovement>();
    }

    private void Update()
    {
        if (MenuController.IsGamePaused()) return;

        if (movement.GetState() != NPCMovement.MovementState.IDLE)
        {
            dialogText.gameObject.SetActive(false);
            return;
        }
        if (speaking) return;
        if (Vector3.Distance(player.transform.position, transform.position) > interactDistance)
        {
            dialogText.gameObject.SetActive(false);
            return;
        };
        dialogText.gameObject.SetActive(true);
        dialogText.text = "Press 'E' to interact";
        if (!Input.GetKeyDown(KeyCode.E)) return;
        
        StopAllCoroutines();
        StartCoroutine(nameof(Speak));
    }
    
    private bool speaking;
    private IEnumerator Speak()
    {
        speaking = true;
        var sentence = sentences[Random.Range(0, sentences.Length)];
        dialogText.gameObject.SetActive(true);

        for (var i = 0; i < sentence.Length; i++)
        {
            var text = sentence.Substring(0, i + 1);
            dialogText.text = text;
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(5f);
        dialogText.gameObject.SetActive(false);
        speaking = false;
    }
}
