using Unity.VisualScripting;
using UnityEngine;

public class Org1Behaviour : MonoBehaviour
{
    // time
    [SerializeField] private float totalTime;
    
    [SerializeField] GameManager gameManager;
    
    [SerializeField] private float wanderSpeed;
    [SerializeField] private float targetX;
    [SerializeField] private float targetY;

    [SerializeField] private bool isHungry;
    [SerializeField] private bool isTired;
    [SerializeField] private bool isPanic;
    [SerializeField] private bool spawnBaby;
    [SerializeField] Org1States states;
    
    // Panic Variables
    [SerializeField] private float panicSpeed;
    
    [SerializeField] GameObject partner;
    [SerializeField] GameObject food;
    [SerializeField] float sleepTime;
    [SerializeField] private float lastBaby;
    [SerializeField] GameObject org1Prefab;
    [SerializeField] bool spawning;
    
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] GameObject deathParticlesPrefab;
    
    void Start()
    {
        wanderSpeed = 2f;
        targetX = transform.position.x;
        targetY = transform.position.y;
        states = GetComponent<Org1States>();
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
            partner = GameObject.FindGameObjectWithTag("Org1");
        }
        
        isHungry = states.IsHungry();
        isTired = states.IsTired();
        isPanic = states.GetPanic();
        spawnBaby = states.SpawnBaby();

        if (targetX <= transform.position.x)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }

        if (isPanic)
        {
            PanicBehaviour();
        }

        if (isHungry && !isPanic)
        {
            wanderSpeed = 2f;
            if (food == null)
            {
                food = GameObject.FindGameObjectWithTag("Plant");
            }
            HungerBehaviour();
        }

        if (isTired && !isPanic && !isHungry)
        {
            TiredBehaviour();
        }

        if (spawnBaby && !isTired && !isHungry && !isPanic)
        {
            SpawnBabyBehaviour();
        }
        
        if (!isHungry && !isTired && !isPanic)
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

    void PanicBehaviour()
    {
        panicSpeed = wanderSpeed * 2;
        
        if (targetX.Equals(transform.position.x) && targetY.Equals(transform.position.y))
        {
            targetX = Random.Range(-10f, 10f);
            targetY = Random.Range(-10f, 10f);
        }

        transform.position = Vector2.MoveTowards(transform.position, new Vector2(targetX, targetY), panicSpeed * Time.deltaTime);
        
    }

    void HungerBehaviour()
    {
        targetX = food.transform.position.x;
        targetY = food.transform.position.y;
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(targetX, targetY), wanderSpeed * Time.deltaTime);
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
        if (lastBaby >= 1)
        {
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
        if (other.gameObject.CompareTag("Plant") && isHungry)
        {
            states.ResetHunger();
            gameManager.plants.Remove(other.gameObject);
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
            GameObject baby = Instantiate(org1Prefab, spawnPosition, Quaternion.identity);
        }

        if (states.Die() && other.gameObject.name == "SeaFloor")
        {
            Instantiate(deathParticlesPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    void Death()
    {
        GameObject seaFloor = GameObject.FindGameObjectWithTag("SeaFloor");
        if (this.transform.position.y >= seaFloor.transform.position.y)
        {
            spriteRenderer.flipY = true;
            rb.gravityScale = 0.5f;
        }
    }
}
