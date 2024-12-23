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
        if(MenuController.IsGamePaused()) return;

        if (movement.GetState() != NPCMovement.MovementState.IDLE) return;
        if (!Input.GetKeyDown(KeyCode.E)) return;
        if (Vector3.Distance(player.transform.position, transform.position) > interactDistance) return;
        
        StopAllCoroutines();
        StartCoroutine(nameof(Speak));
    }

    private IEnumerator Speak()
    {
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
    }
}
