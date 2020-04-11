using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{    
    public static GameManager instance;
    public bool inMainMenu;
    public Sprite openGate;    
    private Scene activeScene;

    IDictionary<string, bool> worldState = new Dictionary<string, bool>()
    {
        {"GateOpen", false},
        {"CrownObtained", false},
        {"GruntDead", false},
        {"AppleObtained", false},
        {"BlueKeyObtained", false},
        {"YellowKeyObtained", false},
        {"RedKeyObtained", false},
        {"GarlicObtained", false},
        {"VampireDead", false}
    };

    //Called before start, after all objects are initialized
    private void Awake()
    {
        //In case 2 GameManagers exist
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }        

        instance = this;
        inMainMenu = false;
        DontDestroyOnLoad(gameObject);
        Time.timeScale = 1f;

    }    

    private void Update()
    {
        if (inMainMenu && Input.GetKeyDown(KeyCode.Return)) 
        {            
            Loader.Load(SceneName.Overworld, new Vector3(-7.5f, -2.5f, 0));
            inMainMenu = false;
        }

        if(activeScene != SceneManager.GetActiveScene())
        {
            //Debug.Log("Scene change");
            activeScene = SceneManager.GetActiveScene();
            PrepareScene();
        }                
    }

    //Messy way of remembering the state of the world when switching between scenes
    private void PrepareScene()
    {
        //Debug.Log(Player.GetInstance().LastPosition());
        if (activeScene.name == "Overworld")
        {
            if (worldState["GateOpen"])
            {
                GameObject.Find("GraveyardGate").GetComponent<SpriteRenderer>().sprite = openGate;
                GameObject.Find("GraveyardGate").GetComponent<BoxCollider2D>().enabled = false;
            }
            if (worldState["GruntDead"])
            {                
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                foreach(GameObject enemy in enemies)
                {
                    if(enemy.name == "Grunt")
                        Destroy(enemy);
                }                
            }
            if (worldState["AppleObtained"])
            {
                Destroy(GameObject.Find("Apple"));
            }
            if (worldState["BlueKeyObtained"])
            {
                GameObject.Find("Skeleton").GetComponent<InteractiveObject>().Unlock();
            }
            if (worldState["YellowKeyObtained"])
            {
                Destroy(GameObject.Find("YellowKey"));
            }           
        }
        else if(activeScene.name == "Studio")
        {
            if (worldState["GarlicObtained"])
            {
                Destroy(GameObject.Find("Garlic"));
            }
        }
        else if(activeScene.name == "Crypt")
        {            
            if (worldState["CrownObtained"])
            {                
                Destroy(GameObject.Find("Crown"));
                GameObject.Find("QueenKazoo").GetComponent<InteractiveObject>().Unlock();
            }
            if (worldState["VampireDead"])
            {
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (GameObject enemy in enemies)
                {
                    if (enemy.name == "Vampire")
                        Destroy(enemy);
                }
                Player.GetInstance().SetOverworldPosition(new Vector3(5.5f, 2.4f, 0));
                GameObject.Find("Coffin").GetComponent<InteractiveObject>().SetSpecial(true);
            }
            if (worldState["RedKeyObtained"])
            {
                Inventory inventory = Player.GetInstance().GetInventory();
                inventory.RemoveItem(ItemType.Garlic);
                inventory.AddItem(new Item { itemType = ItemType.RedKey, amount = 1, itemName = "Red Key" });
                SetWorldState("RedKeyObtained", false); // prevent getting key twice
            }
        }
        else if(activeScene.name == "Battle")
        {            
            if (FindObjectOfType<Enemy>().name == "Grunt")
            {
                Dialogue battleText = new Dialogue();
                ArrayList sent = new ArrayList
                {
                    "Before we fight I wanted to give you a few pointers for your first battle.",
                    "We will fight to a beat, you win if you can deplete my health bar. You lose if I deplete yours.",
                    "You can keep track of the beat with the sound, or the colliding bars at the bottom of the screen.",
                    "Your first step is to build up energy, which is done by pressing 'W' - in time with the beat, of course.",
                    "You gain 1 energy every time you press 'W', energy is used for attacking and healing.",
                    "To attack press 'D', this will damage the enemy if they are not blocking.",
                    "To heal press 'A', this will some health each time it is used.",
                    "Both attacking and healing cost 1 energy per use.",
                    "Lastly, you can block with 'S' which protects you from damage, you do not need energy to block.",
                    "That is it, the battle will begin when you are ready. Good luck!"
                };                
                battleText.sentences = (string[])sent.ToArray(typeof(string));
                FindObjectOfType<UI_Assistant>().StartDialogue(battleText, UI_Assistant.DialogueType.Battle);
            }
            else if(FindObjectOfType<Enemy>().name == "Vampire")
            {
                Dialogue battleText = new Dialogue();
                ArrayList sent = new ArrayList
                {
                    "You want this Red Key ehhh?",
                    "Show me what you got!"
                };                              
                battleText.sentences = (string[])sent.ToArray(typeof(string));
                FindObjectOfType<UI_Assistant>().StartDialogue(battleText, UI_Assistant.DialogueType.Battle);
            }
            else
            {
                FindObjectOfType<BeatKeeper>().SetRunning(true);
            }
        }
    }    

    public void SetWorldState(string key, bool state)
    {
        if (worldState.ContainsKey(key))
        {
            worldState[key] = state;
        }
        else
        {
            worldState.Add(key, state);
        }
    }

    public static GameManager GetInstance()
    {
        return instance;
    }    
}