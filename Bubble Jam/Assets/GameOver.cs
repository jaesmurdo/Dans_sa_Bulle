using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    // Méthode appelée pour lancer le jeu
    public void PlayGame()
    {
        // Charge la scène suivante de manière asynchrone
        SceneManager.LoadSceneAsync(1);
    }

    public void BackMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
