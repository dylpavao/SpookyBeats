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
    private readonly string[] states = new string[] { "attack", "charge", "block", "heal" };

    private bool firstUpdate = true; 

    // Start is called before the first frame update
    protected override void Start()
    {
        maxHealth = 5;
        currentHealth = maxHealth;
        maxMana = 5;
        currentMana = 0;
        state = null;
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

            if (playerPos.x >= x1 && playerPos.x <= x2 && playerPos.y >= y1 && playerPos.y <= y2)
            {
                //Debug.Log("Player in range");

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
        else
        {
            if (firstUpdate)            //bandaid solution??
            {
                UpdateHealthBar();
                UpdateManaBar();
                firstUpdate = false;
            }
        }
    }

    public void ChooseMove()
    {
        int move = Random.Range(0, states.Length);              
        state = states[move];
    }

    public void EnactMove()
    {
        //create enums for states

        if (state == "attack" && currentMana > 0)
        {
            currentMana--;
            UpdateManaBar();
            Player.GetInstance().TakeDamage(1);
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
    }

    public void TakeDamage(int dmg)
    {
        if (state != "block")
        {                 
            currentHealth -= dmg;
            UpdateHealthBar();

            if (currentHealth == 0)
            {
                //player wins
                GameManager.GetInstance().SetWorldState("GruntDead", true);                
                Loader.Load(SceneName.Overworld, Player.GetInstance().OverworldPosition());
            }
        }
    }
}