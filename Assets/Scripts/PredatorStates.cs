using UnityEngine;

public class PredatorStates : MonoBehaviour
{
    [SerializeField] private float age;
    [SerializeField] public float hunger;
    [SerializeField] public float energy;
    [SerializeField] public bool hunting;
    
    // time
    [SerializeField] private float totalTime;
    
    [SerializeField] private float secsInYear;
    [SerializeField] private float secsDecrementHunger;
    [SerializeField] private float secsDecrementEnergy;
    [SerializeField] private float lastAge;
    [SerializeField] private float lastHunger;
    [SerializeField] private float lastEnergy;
    
    [SerializeField] private bool die;
    
    
    void Start()
    {
        age = 0;
        hunger = 20;
        energy = 40;
        hunting = false;
        
        // time
        totalTime = 0f;
        
        secsInYear = 30f;
        secsDecrementHunger = 10f;
        secsDecrementEnergy = 20f;

        lastAge = 0f;
        lastHunger = 0f;
        lastEnergy = 0f;
    }
    
    void Update()
    {
        // time
        totalTime += Time.deltaTime;

        if (totalTime - lastAge > secsInYear)
        {
            age++;
            lastAge = totalTime;
            Debug.Log("Age:" + age);
        }

        if (totalTime - lastHunger > secsDecrementHunger)
        {
            hunger--;
            lastHunger = totalTime;
            Debug.Log("Hunger:" + hunger);
        }

        if (totalTime - lastEnergy > secsDecrementEnergy)
        {
            energy--;
            lastEnergy = totalTime;
            Debug.Log("Energy:" + energy);
        }
        
        IsHungry();
        IsTired();
        CheckAge();
        SpawnBaby();
        IsHunting();
        
    }

    public bool IsHungry()
    {
        if (hunger < 0)
        {
            return true;
        }
        else if (hunger >= 0)
        {
            return false;
        }
        else if (hunger < -100)
        {
            die = true;
        }

        return false;
    }

    public bool IsTired()
    {
        if (energy < 0)
        {
            return true;
        }
        else if (energy >= 0)
        {
            return false;
        }
        else if (energy < -100)
        {
            die = true;
        }
        return false;
    }
    
    public bool CheckAge()
    {
        if (age >= 100)
        {
            int dieProb = Random.Range(0, 10);
            if (dieProb > 5)
            {
                die = true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }

        return false;
    }

    public bool IsHunting()
    {
        if (IsHungry())
        {
            return true;
        }

        return false;
    }

    public bool SpawnBaby()
    {
        if (age >= 3 && age <= 80)
        {
            float haveBaby = Random.Range(0f, 10f);
            if (haveBaby > 9.99)
            {
                return true;
            }
        }
        return false;
    }

    public bool Die()
    {
        return die;
    }
    
    // resets

    public void ResetHunger()
    {
        hunger = 20;
    }

    public void ResetEnergy()
    {
        energy = 40;
    }

    public void ResetHunting()
    {
        hunting = false;
    }
}
