using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

public class Player : MonoBehaviour
{

    public int maxHealth = 3;
    public int currentHealth;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        print(currentHealth);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            TakeDamage();
        }

        if (currentHealth <= 0)
        {
            Die();
       
        }


    }

    public void TakeDamage()
    {
        currentHealth -= 1;
        print(currentHealth);
    }

    void Die()
    {
        print("Game Over");
    }
}
