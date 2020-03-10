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
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleMenu();   
        }
        if (inMenu)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A))
            {                
                if (menuCursor != 0)
                    menuCursor--;                
                    
                UpdateMenu();
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
            {                
                if(menuCursor != menuTextBoxes.Length-1)
                {
                    if(menuText[menuCursor+1] != "")
                    {
                        menuCursor++;
                    }                    
                }                    

                UpdateMenu();
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {                
                if (menuText[menuCursor] == "Inventory")
                {
                    inventory = Player.GetInstance().GetInventory();                  
                    ClearMenuText();                    
                    menuText = inventory.GetPage(0,menuTextBoxes.Length);                    
                    UpdateMenu();
                }
            }
        }        
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

    public void ToggleMenu()
    {
        if (inMenu)
        {
            inMenu = false;
            Player.GetInstance().SetMoveable(true);
        }
        else
        {
            inMenu = true;
            menuCursor = 0;
            menuText = (string[]) pauseMenuItems.Clone();
            UpdateMenu();
            Player.GetInstance().SetMoveable(false);
        }            

        menu.SetActive(inMenu);
         
    }

    public void UpdateMenu()
    {
        for (int i = 0; i < menuTextBoxes.Length; i++)
        {
            Debug.Log(menuText[i].ToString());
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

}
