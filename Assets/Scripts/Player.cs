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
    private Animator animator;
    private Vector3 overworldPosition;
    private bool firstUpdate = true;
    private bool hasCrown = false;
    private bool enemyDefeated = false;    

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
        currentMana = 1;
        state = null;        
        animator = GetComponent<Animator>(); // put in parent class?
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

            string action = null;

            if (Input.GetKeyDown(KeyCode.Q))
                action = "attack";
            else if (Input.GetKeyDown(KeyCode.W))
                action = "charge";
            else if (Input.GetKeyDown(KeyCode.E))
                action = "block";
            else if (Input.GetKeyDown(KeyCode.R))
                action = "heal";


            if (action != null && GameObject.Find("BeatKeeper").GetComponent<BeatKeeper>().HitBeat())
                state = action;

        }
        else if (IsMoveable()) // change this to set new destination if moving already
        {
            //Debug.Log(IsMoveable().ToString());            

            int horizontal = 0;
            int vertical = 0;
            horizontal = (int)Input.GetAxisRaw("Horizontal");
            vertical = (int)Input.GetAxisRaw("Vertical");

            if(vertical != 0)            
                horizontal = 0;            

            if (horizontal != 0 || vertical != 0)
            {
                GameObject.Find("UI_Assistant").GetComponent<UI_Assistant>().DeactivateMessage();

                if (IsMoving() && SameDirection(horizontal, vertical))
                {
                    UpdateDestination(horizontal, vertical);
                }
                else if (!IsMoving())
                {
                    SetDirection(horizontal, vertical);
                    Move(horizontal, vertical);
                }                
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                SearchForInteractiveObject();                
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

    //temporary
    private void SearchForInteractiveObject()
    {
        //need to make dynamic
        if(SceneManager.GetActiveScene().name == "Overworld")
        {
            if(transform.position.x == 4.5 && transform.position.y == -1.5 && dir == Direction.Right)
            {
                GameObject.Find("UI_Assistant").GetComponent<UI_Assistant>().SetMessage("Beware of the Ghost Gang!");
            }
        }
        else if(SceneManager.GetActiveScene().name == "Crypt")
        {
            //Debug.Log
            if((transform.position.x == 3.5 || transform.position.x == 4.5) && transform.position.y == 0.5 && dir == Direction.Up)
            {
                if (hasCrown)
                {
                    GameObject.Find("UI_Assistant").GetComponent<UI_Assistant>().SetMessage("Thank you Doug, now go stop the Ghost Gang!");
                }
                else
                {
                    GameObject.Find("UI_Assistant").GetComponent<UI_Assistant>().SetMessage("I seem to be missing my crown...");
                }                
            }            
        }        
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
    }

    //Call at start of battle
    private void UpdateManaBar()
    {
        GameObject.Find("PlayerManaNumbers").GetComponent<TextMeshPro>().SetText(currentMana + " / " + maxMana);
        GameObject.Find("PlayerManaBar").GetComponent<HealthBar>().SetSize(currentMana / maxMana);
    }

    //Call at start of battle
    private void UpdateHealthBar()
    {
        GameObject.Find("PlayerHealthNumbers").GetComponent<TextMeshPro>().SetText(currentHealth + " / " + maxHealth);
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
                //Loader.playerPos = new Vector3(-1.5f, -9.5f, 0);
                DisableMovement();
                Loader.Load(Loader.Scene.GameOver);
            }
        }
    }

    public void ResetState()
    {
        state = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            DisableMovement();
            SetDirection(1,0);
            overworldPosition = new Vector3(6.5f, -12.5f, 0);
            Loader.playerPos = new Vector3(-1.5f, 2.5f, 0); // combine with load?
            Loader.Load(Loader.Scene.Battle);
        }
        else if (collision.tag == "CryptDoor")
        {
            DisableMovement();
            overworldPosition = new Vector3(5.5f, 2.5f, 0);
            Loader.playerPos = new Vector3(0.5f, 0.5f, 0); // combine with load?
            Loader.Load(Loader.Scene.Crypt);                                    
        }
        else if (collision.tag == "OverworldDoor")
        {
            DisableMovement();
            Loader.playerPos = overworldPosition;
            Loader.Load(Loader.Scene.Overworld);                        
        }
        else if (collision.tag == "Item")
        {
            hasCrown = true;
            Destroy(GameObject.Find("Crown"));
            GameObject.Find("UI_Assistant").GetComponent<UI_Assistant>().SetMessage("Obtained a Crown");
            //Stop();
        }
    }    

    //temp
    public void SetEnemyDefeat(bool ed)
    {
        enemyDefeated = ed;
    }

    //temp
    public bool DefeatedEnemy()
    {
        return enemyDefeated;
    }

    public bool HasCrown()
    {
        return hasCrown;
    }

    public static Player GetInstance()
    {
        return instance;
    }

    private void OnDestroy()
    {
        Debug.Log("Player Destroyed");
    }
}