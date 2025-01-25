using UnityEngine;

public class RaycastDetections : MonoBehaviour
{
    // La distance maximale du Raycast
    public float maxRaycastDistance = 100f;

    void Update()
    {
        // Détection du clic gauche de la souris
        if (Input.GetMouseButtonDown(0))  // 0 = Clic gauche
        {
            DetectObject();
        }
    }

    // Fonction pour détecter un objet avec un Raycast
    void DetectObject()
    {
        // Crée un rayon 2D à partir de la position de la souris dans le monde
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Si le rayon touche un objet
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.down, maxRaycastDistance);

        // Si le rayon touche un objet, on entre dans cette condition
        if (hit.collider != null)
        {
            // Vérifie si l'objet a le tag "bubble"
            if (hit.transform.CompareTag("bubble"))
            {
                // Accéder au script CircleStateController attaché à l'objet détecté
                CircleStateController circleStateController = hit.transform.GetComponent<CircleStateController>();

                if (circleStateController != null)
                {
                    // Décrémenter l'état de cet objet (celui avec le tag "bubble")
                    circleStateController.DecrementState();
                }
            }
        }
    }
}
