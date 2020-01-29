using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class Player : MovingObject
{
    private float currentHealth;
    private float maxHealth;
    private float maxMana;
    private float currentMana;
    private string state;

    private Animator animator;

    // Start is called before the first frame update
    protected override void Start()
    {
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
        if (GameManager.GetInstance().InBattle())
        {
            string action = null;

            if (Input.GetKeyDown(KeyCode.A))
                action = "attack";
            else if (Input.GetKeyDown(KeyCode.L))
                action = "charge";
            else if (Input.GetKeyDown(KeyCode.S))
                action = "block";
            else if (Input.GetKeyDown(KeyCode.D))
                action = "heal";


            //if (action != null && GameObject.Find("BeatKeeper").GetComponent<BeatKeeper>().HitBeat())
            //    state = action;

        }
        else if (IsMoveable())
        {
            int horizontal = 0;
            int vertical = 0;
            horizontal = (int)Input.GetAxisRaw("Horizontal");
            vertical = (int)Input.GetAxisRaw("Vertical");

            if (horizontal != 0 || vertical != 0)
            {
                if (horizontal > 0)
                {
                    animator.SetBool("PlayerRight", true);
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
            //GameObject.Find("Enemy").GetComponent<Enemy>().TakeDamage(1);
        }
        else if (state == "charge" && currentMana < maxMana)
        {
            currentMana++;
            UpdateManaBar();
        }
        else if (state == "heal" && currentMana > 0)
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
        //GameObject.Find("PlayerManaBar").GetComponent<HealthBar>().SetSize(currentMana / maxMana);
    }

    //Call at start of battle
    private void UpdateHealthBar()
    {
        //GameObject.Find("PlayerHealthBar").GetComponent<HealthBar>().SetSize(currentHealth / maxHealth);
    }

    public void TakeDamage(int dmg)
    {
        if (state != "block")
        {
            currentHealth -= dmg; // prevent negative health            
            UpdateHealthBar();
        }
    }

    public void Reset()
    {
        state = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        Debug.Log("Collisipon");
        if (collision.tag == "Enemy")
        {
            GameManager.instance.StartBattle();
        }
        else if (collision.tag == "CryptDoor")
        {
            Debug.Log("Loading Crypt...");
            GameManager.instance.LoadCrypt();
            //transform.position = new Vector3(0, 0, 0);
            //GameObject.Find("SceneLoader").GetComponent<SceneLoader>().LoadScene("Crypt");            
        }        
    }

    private void OnDestroy()
    {
        Debug.Log("Player Destroyed");
    }
}