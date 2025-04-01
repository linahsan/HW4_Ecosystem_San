using UnityEngine;

public class PlantState : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float totalTime;
    [SerializeField] GameManager gameManager;
    [SerializeField] float driftSpeed = 0.2f;
    [SerializeField] float floatTime = 3f; 
    [SerializeField] Vector2 direction;
    [SerializeField] float timer;
    [SerializeField] GameObject seaFloor;
    
    // states
    [SerializeField] bool floating;
    [SerializeField] private bool old;
    [SerializeField] private bool dying;
    
    [SerializeField] float shrinkSpeed = 0.5f;
    
    void Start()
    {
        totalTime = 0f;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        
        direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-0.5f, 0.5f)).normalized;
        rb = GetComponent<Rigidbody2D>();
        seaFloor = GameObject.Find("SeaFloor");
    }
    
    void Update()
    {
        totalTime += Time.deltaTime;

        if (totalTime >= 50)
        {
            Decomposing();
            
        }

        if (totalTime <= 30)
        {
            Float();
        }

        if (totalTime <= 50 && totalTime >= 30 && transform.position.y >= seaFloor.transform.position.y)
        {
            Old();
        }
        
    }
    
    void Float()
    {
        transform.Translate(direction * driftSpeed * Time.deltaTime);
        timer += Time.deltaTime;
        
        if (timer >= floatTime)
        {
            direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-0.5f, 0.5f)).normalized;
            timer = 0f;
        }
    }

    void Old()
    {
        GameObject seaFloor = GameObject.FindGameObjectWithTag("SeaFloor");
        if (this.transform.position.y >= seaFloor.transform.position.y)
        {
            rb.gravityScale = 0.5f;
        }
    }

    void Decomposing()
    {
        transform.localScale -= new Vector3(shrinkSpeed, shrinkSpeed, 0) * Time.deltaTime;
        
        if (transform.localScale.x <= 0.05f)
        {
            gameManager.plants.Remove(this.gameObject);
            Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == seaFloor)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }
}
