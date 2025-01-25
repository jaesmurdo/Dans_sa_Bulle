using UnityEngine;

public class CircleStateController : MonoBehaviour
{
    [System.Serializable]
    public class State
    {
        public Vector3 scale; // Taille associée à cet état
        public Color color; // Couleur associée à cet état
    }

    public Player playerScript; // Référence au script Player

    [Header("States Configuration")]
    public State[] states; // Tableau des états
    public GameObject bubblePrefab; // Prefab de la bulle à instancier après fusion

    private int currentIndex; // Index de l'état actuel
    private int targetIndex; // Index cible de l'état pour interpolation

    private Renderer circleRenderer; // Renderer du rond
    private Collider2D circleCollider; // Collider de la bulle

    [Header("Lerp Configuration")]
    [SerializeField] private float lerpDuration = 1f; // Durée de l'interpolation (modifiable dans l'inspecteur)
    [SerializeField] private AnimationCurve lerpCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f); // Courbe de lerp non linéaire (modifiable dans l'inspecteur)

    private float lerpTime = 0f; // Temps écoulé pour lerp
    private bool isInterpolating = false; // Flag pour savoir si l'interpolation est en cours

    [Header("Increment Configuration")]
    public float incrementInterval = 5f; // Intervalle pour l'incrémentation dans chaque cercle
    private float timeSinceLastIncrement = 0f; // Temps depuis la dernière incrémentation

    void Start()
    {
        // Trouver le GameObject "Player" dans la scène
        GameObject playerObject = GameObject.Find("Player"); // Assure-toi que le GameObject s'appelle bien "Player"

        if (playerObject != null)
        {
            // Récupérer le script Player attaché à l'objet
            playerScript = playerObject.GetComponent<Player>();
        }
        else
        {
            Debug.LogError("GameObject 'Player' not found!");
        }

        if (states.Length == 0)
        {
            return;
        }

        circleRenderer = GetComponent<Renderer>();
        circleCollider = GetComponent<Collider2D>(); // Récupère le Collider2D de la bulle

        if (circleRenderer == null || circleCollider == null)
        {
            return;
        }

        // Initialiser à un état aléatoire
        currentIndex = Random.Range(0, states.Length);
        targetIndex = currentIndex;
        ApplyState();
    }

    public void InitializeRandomState()
    {
        if (states.Length == 0)
        {
            return;
        }

        currentIndex = Random.Range(0, states.Length);
        targetIndex = currentIndex;
        ApplyState();
    }

    // Décrémente l'état du cercle
    public void DecrementState()
    {
        if (currentIndex == 0)
        {
            Destroy(gameObject);
            return;
        }

        targetIndex = currentIndex - 1;
        lerpTime = 0f;

        if (!isInterpolating)
        {
            isInterpolating = true;
        }
    }

    // Incrémente l'état du cercle
    public void IncrementState()
    {
        if (currentIndex == states.Length - 1)
        {
            Destroy(gameObject); // Détruit l'objet
            playerScript.TakeDamage();
            return; // Sort de la méthode pour éviter de continuer
        }

        targetIndex = currentIndex + 1;
        lerpTime = 0f;

        if (!isInterpolating)
        {
            isInterpolating = true;
        }
    }

    void Update()
    {
        // Gérer l'incrémentation avec un intervalle de temps par cercle
        timeSinceLastIncrement += Time.deltaTime;

        if (timeSinceLastIncrement >= incrementInterval)
        {
            IncrementState();
            timeSinceLastIncrement = 0f;
        }

        if (isInterpolating)
        {
            lerpTime += Time.deltaTime / lerpDuration;
            float lerpFactor = lerpCurve.Evaluate(lerpTime);

            transform.localScale = Vector3.Lerp(states[currentIndex].scale, states[targetIndex].scale, lerpFactor);
            circleRenderer.material.color = Color.Lerp(states[currentIndex].color, states[targetIndex].color, lerpFactor);

            if (lerpTime >= 1f)
            {
                currentIndex = targetIndex;
                ApplyState();
                isInterpolating = false;
            }
        }
    }

    private void ApplyState()
    {
        if (currentIndex < 0 || currentIndex >= states.Length)
        {
            return;
        }

        transform.localScale = states[currentIndex].scale;
        circleRenderer.material.color = states[currentIndex].color;
    }

    public Vector3 GetTargetScale()
    {
        if (states.Length == 0 || currentIndex < 0 || currentIndex >= states.Length)
        {
            return Vector3.zero; // Retourne zéro si l'état est invalide
        }
        return states[currentIndex].scale; // Retourne la taille correspondant à l'état actuel
    }

    // Gestion des collisions
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Vérifie si la collision est avec un autre objet ayant le tag "bubble"
        if (collision.gameObject.CompareTag("bubble"))
        {
            CircleStateController otherCircle = collision.gameObject.GetComponent<CircleStateController>();

            // Si l'autre objet a aussi un CircleStateController
            if (otherCircle != null)
            {
                // Compare les index
                if (currentIndex == otherCircle.currentIndex)
                {
                    // Détruire les deux objets
                    Vector2 collisionPoint = collision.GetContact(0).point; // Point de collision

                    Destroy(collision.gameObject); // Détruit l'autre bulle
                    Destroy(gameObject); // Détruit cette bulle

                    // Instancier une nouvelle bulle à l'index supérieur
                    if (bubblePrefab != null && currentIndex + 1 < states.Length)
                    {
                        GameObject newBubble = Instantiate(bubblePrefab, collisionPoint, Quaternion.identity);
                        CircleStateController newCircleController = newBubble.GetComponent<CircleStateController>();

                        if (newCircleController != null)
                        {
                            newCircleController.SetState(currentIndex + 1); // Définir le nouvel état

                            // **Activer le script et le collider**
                            newCircleController.enabled = true; // Active le script
                            newCircleController.circleCollider.enabled = true; // Active le collider

                            Debug.Log("Script et Collider activés pour la nouvelle bulle!"); // Log pour le débogage
                        }
                    }
                }
            }
        }
    }

    // Définit manuellement un état pour une bulle nouvellement créée
    public void SetState(int index)
    {
        if (index < 0 || index >= states.Length)
        {
            return;
        }

        currentIndex = index;
        targetIndex = currentIndex;
        ApplyState();
    }
}
