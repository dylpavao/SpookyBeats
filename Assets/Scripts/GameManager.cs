using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    public bool inBattle;

    private GameObject loadImage;
    private Text loadText;
    //public Camera mainCamera;        

    public Vector3 bottomLeft;
    public Vector3 topRight;



    //Called before start, after all objects are initialized
    void Awake()
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
        inBattle = false;

        loadImage = GameObject.Find("LoadImage");
        //loadText = GameObject.Find("LoadText").GetComponent<Text>();
        loadImage.SetActive(false);        
    }

    public static GameManager GetInstance()
    {
        return instance;
    }

    private void OnDestroy()
    {
        Debug.Log("GameManager Destroyed");
    }

    //Change to wait for player to 
    public void StartBattle()
    {
        inBattle = true;
        loadImage.SetActive(true);
        Invoke("LoadBattle", 1);
    }

    private void LoadBattle()
    {
        GameObject.Find("Player").transform.position = new Vector3((float)-1.5, (float)2.5, 0);
        //SceneLoader sceneLoader = GameObject.Find("SceneLoader").GetComponent<SceneLoader>();
        //sceneLoader.LoadScene("Battle");
    }

    public void LoadCrypt()
    {
        //Stop player movement;
        GameObject.Find("Player").GetComponent<Player>().SetMoveable(false);
        loadImage.SetActive(true);
        Invoke("RealLoadCrypt", 0.25f);
    }

    //NEED BETTER LOADING CONVENTION
    private void RealLoadCrypt()
    {
        GameObject.Find("Player").transform.position = new Vector3(0, 0, 0); //
        SceneLoader sceneLoader = GameObject.Find("SceneLoader").GetComponent<SceneLoader>();
        sceneLoader.LoadScene("Crypt");
        GameObject.Find("Player").GetComponent<Player>().SetMoveable(true);
    }


    public bool InBattle()
    {
        return inBattle;
    }
}