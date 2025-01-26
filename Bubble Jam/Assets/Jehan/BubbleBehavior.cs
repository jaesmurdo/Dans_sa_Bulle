using UnityEngine;
using System.Collections;

public class CircleStateController : MonoBehaviour
{
    [System.Serializable]
    public class State
    {
        public Vector3 scale; // Taille associée à cet état
        public Color color; // Couleur associée à cet état
    }

    public Player playerScript; // Référence au script Player
    public ShakeEffectController shakeEffectController; // Référence au script de secousse
    private Vector3 collisionPoint; // Point de collision
    [SerializeField] private float moveToCollisionDuration = 0.01f; // Durée pour atteindre le point de collision
    

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

    [Header("Sound Configuration")]
    [SerializeField] private AudioClip spawnSound; // Son à jouer lors de l'apparition (spawn)
    [SerializeField] private AudioClip incrementSound; // Son à jouer lors de l'incrémentation
    [SerializeField] private AudioClip decrementSound; // Son à jouer lors de la décrémentation
    [SerializeField] private AudioClip destructionSound; // Son à jouer lors de la destruction
    [SerializeField] private AudioClip explosionSound; // Son d'explosion
    [SerializeField] private AudioClip pointSound; // Son à jouer lors de la destruction à l'index 0

    private AudioSource audioSource; // Référence à l'AudioSource

    void Start()
    {
        // Initialisation des composants et son
        circleRenderer = GetComponent<Renderer>();
        circleCollider = GetComponent<Collider2D>(); // Récupère le Collider2D de la bulle
        audioSource = GetComponent<AudioSource>();

        if (circleRenderer == null || circleCollider == null)
        {
            return;
        }

        // Initialiser à un état aléatoire
        currentIndex = Random.Range(0, states.Length);
        targetIndex = currentIndex;
        ApplyState();

        // Jouer le son de spawn lors de l'apparition de la bulle
        if (audioSource != null && spawnSound != null)
        {
            audioSource.PlayOneShot(spawnSound); // Joue le son de spawn
        }
    }

    void Update()
    {
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

    public void DecrementState()
    {
        if (currentIndex > 0)
        {
            targetIndex = currentIndex - 1;
            lerpTime = 0f;

            if (!isInterpolating)
            {
                isInterpolating = true;
            }

            // Joue le son de décrémentation
            if (audioSource != null && decrementSound != null)
            {
                audioSource.PlayOneShot(decrementSound);
            }
        }
        else
        {
            // Si l'index est déjà à 0, détruit la bulle avec le son "point sound"
            StartCoroutine(DestroyWithPointSound());
        }
    }

    public void InitializeRandomState()
    {
        currentIndex = Random.Range(0, states.Length); // Choisit un index aléatoire
        targetIndex = currentIndex;
        ApplyState();
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

    private void IncrementState()
    {
        if (currentIndex == states.Length - 1)
        {
            if (audioSource != null && destructionSound != null)
            {
                audioSource.PlayOneShot(destructionSound);
            }

            Destroy(gameObject);
            return;
        }

        targetIndex = currentIndex + 1;
        lerpTime = 0f;

        if (!isInterpolating)
        {
            isInterpolating = true;
        }

        if (audioSource != null && incrementSound != null)
        {
            audioSource.PlayOneShot(incrementSound);
        }

        // Activer la secousse si l'index atteint 4
        if (targetIndex == 4 && shakeEffectController != null)
        {
            shakeEffectController.StartShake();
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Vérifie que l'objet en collision est étiqueté "bubble"
        if (collision.gameObject.CompareTag("bubble"))
        {
            Debug.Log("Collision détectée avec un objet étiqueté 'bubble'.");

            // Récupère le script CircleStateController de l'autre objet
            CircleStateController otherCircle = collision.gameObject.GetComponent<CircleStateController>();

            if (otherCircle != null)
            {
                Debug.Log("Autre objet possède un CircleStateController.");

                // Vérifie si les deux objets ont le même index
                if (currentIndex == otherCircle.currentIndex)
                {
                    Debug.Log("Les deux objets ont le même index. Déclenchement de l'animation de fusion.");

                    // Déterminer le point de collision
                    collisionPoint = collision.GetContact(0).point;

                    // Désactiver les colliders des deux objets
                    circleCollider.enabled = false;
                    otherCircle.circleCollider.enabled = false;

                    // Déplacer les deux bulles vers le point de collision
                    StartCoroutine(MoveToCollisionPoint(otherCircle));
                }
                else
                {
                    Debug.Log("Les objets n'ont pas le même index. Aucune destruction.");
                }
            }
            else
            {
                Debug.Log("L'autre objet n'a pas de CircleStateController.");
            }
        }
        else
        {
            Debug.Log("L'objet en collision n'est pas une 'bubble'.");
        }
    }

    // Coroutine pour déplacer les bulles vers le point de collision
    private IEnumerator MoveToCollisionPoint(CircleStateController otherCircle)
    {
        Vector3 startPosition = transform.position; // Position initiale
        Vector3 otherStartPosition = otherCircle.transform.position; // Position initiale de l'autre bulle

        float elapsedTime = 0f;

        while (elapsedTime < moveToCollisionDuration)
        {
            // Calculer le facteur de progression
            float lerpFactor = elapsedTime / moveToCollisionDuration;

            // Déplacer la bulle actuelle et l'autre bulle vers le point de collision
            transform.position = Vector3.Lerp(startPosition, collisionPoint, lerpFactor);
            otherCircle.transform.position = Vector3.Lerp(otherStartPosition, collisionPoint, lerpFactor);

            elapsedTime += Time.deltaTime;
            yield return null; // Attendre la prochaine frame
        }

        // S'assurer que les positions sont alignées avec le point de collision
        transform.position = collisionPoint;
        otherCircle.transform.position = collisionPoint;

        // Détruire les deux bulles
        Destroy(gameObject);
        Destroy(otherCircle.gameObject);
    }

    // Coroutine pour retarder la destruction avec le son "point sound"
    private IEnumerator DestroyWithPointSound()
    {
        // Joue le son "point sound"
        if (audioSource != null && pointSound != null)
        {
            audioSource.PlayOneShot(pointSound);
        }

        // Attente de 0.1 seconde
        yield return new WaitForSeconds(0.02f);

        // Détruire l'objet
        Destroy(gameObject);
    }

    private IEnumerator DelayedDestroy(CircleStateController otherCircle)
    {
        // Attend un peu avant de détruire les objets (par exemple 0.5 seconde)
        yield return new WaitForSeconds(0.5f);

        DestroyWithExplosionSound();
        otherCircle.DestroyWithExplosionSound();
    }

    private void DestroyWithExplosionSound()
    {
        // Joue le son d'explosion
        if (audioSource != null && explosionSound != null)
        {
            audioSource.PlayOneShot(explosionSound);
        }
        Destroy(gameObject); // Détruire l'objet
    }
}
