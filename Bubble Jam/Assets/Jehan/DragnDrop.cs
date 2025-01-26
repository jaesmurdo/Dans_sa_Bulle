using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    public float maxRaycastDistance = 100f;
    private GameObject selectedObject;
    private bool isDragging = false;

    public float smoothSpeed = 10f;
    public float stopDistance = 0.1f;
    public float sphereRadius = 0.5f;

    private Vector2 velocity = Vector2.zero;
    public float dragFriction = 0.98f;
    public float dragSpeed = 0.5f;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))  // 1 = Clic droit
        {
            DetectObjectForDrag();
        }

        if (isDragging)
        {
            DragObject();

            if (Input.GetMouseButtonUp(1))
            {
                StopDragging();
            }
        }
        else
        {
            ApplyDragInertia();
        }
    }

    void DetectObjectForDrag()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.CircleCast(mousePosition, sphereRadius, Vector2.zero, maxRaycastDistance);

        if (hit.collider != null && hit.transform.CompareTag("bubble"))
        {
            selectedObject = hit.transform.gameObject;
            isDragging = true;
        }
    }

    void DragObject()
    {
        if (selectedObject != null)
        {
            // Utiliser le Rigidbody2D pour déplacer l'objet
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Rigidbody2D rb = selectedObject.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                // Applique un déplacement en utilisant la physique
                Vector2 direction = (mousePosition - (Vector2)selectedObject.transform.position).normalized;
                rb.linearVelocity = direction * smoothSpeed;
            }

            velocity = (mousePosition - (Vector2)selectedObject.transform.position) * smoothSpeed;

            if (Vector2.Distance((Vector2)selectedObject.transform.position, mousePosition) < stopDistance)
            {
                StopDragging();
            }
        }
    }

    void ApplyDragInertia()
    {
        if (selectedObject != null)
        {
            Rigidbody2D rb = selectedObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Applique une inertie à l'objet après le drag
                rb.linearVelocity = velocity;

                // Réduit progressivement la vitesse pour simuler la friction
                velocity *= dragFriction;

                if (velocity.magnitude < 0.01f)
                {
                    velocity = Vector2.zero;
                }
            }
        }
    }

    void StopDragging()
    {
        isDragging = false;

        if (selectedObject != null)
        {
            Rigidbody2D rb = selectedObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                velocity = rb.linearVelocity;  // La vitesse finale du Rigidbody est utilisée comme inertie
                rb.linearVelocity = Vector2.zero;  // Arrête tout mouvement immédiat
            }
        }

        selectedObject = null;
    }
}
