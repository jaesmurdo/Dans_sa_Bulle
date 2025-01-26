using UnityEngine;

public class CircleStateController : MonoBehaviour
{
    [System.Serializable]
    public class State
    {
        public Vector3 scale; // Taille associée à cet état
        public Color color;   // Couleur associée à cet état
    }

    [Header("Player Configuration")]
    private Player playerScript;   // Référence au script Player

    [Header("Shake Effect Configuration")]
    public ShakeEffectController shakeEffectController; // Référence au script de secousse

    [Header("States Configuration")]
    public State[] states;         // Tableau des états
    public GameObject bubblePrefab; // Prefab de la bulle à instancier après fusion

    private int currentIndex;       // Index de l'état actuel
    private int targetIndex;        // Index cible de l'état pour interpolation

    private Renderer circleRenderer; // Renderer du cercle
    private Collider2D circleCollider; // Collider de la bulle

    [Header("Lerp Configuration")]
    [SerializeField] private float lerpDuration = 1f; // Durée de l'interpolation
    [SerializeField] private AnimationCurve lerpCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f); // Courbe d'interpolation

    private float lerpTime = 0f;       // Temps écoulé pour l'interpolation
    private bool isInterpolating = false; // Flag pour savoir si l'interpolation est en cours

    [Header("Increment Configuration")]
    public float incrementInterval = 5f; // Intervalle pour l'incrémentation
    private float timeSinceLastIncrement = 0f; // Temps depuis la dernière incrémentation

    [Header("Sound Configuration")]
    [SerializeField] private AudioClip spawnSound;
    [SerializeField] private AudioClip incrementSound;
    [SerializeField] private AudioClip decrementSound;
    [SerializeField] private AudioClip destructionSound;

    private AudioSource audioSource;

    void Start()
    {
        // Initialisation des composants et audio
        circleRenderer = GetComponent<Renderer>();
        circleCollider = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();

        if (circleRenderer == null || circleCollider == null)
        {
            Debug.LogError("Renderer ou Collider non trouvé !");
            return;
        }

        // Recherche du Player via son script
        playerScript = FindObjectOfType<Player>();
        if (playerScript == null)
        {
            Debug.LogError("Aucun objet dans la scène ne contient le script Player !");
            return;
        }

        // Initialiser à un état aléatoire
        currentIndex = Random.Range(0, states.Length);
        targetIndex = currentIndex;
        ApplyState();

        // Jouer le son de spawn
        if (audioSource != null && spawnSound != null)
        {
            audioSource.PlayOneShot(spawnSound);
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
            DestroyWithSound();
        }
    }

    private void IncrementState()
    {
        if (currentIndex == states.Length - 1)
        {
            // Joue le son de destruction
            if (audioSource != null && destructionSound != null)
            {
                audioSource.PlayOneShot(destructionSound);
            }

            // Applique des dégâts au joueur
            if (playerScript != null)
            {
                playerScript.TakeDamage();
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

    private void ApplyState()
    {
        if (currentIndex < 0 || currentIndex >= states.Length)
        {
            return;
        }

        transform.localScale = states[currentIndex].scale;
        circleRenderer.material.color = states[currentIndex].color;
    }

    private void DestroyWithSound()
    {
        if (audioSource != null && destructionSound != null)
        {
            audioSource.PlayOneShot(destructionSound);
        }

        Destroy(gameObject);
    }
}
