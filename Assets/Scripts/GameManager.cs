using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{    
    public static GameManager instance;
    public bool inMainMenu;
    public Sprite gateOpen;
    private bool doorLocked = true;
    private bool enemyAlive = true;

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
        inMainMenu = true;
        DontDestroyOnLoad(gameObject);
        Time.timeScale = 1f;

    }    

    private void Update()
    {
        if (inMainMenu && Input.GetKeyDown(KeyCode.Return)) 
        {
            Loader.playerPos = new Vector3(0.5f, 0.5f, 0);
            Loader.Load(Loader.Scene.Overworld);
            inMainMenu = false;
        }

        if (doorLocked && SceneManager.GetActiveScene().name == "Overworld" && Player.GetInstance().HasCrown())
        {
            Debug.Log("Unlocking..");
            doorLocked = false;
            GameObject.Find("GraveyardGate").GetComponent<SpriteRenderer>().sprite = gateOpen;
            GameObject.Find("GraveyardGate").GetComponent<BoxCollider2D>().enabled = false;
        }

        if (enemyAlive && SceneManager.GetActiveScene().name == "Overworld" && Player.GetInstance().DefeatedEnemy())
        {
            Debug.Log("asdas");
            enemyAlive = false;
            Destroy(GameObject.Find("Enemy"));
        }

    }

    public static GameManager GetInstance()
    {
        return instance;
    }

    private void OnDestroy()
    {
        Debug.Log("GameManager Destroyed");
    }        
       
}