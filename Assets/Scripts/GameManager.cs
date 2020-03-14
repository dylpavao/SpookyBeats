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
        {"GarlicObtained", false}
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
            }
        }
        else if(activeScene.name == "Battle")
        {
            Dialogue battleText = new Dialogue();
            string[] sentences = new string[9];
            sentences[0] = "Before we fight I wanted to give you a few pointers for your first battle.";
            sentences[1] = "We will fight to a beat, you win if you can deplete my health bar. You lose if I deplete yours.";
            sentences[2] = "You can keep track of the beat with the sound, or the colliding bars at the bottom of the screen.";
            sentences[3] = "Your first step is to build up energy, which is done by pressing 'W' - in time with the beat, of course.";
            sentences[4] = "You gain 1 energy every time you press 'W', energy is used for attacking and healing.";
            sentences[5] = "To attack press 'D', this will damage the enemy by 1 point if they are not blocking.";
            sentences[6] = "To heal press 'A', this will recover 1 health each time it is used.";
            sentences[7] = "Both attacking and healing cost 1 energy per use.";
            sentences[8] = "Lastly, you can block with 'S' which protects you from damage, you do not need energy to block.";
            sentences[8] = "That is it, the battle will begin when you are ready. Good luck!";
            battleText.sentences = sentences;
            FindObjectOfType<UI_Assistant>().StartDialogue(battleText);
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

    private void OnDestroy()
    {
        //Debug.Log("GameManager Destroyed");
    }        
       
}