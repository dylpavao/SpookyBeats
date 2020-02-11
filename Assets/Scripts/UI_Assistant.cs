using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Assistant : MonoBehaviour //make SINGLETON
{
    private Text messageText;
    private GameObject message;
    private bool inDialogue;
    private Queue<string> sentences;

    private Inventory inventory;

    private void Awake()
    {               
        message = GameObject.Find("Message");
        messageText = transform.Find("Message").Find("Text").GetComponent<Text>();
    }

    private void Start()
    {
        sentences = new Queue<string>();
        inDialogue = false;
        message.SetActive(false);
    }

    public void SetInventory(Inventory inventory)
    {
        this.inventory = inventory;
    }

    public void StartDialogue(Dialogue dialogue)
    {       
        Player.GetInstance().SetMoveable(false); //may be bad spot for this
        sentences.Clear();
        inDialogue = true;
        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        message.SetActive(true);
        DisplayNextSentence();
    }

    public bool DisplayNextSentence()
    {
        if(sentences.Count == 0)
        {
            EndDialogue();
            return true;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
        return false;
    }

    IEnumerator TypeSentence (string sentence)
    {
        messageText.text = "";

        foreach(char letter in sentence.ToCharArray())
        {
            messageText.text += letter;
            yield return new WaitForSeconds(0.025f);
        }
    }

    void EndDialogue()
    {
        
        Player.GetInstance().SetMoveable(true);
        inDialogue = false;
        message.SetActive(false);
    }

    public bool InDialogue()
    {
        return inDialogue;
    }

}
