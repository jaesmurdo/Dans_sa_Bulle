using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public float spawnInterval = 3f; // Intervalle de temps en secondes pour le spawn
    public GameObject[] spawnLocations; // Tableau des emplacements de spawn (Empty GameObjects dans la scène)
    public GameObject objectToSpawn; // Le prefab à instancier

    [Header("Bubble Management")]
    [SerializeField] private int maxBubblesAllowed = 3; // Nombre maximum de bulles autorisées à spawn
    [SerializeField] private float minimumDistanceBetweenBubbles = 0.5f; // Distance minimale entre les bulles

    private float timeSinceLastSpawn = 0f; // Temps depuis le dernier spawn

    void Start()
    {
        if (spawnLocations.Length == 0 || objectToSpawn == null) return;
    }

    void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;

        // Vérifie si le spawn doit avoir lieu
        if (timeSinceLastSpawn >= spawnInterval)
        {
            // Réinitialiser le timer de spawn
            timeSinceLastSpawn = 0f;

            // Vérifie si le nombre actuel de bulles dans la scène est inférieur au maximum autorisé
            int currentBubbleCount = GameObject.FindGameObjectsWithTag("bubble").Length;

            if (currentBubbleCount < maxBubblesAllowed)
            {
                // Si le nombre de bulles est inférieur à la limite, spawn une nouvelle bulle
                SpawnObjectAtRandomLocation();
            }
        }
    }

    void SpawnObjectAtRandomLocation()
    {
        if (spawnLocations.Length == 0 || objectToSpawn == null) return;

        bool spawnSuccessful = false;

        // On tente de spawn jusqu'à ce qu'on trouve un emplacement libre
        for (int attempt = 0; attempt < 10; attempt++)
        {
            // Choisir un emplacement libre aléatoire
            int randomIndex = Random.Range(0, spawnLocations.Length);
            Vector3 spawnPosition = spawnLocations[randomIndex].transform.position;

            // Vérifier si l'emplacement est occupé par une autre bulle ou trop proche d'une autre bulle
            bool isOccupied = IsLocationOccupied(spawnPosition);

            if (!isOccupied)
            {
                // Si l'emplacement est libre, spawn l'objet à cette position
                GameObject spawnedObject = Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);

                // Initialiser l'état de la bulle de manière aléatoire
                CircleStateController circleStateController = spawnedObject.GetComponent<CircleStateController>();
                if (circleStateController != null)
                {
                    circleStateController.InitializeRandomState(); // Fonction pour initialiser un état aléatoire
                }

                spawnSuccessful = true;
                break; // Terminer dès qu'un spawn est réussi
            }
        }

        // Si aucune position libre n'a été trouvée après 10 tentatives, rien ne se passe.
        if (!spawnSuccessful)
        {
            Debug.LogWarning("Impossible de spawn une bulle après plusieurs tentatives.");
        }
    }

    bool IsLocationOccupied(Vector3 position)
    {
        // Vérifier si un objet est déjà présent à cet emplacement
        GameObject[] existingBubbles = GameObject.FindGameObjectsWithTag("bubble");

        foreach (var bubble in existingBubbles)
        {
            // Si la distance entre la position de la bulle et l'emplacement est suffisamment petite, c'est considéré comme occupé
            if (Vector3.Distance(bubble.transform.position, position) < minimumDistanceBetweenBubbles)
            {
                return true; // L'emplacement est occupé
            }
        }

        return false; // L'emplacement est libre
    }
}
