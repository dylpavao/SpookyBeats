using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Player : MovingObject
{
    private float currentHealth;
    private float maxHealth;
    private float maxMana;
    private float currentMana;
    private string state;
    private bool firstUpdate = true; //temp?    
    private Animator animator;   
    private Inventory inventory;
    private GameObject currentInterObj;
    private InteractiveObject currentInterObjScript;
    private Vector3 overworldPosition;
    private static Player instance;
    private Enemy currentEnemy;

    // Start is called before the first frame update
    protected override void Start()
    {
        //In case 2 Player exist
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        maxHealth = 10;
        currentHealth = maxHealth;
        maxMana = 5;
        currentMana = 0;
        state = null;
        animator = GameObject.Find("PlayerGraphics").GetComponent<Animator>(); // put in parent class?                                                                           
        inventory = new Inventory();        
        base.Start();        

        DontDestroyOnLoad(gameObject);

    }

    // Update is called once per frame
    private void Update()
    {        
        if (SceneManager.GetActiveScene().name == "Battle") // Battle Controls
        {            
            if (firstUpdate)
            {                
                currentHealth = 10;
                currentMana = 0;
                UpdateHealthBar();
                UpdateManaBar();
                firstUpdate = false;
            }        
            else
            {
                string action = null;

                if (Input.GetKeyDown(KeyCode.D))
                {
                    action = "Attacking";                    
                }
                else if (Input.GetKeyDown(KeyCode.W))
                {
                    action = "Charging";                    
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    action = "Blocking";                    
                }
                else if (Input.GetKeyDown(KeyCode.A))
                {
                    action = "Healing";                    
                }

                if (action != null && FindObjectOfType<BeatKeeper>().HitBeat())
                {                    
                    state = action;
                    EnactMove();
                }                    
            }           
        }
        else // in non battle state
        {
            // Get directional input
            firstUpdate = true;
            int horizontal = 0;
            int vertical = 0;
            horizontal = (int)Input.GetAxisRaw("Horizontal");
            vertical = (int)Input.GetAxisRaw("Vertical");

            if(vertical != 0)            
                horizontal = 0;

            if ((horizontal != 0 || vertical != 0) && IsMoveable()) //character control [W,A,S,D]
            {                               
                if (IsMoving() && SameDirection(horizontal, vertical))
                {
                    UpdateDestination(horizontal, vertical);
                }
                else if (!IsMoving())
                {
                    SetDirection(horizontal, vertical);
                    Move(horizontal, vertical);
                }
                currentInterObj = null;
                currentInterObjScript = null;                                           
            }            
        }
    }    

    private bool SameDirection(int horizontal, int vertical)
    {
        if (dir == Direction.Right && horizontal == 1)
        {
            return true; 
        }
        else if (dir == Direction.Left && horizontal == -1)
        {
            return true;
        }
        else if (dir == Direction.Up && vertical == 1)
        {
            return true;
        }
        else if (dir == Direction.Down && vertical == -1)
        {
            return true;
        }

        return false;
    }

    public void SetDirection(int horizontal, int vertical)
    {        
        if (horizontal == 1)
        {
            dir = Direction.Right;
            animator.SetBool("PlayerRight", true);
            animator.SetBool("PlayerFront", false);
            animator.SetBool("PlayerBack", false);
            animator.SetBool("PlayerLeft", false);
        }
        else if (horizontal == -1)
        {
            dir = Direction.Left;
            animator.SetBool("PlayerLeft", true);
            animator.SetBool("PlayerRight", false);
            animator.SetBool("PlayerFront", false);
            animator.SetBool("PlayerBack", false);
        }
        else if(vertical == -1)
        {
            dir = Direction.Down;
            animator.SetBool("PlayerFront", true); // change front to down & back to up
            animator.SetBool("PlayerRight", false);
            animator.SetBool("PlayerBack", false);
            animator.SetBool("PlayerLeft", false);
        }
        else if(vertical == 1) //change to up
        {
            dir = Direction.Up;
            animator.SetBool("PlayerFront", false);
            animator.SetBool("PlayerRight", false);
            animator.SetBool("PlayerBack", true);
            animator.SetBool("PlayerLeft", false);
        }
    }

    public void EnactMove()
    {            
        if (state == "Attacking" && currentMana > 0)
        {
            animator.SetBool(state, true);
            currentMana--;
            UpdateManaBar();            
            FindObjectOfType<Enemy>().TakeDamage(Random.Range(0,2)+1);
        }
        else if (state == "Charging" && currentMana < maxMana)
        {
            animator.SetBool(state, true);
            currentMana++;
            UpdateManaBar();
        }
        else if (state == "Healing" && currentMana > 0 && currentHealth < maxHealth)
        {            
            animator.SetBool(state, true);
            currentMana--;
            currentHealth += Random.Range(0, 2) + 1;
            if (currentHealth > maxHealth)
                currentHealth = maxHealth;
            UpdateManaBar();
            UpdateHealthBar();
        }
        else if(state == "Blocking")
        {
            animator.SetBool(state, true);
        }        
    }
    
    private void UpdateManaBar()
    {
        GameObject.Find("PlayerManaNumbers").GetComponent<TextMeshPro>().SetText(currentMana.ToString());
        GameObject.Find("PlayerManaBar").GetComponent<HealthBar>().SetSize(currentMana / maxMana);
    }

    private void UpdateHealthBar()
    {
        GameObject.Find("PlayerHealthNumbers").GetComponent<TextMeshPro>().SetText(currentHealth.ToString());
        GameObject.Find("PlayerHealthBar").GetComponent<HealthBar>().SetSize(currentHealth / maxHealth);
    }

    public void TakeDamage(int dmg)
    {
        if (state != "Blocking")
        {
            animator.SetBool("Damaged", true);
            currentHealth -= dmg;      
            if (currentHealth < 0)
                currentHealth = 0;
            UpdateHealthBar();            
            if (currentHealth == 0)
            {
                //game over                
                DisableMovement(true);
                currentEnemy.ResetFirstUpdate();
                Loader.Load(SceneName.GameOver, new Vector3(100,100,-100));
            }
        }
    }    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            Battle(collision.gameObject.GetComponent<Enemy>());                  
        }
        else if (collision.tag == "Door")
        {
            DisableMovement(true);
            Door door = collision.gameObject.GetComponent<Door>();

            if (door.GetSceneName() == SceneName.Overworld)
                door.SetPlayerStartingPos(overworldPosition);
            else
                overworldPosition = LastPosition();

            Loader.Load(door.GetSceneName(), door.GetPlayerStartingPos());                                           
        }          
        else if (collision.tag == "Interactable")
        {
            //Debug.Log(collision.name);
            currentInterObj = collision.gameObject;
            currentInterObjScript = currentInterObj.GetComponent<InteractiveObject>();
        }
    }    

    public void Battle(Enemy enemy)
    {        
        currentEnemy = enemy;
        enemy.PrepareForBattle();
        DisableMovement(true);
        ResetState();
        currentEnemy.ResetState();
        SetDirection(1, 0);
        overworldPosition = LastPosition();
        Loader.Load(SceneName.Battle, new Vector3(-1.5f, 4.5f, 0));
    }

    public void SetMoveable(bool moveable)
    {
        if (moveable)
        {
            EnableMovement();
        }
        else
        {
            DisableMovement(false);
        }
    }

    public void SetOverworldPosition(Vector3 pos)
    {
        overworldPosition = pos;
    }

    public Vector3 OverworldPosition()
    {
        return overworldPosition;
    }

    public void ResetState()
    {
        state = null;
        animator.SetBool("Charging", false);
        animator.SetBool("Healing", false);
        animator.SetBool("Attacking", false);
        animator.SetBool("Blocking", false);
        //animator.SetBool("Damaged", false);
    }

    public Enemy GetCurrentEnemy()
    {
        return currentEnemy;
    }

    public static Player GetInstance()
    {
        return instance;
    }

    public void ClearInventory()
    {
        inventory = new Inventory();
    }

    public Inventory GetInventory()
    {
        return inventory;
    }

    public void ClearInteractiveObject()
    {
        currentInterObj = null;
    }

    public bool HasInteractiveObject()
    {
        return currentInterObj != null;
    }

    public GameObject GetInteractiveObject()
    {
        return currentInterObj;
    }

    public InteractiveObject GetInteractiveObjectScript()
    {
        return currentInterObjScript;
    }

    public bool InBattle()
    {
        return SceneManager.GetActiveScene().name == "Battle";
    }    
}