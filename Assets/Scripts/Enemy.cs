using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Enemy : MovingObject
{    
    private float maxHealth;
    private float currentHealth;
    private float maxMana;
    private float currentMana;
    private int range;
    private string state;
    private Animator animator;
    private readonly string[] states = new string[] { "Attacking", "Charging", "Blocking", "Healing" };
    private bool firstUpdate = true; 

    // Start is called before the first frame update
    protected override void Start()
    {        
        maxHealth = 10;
        currentHealth = maxHealth;
        maxMana = 5;
        currentMana = 0;
        state = null;

        animator = GameObject.Find("EnemyGraphics").GetComponent<Animator>();

        //switch (gameObject.name)
        //{
        //    case "Grunt":
        //        animator = GameObject.Find("EnemyGraphics").GetComponent<Animator>();
        //        break;
        //    case "Vampire":
        //        animator = gameObject.GetComponent<Animator>();
        //        break;
        //}

        range = 3;        
        base.Start();
               
    }

    private void Update()
    {        
        if (SceneManager.GetActiveScene().name == "Overworld")
        {
            Vector3 playerPos = Player.GetInstance().transform.position;
            float x1 = transform.position.x - range;
            float x2 = transform.position.x + range;
            float y1 = transform.position.y - range;
            float y2 = transform.position.y + range;

            //Checks if player is in range, moves towards player if so
            if (playerPos.x >= x1 && playerPos.x <= x2 && playerPos.y >= y1 && playerPos.y <= y2)
            {                
                int yDir = 0;
                int xDir = 0;
                if (playerPos.x > transform.position.x)
                {
                    xDir = 1;
                }
                else if (playerPos.x < transform.position.x)
                {
                    xDir = -1;
                }

                if (playerPos.y > transform.position.y)
                {
                    yDir = 1;
                }
                else if (playerPos.y < transform.position.y)
                {
                    yDir = -1;
                }

                if (IsMoveable())
                {                    
                    Move(xDir, yDir);
                }
            }
        }
        else if(SceneManager.GetActiveScene().name == "Battle")
        {
            if (firstUpdate)           
            {                
                transform.position = new Vector3(1.5f, 4.5f, 0);
                currentHealth = maxHealth;
                currentMana = 0;
                UpdateHealthBar();
                UpdateManaBar();
                firstUpdate = false;
            }
        }
    }

    public void ChooseMove()
    {
        Debug.Log(currentMana);
        if (currentMana == 0)
        {
            state = "Charging";
        }
        else if(currentHealth <= 5 && currentMana >= 1)
        {
            int a = Random.Range(0, 2);
            if (a == 0)
                state = "Healing";
            else
                state = "Attacking";
        }        
        else if(currentHealth == maxHealth)
        {
            int a = Random.Range(0, 3);
            if (a == 0)
                state = "Attacking";
            else if (a == 1)
                state = "Blocking";
            else
                state = "Charging";
        }
        else
        {
            state = states[Random.Range(0, 4)];
        }
    }

    public void EnactMove()
    {        
        //create enums for states
        if (state == "Attacking" && currentMana > 0)
        {
            animator.SetBool(state, true);
            currentMana--;
            UpdateManaBar();
            Player.GetInstance().TakeDamage(Random.Range(0, 2)+1);
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
        else if (state == "Blocking")
        {
            animator.SetBool(state, true);
        }        
    }

    private void UpdateManaBar()
    {
        GameObject.Find("EnemyManaNumbers").GetComponent<TextMeshPro>().SetText(currentMana.ToString());
        GameObject.Find("EnemyManaBar").GetComponent<HealthBar>().SetSize(currentMana / maxMana);
    }

    private void UpdateHealthBar()
    {
        GameObject.Find("EnemyHealthNumbers").GetComponent<TextMeshPro>().SetText(currentHealth.ToString());
        GameObject.Find("EnemyHealthBar").GetComponent<HealthBar>().SetSize(currentHealth / maxHealth);
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

    public void ResetFirstUpdate()
    {
        firstUpdate = true;
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
                //player wins
                FindObjectOfType<Player>().ResetState();

                switch (gameObject.name)
                {
                    case "Grunt": //rename to grunt
                        GameManager.GetInstance().SetWorldState("GruntDead", true);
                        Loader.Load(SceneName.Overworld, Player.GetInstance().OverworldPosition());
                        break;
                    case "Vampire":
                        GameManager.GetInstance().SetWorldState("VampireDead", true);
                        GameManager.GetInstance().SetWorldState("RedKeyObtained", true);
                        Loader.Load(SceneName.Crypt, Player.GetInstance().OverworldPosition());
                        break;
                }                                                        
            }
        }
    }    

    public void PrepareForBattle()
    {
        DisableMovement(true);
        SetDestination(new Vector3(1.5f, 4.5f, 0));
        DontDestroyOnLoad(gameObject);
    }        
}