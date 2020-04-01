using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Assistant : MonoBehaviour //make SINGLETON
{
    private Text messageText;
    private GameObject message;
    private GameObject menu;
    private bool inDialogue;
    private bool inMenu;
    private Queue<string> sentences;
    private Text[] menuTextBoxes; 
    private int menuCursor;
    private Inventory inventory;
    private readonly string[] pauseMenuItems = new string[] { "Inventory", "Stats", "Save", "Quit" };
    private string[] menuText;
    private bool typing;
    private string currentSentence;

    private void Awake()
    {
        menu = GameObject.Find("Menu");
        message = GameObject.Find("Message");
        messageText = transform.Find("Message").Find("Text").GetComponent<Text>();
        

        menuTextBoxes = new Text[4];
        for(int i = 0; i < menuTextBoxes.Length; i++)
        {
            menuTextBoxes[i] = transform.Find("Menu").Find("Text" + i).GetComponent<Text>();
        }

        //int d = 0;
        //menus = new Text[2, 2];
        //for(int i = 0; i < menuTextBoxes.GetLength(0); i++)
        //{
        //    for (int j = 0; j < menuTextBoxes.GetLength(1); j++)
        //    {
        //        menus[i,j] = transform.Find("Menu").Find("Text" + d).GetComponent<Text>();
        //        d++;
        //    }
        //}
    }

    private void Start()
    {
        menuCursor = 0;
        sentences = new Queue<string>();
        inDialogue = false;
        inMenu = false;
        message.SetActive(false);
        menu.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !inDialogue && !Player.GetInstance().InBattle())
        {
            ToggleMenu();   
        }

        if (inMenu && !inDialogue)
        {
            Player.GetInstance().SetMoveable(false);
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A))
            {
                if (menuCursor != 0)
                {
                    menuCursor--;
                    FindObjectOfType<AudioManager>().Play("Click");

                }                    
                UpdateMenu();

            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
            {
                if (menuCursor != menuTextBoxes.Length - 1)
                {
                    if (menuText[menuCursor + 1] != "")
                    {
                        menuCursor++;
                        FindObjectOfType<AudioManager>().Play("Click");
                    }
                }
                UpdateMenu();
            }            
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (inDialogue)
            {                
                bool endOfDialogue = DisplayNextSentence();                
                if (endOfDialogue && Player.GetInstance().HasInteractiveObject())
                {
                    if(Player.GetInstance().GetInteractiveObject().GetComponent<InteractiveObject>().IsItem() && !inMenu)
                    {
                        Destroy(Player.GetInstance().GetInteractiveObject());                        
                    }
                    inventory.CheckKeys();
                }                
            }
            else if (inMenu)
            {                
                if (menuText[menuCursor] == "Inventory")
                {
                    inventory = Player.GetInstance().GetInventory();
                    if (inventory.NumItems() > 0)
                    {
                        ClearMenuText();
                        menuText = inventory.GetPage(0, menuTextBoxes.Length);
                        UpdateMenu();
                    }
                    else
                    {
                        string[] test = new string[] { "Doug's inventory is empty..." };
                        Dialogue d = new Dialogue { sentences = test };
                        StartDialogue(d);
                    }
                }
            }            
            else if (Player.GetInstance().GetInteractiveObject() != null)
            {
                InteractiveObject interObjScript = Player.GetInstance().GetInteractiveObjectScript();
                inventory = Player.GetInstance().GetInventory();

                if (interObjScript.IsItem())
                {
                    inventory.AddItem(interObjScript.GetItem());
                }
                else if (interObjScript.NeedsItem() && inventory.HasItem(interObjScript.NeededItem()))
                {
                    interObjScript.Unlock();
                    if (interObjScript.GivesItem())
                    {
                        inventory.RemoveItem(interObjScript.NeededItem());
                        inventory.AddItem(interObjScript.GetItem());
                    }                    
                }                                                               

                interObjScript.TriggerDialogue();
            }
        }
    }    

    public void StartDialogue(Dialogue dialogue)
    {
        FindObjectOfType<AudioManager>().Play("Accept");
        Player.GetInstance().SetMoveable(false);
        sentences.Clear();
        inDialogue = true;
        foreach (string sentence in dialogue.sentences)
        {
            Debug.Log(sentence);
            sentences.Enqueue(sentence);            
        }
        message.SetActive(true);
        DisplayNextSentence();             
    }

    public bool DisplayNextSentence()
    {
        FindObjectOfType<AudioManager>().Play("Accept");
        if (typing)
        {
            StopAllCoroutines();
            messageText.text = currentSentence;
            typing = false;
            return false;
        }
        else if(sentences.Count == 0)
        {
            EndDialogue();
            return true;
        }
        else
        {
            string sentence = sentences.Dequeue();
            currentSentence = sentence;
            StopAllCoroutines();
            StartCoroutine(TypeSentence(sentence));
            return false;
        }                        
    }

    IEnumerator TypeSentence (string sentence)
    {
        messageText.text = "";
        typing = true;
        foreach(char letter in sentence.ToCharArray())
        {
            messageText.text += letter;            
            yield return new WaitForSeconds(0.025f);
        }
        typing = false;
    }

    public void AppendDialogue(Dialogue dialogue)
    {
        foreach (string sentence in dialogue.sentences)
        {            
            sentences.Enqueue(sentence);
        }
    }

    private void EndDialogue()
    {               
        inDialogue = false;
        message.SetActive(false);
        Player.GetInstance().SetMoveable(true);
    }

    public void ToggleMenu()
    {
        if (inMenu)
        {
            FindObjectOfType<AudioManager>().Play("MenuClose");
            inMenu = false;
            Player.GetInstance().SetMoveable(true);            
        }
        else
        {
            FindObjectOfType<AudioManager>().Play("MenuOpen");            
            Player.GetInstance().SetMoveable(false);
            inMenu = true;
            menuCursor = 0;
            menuText = (string[]) pauseMenuItems.Clone();
            UpdateMenu();            
        }            

        menu.SetActive(inMenu);
         
    }

    public void UpdateMenu()
    {
        for (int i = 0; i < menuTextBoxes.Length; i++)
        {            
            if (i == menuCursor)
                menuTextBoxes[i].text = "> " + menuText[i];
            else
                menuTextBoxes[i].text = "  " + menuText[i];
        }        
    }    

    private void ClearMenuText()
    {
        for(int i = 0; i < menuText.Length; i++)
        {
            menuText[i] = "";
        }
    }

    public bool InMenu()
    {
        return inMenu;
    }

    public bool InDialogue()
    {
        return inDialogue;
    }

    public bool IsBusy()
    {
        return inMenu || inDialogue;
    }

}
