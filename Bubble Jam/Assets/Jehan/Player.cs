using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{

    public int maxHealth = 2;
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
        SceneManager.LoadSceneAsync(2);
    }
}
