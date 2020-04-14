using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool inMainMenu;
    public Sprite openGate;
    private Scene activeScene;
    private int mainMenuCursor;
    private GameObject videoScreen;

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
        DontDestroyOnLoad(gameObject);
        Time.timeScale = 1f;

    }

    private void Update()
    {
        if (inMainMenu)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (mainMenuCursor == 0) // start game
                {                                       
                    FindObjectOfType<AudioManager>().StopPlaying("MenuSong");
                    VideoManager videoManager = FindObjectOfType<VideoManager>();
                    //Play Cutscene
                    videoManager.PrepareVideo();                    
                }
                else // quit
                {
                    //GameObject.Find("Video Player").GetComponent<VideoManager>().PrepareVideo();
                    Application.Quit();
                }

            }
            if (Input.GetKeyDown(KeyCode.W) && mainMenuCursor == 1)
            {
                FindObjectOfType<AudioManager>().Play("Click");
                GameObject.Find("StartGame").GetComponent<Text>().text = "> Start Game";
                GameObject.Find("Quit").GetComponent<Text>().text = "  Quit";
                mainMenuCursor = 0;
            }
            if (Input.GetKeyDown(KeyCode.S) && mainMenuCursor == 0)
            {
                FindObjectOfType<AudioManager>().Play("Click");
                GameObject.Find("StartGame").GetComponent<Text>().text = "  Start Game";
                GameObject.Find("Quit").GetComponent<Text>().text = "> Quit";
                mainMenuCursor = 1;
            }
        }
        else if (SceneManager.GetActiveScene().name == "GameOver" && Input.GetKeyDown(KeyCode.Space))
        {
            Player.GetInstance().Battle(Player.GetInstance().GetCurrentEnemy());
        }

        if (activeScene != SceneManager.GetActiveScene())
        {
            activeScene = SceneManager.GetActiveScene();
            PrepareScene();
        }
    }

    public void SetVideoScreenActive(bool isActive)
    {
        videoScreen.SetActive(isActive);
    }

    //Messy way of remembering the state of the world when switching between scenes
    private void PrepareScene()
    {
        if (activeScene.name == "MainMenu")
        {
            videoScreen = GameObject.Find("VideoScreen");
            videoScreen.SetActive(false);
            FindObjectOfType<AudioManager>().Play("MenuSong");
            ResetGameState();
            inMainMenu = true;
            mainMenuCursor = 0;
        }
        else if (activeScene.name == "Overworld")
        {
            inMainMenu = false;
            FindObjectOfType<AudioManager>().Play("OverworldSong");
            if (worldState["GateOpen"])
            {
                GameObject.Find("GraveyardGate").GetComponent<SpriteRenderer>().sprite = openGate;
                GameObject.Find("GraveyardGate").GetComponent<BoxCollider2D>().enabled = false;
            }
            if (worldState["GruntDead"])
            {
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (GameObject enemy in enemies)
                {
                    if (enemy.name == "Grunt")
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
        else if (activeScene.name == "Studio")
        {
            if (worldState["GarlicObtained"])
            {
                Destroy(GameObject.Find("Garlic"));
            }
        }
        else if (activeScene.name == "Crypt")
        {
            FindObjectOfType<AudioManager>().Play("Crypt");
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
                Player.GetInstance().SetOverworldPosition(new Vector3(5.5f, 2.5f, 0));
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
        else if (activeScene.name == "Battle")
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
            else if (FindObjectOfType<Enemy>().name == "Vampire")
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
        else if (activeScene.name == "GameOver")
        {
            FindObjectOfType<AudioManager>().Play("GameOver");
        }
        else if (activeScene.name == "Credits")
        {
            videoScreen = GameObject.Find("VideoScreen");
            FindObjectOfType<VideoManager>().PrepareVideo();
        }
    }

    private void ResetGameState()
    {        
        Player.GetInstance().ClearInventory();        
        worldState["GateOpen"] = false;
        worldState["CrownObtained"] = false;
        worldState["GruntDead"] = false;
        worldState["AppleObtained"] = false;
        worldState["BlueKeyObtained"] = false;
        worldState["YellowKeyObtained"] = false;
        worldState["RedKeyObtained"] = false;
        worldState["GarlicObtained"] = false;
        worldState["VampireDead"] = false;                 
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

    public bool InMainMenu()
    {
        return inMainMenu;
    }
    public static GameManager GetInstance()
    {
        return instance;
    }    
}