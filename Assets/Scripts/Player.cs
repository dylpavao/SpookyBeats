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

        maxHealth = 5;
        currentHealth = maxHealth;
        maxMana = 5;
        currentMana = 0;
        state = null;        
        animator = GetComponent<Animator>(); // put in parent class?
        inventory = new Inventory();
        //GameObject.Find("UI_Assistant").GetComponent<UI_Assistant>().SetInventory(inventory); //change UI_Assistant access
        base.Start();        

        DontDestroyOnLoad(gameObject);

    }

    // Update is called once per frame
    private void Update()
    {        
        if (SceneManager.GetActiveScene().name == "Battle")
        {            
            if (firstUpdate)
            {
                UpdateHealthBar();
                UpdateManaBar();
                firstUpdate = false;
            }

            if (Input.GetKeyDown(KeyCode.Space) && FindObjectOfType<UI_Assistant>().InDialogue())
            {
                bool endOfDialogue = GameObject.Find("UI_Assistant").GetComponent<UI_Assistant>().DisplayNextSentence();

                if (endOfDialogue)
                {
                    FindObjectOfType<BeatKeeper>().SetRunning(true);
                }
            }
            else
            {
                string action = null;

                if (Input.GetKeyDown(KeyCode.D))
                {
                    action = "attack";
                    animator.SetBool("Attacking", true);
                }
                else if (Input.GetKeyDown(KeyCode.W))
                {
                    action = "charge";
                    animator.SetBool("Charging", true);
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    action = "block";
                    animator.SetBool("Blocking", true);
                }
                else if (Input.GetKeyDown(KeyCode.A))
                {
                    action = "heal";
                    animator.SetBool("Healing", true);
                }


                if (action != null && FindObjectOfType<BeatKeeper>().HitBeat())
                    state = action;
            }           

        }
        else // in overworld
        {                     

            // Get directional input
            int horizontal = 0;
            int vertical = 0;
            horizontal = (int)Input.GetAxisRaw("Horizontal");
            vertical = (int)Input.GetAxisRaw("Vertical");

            if(vertical != 0)            
                horizontal = 0;

            //interact with object in game
            if (Input.GetKeyDown(KeyCode.Space) && currentInterObj != null)
            {
                Debug.Log(currentInterObj.name);
                if (FindObjectOfType<UI_Assistant>().InDialogue())
                {
                    bool endOfDialogue = GameObject.Find("UI_Assistant").GetComponent<UI_Assistant>().DisplayNextSentence();

                    if (endOfDialogue && currentInterObjScript.IsItem())
                    {
                        inventory.AddItem(currentInterObjScript.GetItem());
                        Destroy(currentInterObj);
                    }
                }
                else
                {
                    if (currentInterObjScript.NeedsItem() && inventory.HasItem(currentInterObjScript.NeededItem()))
                    {
                        currentInterObjScript.Unlock();
                    }
                    currentInterObjScript.TriggerDialogue();
                }
            }
            else if ((horizontal != 0 || vertical != 0) && IsMoveable()) //character control
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

    private void SetDirection(int horizontal, int vertical)
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
        if (state == "attack" && currentMana > 0)
        {
            currentMana--;
            UpdateManaBar();
            GameObject.Find("Enemy").GetComponent<Enemy>().TakeDamage(1);
        }
        else if (state == "charge" && currentMana < maxMana)
        {
            currentMana++;
            UpdateManaBar();
        }
        else if (state == "heal" && currentMana > 0 && currentHealth < maxHealth)
        {
            currentMana--;
            currentHealth++;
            UpdateManaBar();
            UpdateHealthBar();
        }


        animator.SetBool("Charging",false);
        animator.SetBool("Healing", false);
        animator.SetBool("Attacking", false);
        animator.SetBool("Blocking", false);
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
        if (state != "block")
        {            
            currentHealth -= dmg; // prevent negative health            
            UpdateHealthBar();

            if (currentHealth == 0)
            {
                //game over                
                DisableMovement(true);
                Loader.Load(SceneName.GameOver, new Vector3(100,100,-100));
            }
        }
    }    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            enemy.PrepareForBattle();
            DisableMovement(true);            
            SetDirection(1,0);
            overworldPosition = LastPosition();
            Loader.Load(SceneName.Battle, new Vector3(-1.5f, 4.5f, 0));            
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

    public Vector3 OverworldPosition()
    {
        return overworldPosition;
    }

    public void ResetState()
    {
        state = null;
    }

    public static Player GetInstance()
    {
        return instance;
    }

    public Inventory GetInventory()
    {
        return inventory;
    }

    private void OnDestroy()
    {
        //Debug.Log("Player Destroyed");
    }
}