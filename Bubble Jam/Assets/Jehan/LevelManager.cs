using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public float spawnInterval = 3f; // Intervalle de temps en secondes pour le spawn
    public GameObject[] spawnLocations; // Tableau des emplacements de spawn (Empty GameObjects dans la scène)
    public GameObject objectToSpawn; // Le prefab à instancier

    [Header("Bubble Management")]
    [SerializeField] private int maxBubblesAllowed = 3; // Nombre maximum de bulles autorisées à spawn
    [SerializeField] private float minimumDistanceBetweenBubbles = 0.3f; // Distance minimale entre les bulles

    [Header("Sound Configuration")]
    [SerializeField] private AudioClip spawnSound; // Son à jouer lors du spawn
    private AudioSource audioSource; // Référence à l'AudioSource

    private float timeSinceLastSpawn = 0f; // Temps depuis le dernier spawn
    private float elapsedTime = 0f; // Temps écoulé depuis le début du jeu

    [Header("Time Display")]
    public TextMeshProUGUI timerText; // Référence au TextMeshPro pour afficher le temps
    public TimeColor[] timeColorPallet; // Tableau des paliers de temps et des couleurs associées

    void Start()
    {
        if (spawnLocations.Length == 0 || objectToSpawn == null) return;

        // Récupère l'AudioSource attaché à ce GameObject
        audioSource = GetComponent<AudioSource>();

        // Vérifie si AudioSource est bien attaché
        if (audioSource == null)
        {
            Debug.LogError("AudioSource non trouvé sur ce GameObject.");
        }

        // Vérifie si spawnSound est assigné
        if (spawnSound == null)
        {
            Debug.LogError("Le son de spawn n'est pas assigné dans l'inspecteur.");
        }

        // Initialiser l'affichage du temps si un TextMeshPro est assigné
        if (timerText != null)
        {
            timerText.text = "0"; // Affiche 0 au début
        }
    }

    void Update()
    {
        elapsedTime += Time.deltaTime; // Met à jour le temps écoulé
        timeSinceLastSpawn += Time.deltaTime;

        // Met à jour l'affichage du temps
        UpdateTimerDisplay();

        // Vérifie si le spawn doit avoir lieu
        if (timeSinceLastSpawn >= spawnInterval)
        {
            timeSinceLastSpawn = 0f;

            // Vérifie si le nombre actuel de bulles dans la scène est inférieur au maximum autorisé
            int currentBubbleCount = GameObject.FindGameObjectsWithTag("bubble").Length;

            if (currentBubbleCount < maxBubblesAllowed)
            {
                SpawnObjectAtRandomLocation();
            }
            else
            {
                Debug.LogWarning("Nombre maximum de bulles atteint, impossible de spawn une nouvelle bulle.");
            }
        }
    }

    void UpdateTimerDisplay()
    {
        // Affiche le temps arrondi au chiffre entier
        if (timerText != null)
        {
            int timeToDisplay = Mathf.FloorToInt(elapsedTime);
            timerText.text = timeToDisplay.ToString();

            // Change la couleur du texte en fonction des paliers
            foreach (var timeColor in timeColorPallet)
            {
                if (timeToDisplay >= timeColor.timeInSeconds)
                {
                    timerText.color = timeColor.color; // Applique la couleur associée

                    // Modifie la distance entre les bulles si un modificateur est présent
                    minimumDistanceBetweenBubbles = timeColor.distanceModifier;
                    Debug.Log($"Nouveau minimumDistanceBetweenBubbles: {minimumDistanceBetweenBubbles}");

                    // Modifie le nombre maximum de bulles autorisées si un modificateur est présent
                    maxBubblesAllowed = timeColor.maxBubblesModifier;
                    Debug.Log($"Nouveau maxBubblesAllowed: {maxBubblesAllowed}");

                    // Modifie l'intervalle de spawn si un modificateur est présent
                    spawnInterval = timeColor.spawnIntervalModifier;
                    Debug.Log($"Nouveau spawnInterval: {spawnInterval}");
                }
            }
        }
    }

    void SpawnObjectAtRandomLocation()
    {
        if (spawnLocations.Length == 0 || objectToSpawn == null) return;

        bool spawnSuccessful = false;

        for (int attempt = 0; attempt < 20; attempt++)
        {
            int randomIndex = Random.Range(0, spawnLocations.Length);
            Vector3 spawnPosition = spawnLocations[randomIndex].transform.position;

            bool isOccupied = IsLocationOccupied(spawnPosition);

            if (!isOccupied)
            {
                GameObject spawnedObject = Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);

                if (audioSource != null && spawnSound != null)
                {
                    audioSource.PlayOneShot(spawnSound);
                }

                spawnSuccessful = true;
                break;
            }
        }

        if (!spawnSuccessful)
        {
            Debug.LogWarning("Impossible de spawn une bulle après plusieurs tentatives.");
        }
    }

    bool IsLocationOccupied(Vector3 position)
    {
        GameObject[] existingBubbles = GameObject.FindGameObjectsWithTag("bubble");

        foreach (var bubble in existingBubbles)
        {
            if (Vector3.Distance(bubble.transform.position, position) < minimumDistanceBetweenBubbles)
            {
                return true;
            }
        }

        return false;
    }

    [System.Serializable]
    public struct TimeColor
    {
        public int timeInSeconds; // Temps en secondes
        public Color color; // Couleur associée
        public float distanceModifier; // Modificateur de la distance entre les bulles
        public int maxBubblesModifier; // Modificateur du nombre maximum de bulles autorisées
        public float spawnIntervalModifier; // Modificateur pour l'intervalle de spawn
    }
}
