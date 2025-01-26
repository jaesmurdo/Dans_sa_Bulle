using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // N'oublie pas d'ajouter l'espace de nom de TextMeshPro

public class Player : MonoBehaviour
{
    public int maxHealth = 2;
    public int currentHealth;
    
    [Header("UI Configuration")]
    public TextMeshProUGUI healthText; // Référence au TextMeshProUGUI pour afficher la santé

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthDisplay(); // Met à jour l'affichage au début
    }

    void Update()
    {
        // Vérifier si la santé est inférieure ou égale à 0
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void TakeDamage()
    {
        currentHealth -= 1;
        UpdateHealthDisplay(); // Met à jour l'affichage après avoir pris des dégâts
    }

    void Die()
    {
        SceneManager.LoadSceneAsync(2);
    }

    // Méthode pour mettre à jour l'affichage de la santé
    void UpdateHealthDisplay()
    {
        if (healthText != null)
        {
            healthText.text = "" + currentHealth; // Met à jour le texte avec la santé actuelle
        }
    }
}
