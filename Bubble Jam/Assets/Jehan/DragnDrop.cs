using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    // La distance maximale du Raycast pour détecter les objets
    public float maxRaycastDistance = 100f;

    // Référence à l'objet actuellement sélectionné pour le déplacement
    private GameObject selectedObject;

    // Flag pour savoir si un objet est en train d'être déplacé
    private bool isDragging = false;

    // Variables pour appliquer un mouvement fluide
    public float smoothSpeed = 10f; // La vitesse à laquelle l'objet se déplace vers la position de la souris
    public float stopDistance = 0.1f; // La distance seuil pour considérer que l'objet a atteint la souris
    public float sphereRadius = 0.5f; // Le rayon du cercle pour le Raycast sphérique

    // Variables pour le glissement
    private Vector2 velocity = Vector2.zero; // Vitesse actuelle de l'objet après le drag
    public float dragFriction = 0.98f; // Friction pour ralentir progressivement l'objet (glissement)
    public float dragSpeed = 0.5f; // Vitesse du glissement

    void Update()
    {
        // Détection du clic droit (1) pour commencer le drag
        if (Input.GetMouseButtonDown(1))  // 1 = Clic droit
        {
            DetectObjectForDrag();
        }

        // Si un objet est en cours de déplacement, le déplacer
        if (isDragging)
        {
            DragObject();

            // Si le clic droit est relâché, arrêter le drag mais continuer le glissement
            if (Input.GetMouseButtonUp(1))
            {
                StopDragging();
            }
        }
        else
        {
            // Si l'objet n'est pas en train d'être déplacé, appliquer l'inertie
            ApplyDragInertia();
        }
    }

    // Fonction pour détecter l'objet avec le raycast sphérique et commencer à le déplacer
    void DetectObjectForDrag()
    {
        // Récupérer la position de la souris dans le monde
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Raycast sphérique pour détecter les objets sous la souris
        RaycastHit2D hit = Physics2D.CircleCast(mousePosition, sphereRadius, Vector2.zero, maxRaycastDistance);

        // Si un objet a été touché par le rayon
        if (hit.collider != null)
        {
            // Vérifier si l'objet a le tag "bubble"
            if (hit.transform.CompareTag("bubble"))
            {
                selectedObject = hit.transform.gameObject;

                Debug.Log("Objet détecté pour drag : " + selectedObject.name);

                // Commencer à déplacer l'objet
                isDragging = true;
            }
            else
            {
                Debug.Log("L'objet détecté n'a pas le tag 'bubble'.");
            }
        }
        else
        {
            Debug.Log("Aucun objet détecté par le Raycast sphérique.");
        }
    }

    // Fonction pour déplacer l'objet en suivant la souris avec un mouvement fluide
    void DragObject()
    {
        if (selectedObject != null)
        {
            // Récupérer la position de la souris dans le monde
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Déplacer l'objet avec un décalage doux (latence fluide)
            Vector2 targetPosition = Vector2.Lerp((Vector2)selectedObject.transform.position, mousePosition, smoothSpeed * Time.deltaTime);
            selectedObject.transform.position = targetPosition;

            // Calculer la vitesse de déplacement en fonction de la position actuelle et de la position de la souris
            velocity = (mousePosition - (Vector2)selectedObject.transform.position) * smoothSpeed;

            // Vérifie si l'objet est suffisamment proche de la souris pour arrêter le mouvement
            if (Vector2.Distance((Vector2)selectedObject.transform.position, mousePosition) < stopDistance)
            {
                // Si l'objet est suffisamment proche de la position de la souris, on arrête le mouvement
                StopDragging();
            }
        }
    }

    // Fonction pour appliquer le glissement après avoir lâché l'objet
    void ApplyDragInertia()
    {
        if (selectedObject != null)
        {
            // Applique la vitesse actuelle (inertie)
            selectedObject.transform.position += (Vector3)velocity * Time.deltaTime;

            // Réduire progressivement la vitesse pour simuler le glissement (friction)
            velocity *= dragFriction;

            // Si la vitesse devient assez petite, on arrête le glissement
            if (velocity.magnitude < 0.01f)
            {
                velocity = Vector2.zero;
            }
        }
    }

    // Fonction pour arrêter le déplacement de l'objet
    void StopDragging()
    {
        isDragging = false;

        // Commence l'inertie avec une vitesse initiale en fonction du dernier mouvement
        if (selectedObject != null)
        {
            velocity = (Vector2)selectedObject.transform.position - (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        selectedObject = null;
        Debug.Log("Déplacement terminé.");
    }
}
