using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    public float maxRaycastDistance = 100f;  // Distance maximale pour le Raycast
    private GameObject selectedObject;  // L'objet actuellement sélectionné pour le drag
    private bool isDragging = false;  // Est-ce qu'un objet est en cours de drag ?

    public float smoothSpeed = 10f;  // Vitesse de déplacement de l'objet pendant le drag
    public float stopDistance = 0.1f;  // Distance à laquelle on arrête le drag
    public float sphereRadius = 0.5f;  // Rayon pour le raycast, pour la détection de l'objet sous la souris

    private Vector2 velocity = Vector2.zero;  // Vitesse utilisée pour l'inertie après le drag
    public float dragFriction = 0.98f;  // Friction pour simuler l'inertie
    public float dragSpeed = 0.5f;  // Vitesse de drag, affecte le mouvement initial

    void Update()
    {
        // Lorsque le clic droit est enfoncé, on essaie de sélectionner l'objet sous la souris
        if (Input.GetMouseButtonDown(1))  // 1 = Clic droit
        {
            DetectObjectForDrag();
        }

        // Si un objet est sélectionné et qu'on le déplace
        if (isDragging)
        {
            DragObject();

            // Relâchement du clic droit
            if (Input.GetMouseButtonUp(1))
            {
                StopDragging();
            }
        }
        else
        {
            // Applique l'inertie si le drag est terminé
            ApplyDragInertia();
        }
    }

    // Détection de l'objet sous la souris pour le commencer à draguer
    void DetectObjectForDrag()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.CircleCast(mousePosition, sphereRadius, Vector2.zero, maxRaycastDistance);

        // Si un objet avec le tag "bubble" est détecté, on le sélectionne
        if (hit.collider != null && hit.transform.CompareTag("bubble"))
        {
            selectedObject = hit.transform.gameObject;
            isDragging = true;
        }
    }

    // Déplacement de l'objet sous la souris pendant le drag
    void DragObject()
    {
        if (selectedObject != null)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Rigidbody2D rb = selectedObject.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                // Déplace l'objet avec un calcul basé sur la souris, en utilisant la physique pour une meilleure fluidité
                Vector2 direction = (mousePosition - (Vector2)selectedObject.transform.position).normalized;
                rb.linearVelocity = direction * smoothSpeed;
            }

            // La vitesse de déplacement est calculée pour appliquer de l'inertie plus tard
            velocity = (mousePosition - (Vector2)selectedObject.transform.position) * smoothSpeed;

            // Si l'objet est suffisamment proche de la souris, on arrête le drag
            if (Vector2.Distance((Vector2)selectedObject.transform.position, mousePosition) < stopDistance)
            {
                StopDragging();
            }
        }
    }

    // Appliquer l'inertie (freinage) après que l'objet ait été lâché
    void ApplyDragInertia()
    {
        if (selectedObject != null)
        {
            Rigidbody2D rb = selectedObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Applique l'inertie en ajustant la vitesse du Rigidbody
                rb.linearVelocity = velocity;

                // Applique la friction (diminuer progressivement la vitesse)
                velocity *= dragFriction;

                // Arrête le mouvement lorsque la vitesse devient suffisamment faible
                if (velocity.magnitude < 0.01f)
                {
                    velocity = Vector2.zero;
                }
            }
        }
    }

    // Arrêter le drag et conserver l'inertie
    void StopDragging()
    {
        isDragging = false;

        if (selectedObject != null)
        {
            Rigidbody2D rb = selectedObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                velocity = rb.linearVelocity;  // La vitesse finale est utilisée pour l'inertie
                rb.linearVelocity = Vector2.zero;  // Arrête immédiatement le mouvement de l'objet
            }
        }

        selectedObject = null;
    }
}
