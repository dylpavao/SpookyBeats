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

        maxHealth = 10;
        currentHealth = maxHealth;
        maxMana = 10;
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
        else if (IsMoveable())
        {
            int horizontal = 0;
            int vertical = 0;
            horizontal = (int)Input.GetAxisRaw("Horizontal");
            vertical = (int)Input.GetAxisRaw("Vertical");

            if(vertical != 0)            
                horizontal = 0;            

            if (horizontal != 0 || vertical != 0)
            {
                if (horizontal > 0)
                {
                    animator.SetBool("PlayerRight", true); // abstract
                    animator.SetBool("PlayerFront", false);
                    animator.SetBool("PlayerBack", false);
                    animator.SetBool("PlayerLeft", false);
                }
                else if (horizontal < 0)
                {
                    animator.SetBool("PlayerLeft", true);
                    animator.SetBool("PlayerRight", false);
                    animator.SetBool("PlayerFront", false);
                    animator.SetBool("PlayerBack", false);
                }
                if (vertical < 0)
                {
                    animator.SetBool("PlayerFront", true);
                    animator.SetBool("PlayerRight", false);
                    animator.SetBool("PlayerBack", false);
                    animator.SetBool("PlayerLeft", false);
                }
                else if (vertical > 0)
                {
                    animator.SetBool("PlayerFront", false);
                    animator.SetBool("PlayerRight", false);
                    animator.SetBool("PlayerBack", true);
                    animator.SetBool("PlayerLeft", false);
                }
                Move(horizontal, vertical);
            }           
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
            overworldPosition = new Vector3(2.5f, -9.5f, 0);
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