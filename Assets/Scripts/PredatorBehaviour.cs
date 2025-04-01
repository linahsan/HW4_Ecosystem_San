using Unity.VisualScripting;
using UnityEngine;

public class PredatorBehaviour : MonoBehaviour
{
    // time
    [SerializeField] private float totalTime;
    
    [SerializeField] GameManager gameManager;
    
    [SerializeField] private float wanderSpeed;
    [SerializeField] private float targetX;
    [SerializeField] private float targetY;

    [SerializeField] private bool isHungry;
    [SerializeField] private bool isTired;
    [SerializeField] private bool isHunting;
    [SerializeField] private bool spawnBaby;
    [SerializeField] PredatorStates states;
    
    // Panic Variables
    [SerializeField] private float huntingSpeed;
    
    [SerializeField] GameObject partner;
    [SerializeField] GameObject food;
    [SerializeField] float sleepTime;
    [SerializeField] private float lastBaby;
    [SerializeField] private int babyNumber;
    [SerializeField] GameObject predatorPrefab;
    [SerializeField] bool spawning;
    
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] GameObject deathParticlesPrefab;
    
    void Start()
    {
        wanderSpeed = 2f;
        targetX = transform.position.x;
        targetY = transform.position.y;
        states = GetComponent<PredatorStates>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spawning = false;
        rb = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        totalTime += Time.deltaTime;
        
        if (partner == null)
        {
            partner = GameObject.FindGameObjectWithTag("Predator");
        }
        
        isHungry = states.IsHungry();
        isTired = states.IsTired();
        isHunting = states.IsHunting();
        spawnBaby = states.SpawnBaby();

        if (targetX <= transform.position.x)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }
        
        if (isHunting)
        {
            wanderSpeed = 2f;

            if (food == null)
            {
                food = GameObject.FindGameObjectWithTag("Org1");
            }
            HuntingBehaviour();
        }

        if (isTired && !isHunting && !isHungry)
        {
            TiredBehaviour();
        }

        if (spawnBaby && !isTired && !isHungry && !isHunting)
        {
            SpawnBabyBehaviour();
        }
        
        if (!isHungry && !isTired && !isHunting)
        {
            Wander();
        }

        if (states.Die())
        {
            Death();
        }
    }

    void Wander()
    {
        if (targetX.Equals(transform.position.x) && targetY.Equals(transform.position.y))
        {
            targetX = Random.Range(-10f, 10f);
            targetY = Random.Range(-10f, 10f);
        }

        transform.position = Vector2.MoveTowards(transform.position, new Vector2(targetX, targetY), wanderSpeed * Time.deltaTime);
        
    }

    void HuntingBehaviour()
    {
        food.gameObject.GetComponent<Org1States>().IsPanic();
        huntingSpeed = wanderSpeed * 2;
        
        targetX = food.transform.position.x;
        targetY = food.transform.position.y;
        
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(targetX, targetY), huntingSpeed * Time.deltaTime);
        
    }

    void TiredBehaviour()
    {
        wanderSpeed = 0f;
        sleepTime += Time.deltaTime;

        if (sleepTime >= 5f)
        {
            states.ResetEnergy();
            wanderSpeed = 2f;
            sleepTime = 0f;
        }
    }

    void SpawnBabyBehaviour()
    {
        lastBaby += Time.deltaTime;
        if (lastBaby >= 30 && babyNumber < 1)
        {
            babyNumber++;
            spawning = true;
            targetX = partner.transform.position.x;
            targetY = partner.transform.position.y;
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(targetX, targetY), wanderSpeed * Time.deltaTime);
        }
        else
        {
            spawning = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Org1") && isHunting)
        {
            states.ResetHunger();
            states.ResetHunting();
            food = null;
            Destroy(other.gameObject);
        }
        if (other.gameObject == partner && spawning)
        {
            Debug.Log(this.gameObject.name);
            lastBaby = 0;
            float offsetX = Random.Range(-2f, 2f);
            float offsetY = Random.Range(-2f, 2f);
            Vector2 spawnPosition = new Vector2(this.transform.position.x + offsetX, this.transform.position.y + offsetY);
            GameObject baby = Instantiate(predatorPrefab, spawnPosition, Quaternion.identity);
        }
        
        if (states.Die() && other.gameObject.name == "SeaFloor")
        {
            Instantiate(deathParticlesPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
    
    void Death()
    {
        spriteRenderer.flipY = true;
        rb.gravityScale = 0.5f;
    }
}
