using UnityEngine;

public class GameManager : MonoBehaviour
{    
    public static GameManager instance;
    public bool inMainMenu;


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
            Loader.playerPos = new Vector3(0.5f, 0.5f, 0);
            Loader.Load(Loader.Scene.Overworld);
            inMainMenu = false;
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