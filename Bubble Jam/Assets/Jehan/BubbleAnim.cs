using UnityEngine;

public class AnimatorControllerDisplay : MonoBehaviour
{
    [Header("Animator Controller")]
    public RuntimeAnimatorController animatorController; // Référence au Animator Controller

    private Animator animator; // Référence à l'Animator de l'objet

    void Start()
    {
        // Récupère l'Animator attaché à l'objet
        animator = GetComponent<Animator>();

        // Si l'Animator n'est pas trouvé, affiche un message d'erreur
        if (animator == null)
        {
            Debug.LogError("Aucun Animator n'a été trouvé sur cet objet.");
            return;
        }

        // Affiche le nom du RuntimeAnimatorController dans la console
        if (animatorController != null)
        {
            Debug.Log("Animator Controller assigné : " + animatorController.name);
            // Si tu veux changer l'Animator Controller à la volée, tu peux le faire ici :
            animator.runtimeAnimatorController = animatorController;
        }
        else
        {
            Debug.LogWarning("Aucun Animator Controller n'est assigné.");
        }
    }
}
