using UnityEngine;

public class BulleBehavior : MonoBehaviour
{
    public float speed = 5f; // Vitesse de déplacement vers la cible
    public float floatAmplitude = 0.5f; // Amplitude du mouvement flottant
    public float noiseFrequency = 1f; // Fréquence des variations aléatoires

    public bool isFleeing = false; // Indique si l'attaque est en cours
    private Vector2 initialPosition; // Position initiale pour le mouvement flottant

    public float randomTargetRadius = 3f; // Rayon pour les cibles aléatoires
    private Vector2 currentTarget; // Stocke la cible actuelle

    private Rigidbody2D rb; // Référence au Rigidbody2D

    void Start()
    {
        // Récupère le Rigidbody2D pour utiliser la physique pour le mouvement
        rb = GetComponent<Rigidbody2D>();

        // Définit une cible aléatoire au départ
        currentTarget = GetRandomTarget(randomTargetRadius);

        // Enregistre la position de départ
        initialPosition = transform.position;

        // Initialise une fréquence de bruit aléatoire entre 0.5 et 2.0
        noiseFrequency = Random.Range(0.5f, 2.0f);

        Debug.Log("Initial Noise Frequency: " + noiseFrequency); // Pour vérifier dans la console
    }

    void Update()
    {
        // Déclenche l'attaque avec la touche Espace
        if (Input.GetKeyDown(KeyCode.Space) && !isFleeing) // Vérifie si isFleeing est false
        {
            isFleeing = true;
            currentTarget = GetRandomTarget(randomTargetRadius); // Initialise une nouvelle cible directement
        }

        // Appliquer le mouvement
        if (isFleeing)
        {
            // Vérifie si l'objet est proche de la cible pour en générer une nouvelle
            if (Vector2.Distance(transform.position, currentTarget) < 0.1f)
            {
                isFleeing = false; // Désactive l'état fleeing une fois arrivé
                initialPosition = transform.position;
            }
            else
            {
                MoveTowards(currentTarget); // Continue de se déplacer vers la cible
            }
        }
        else
        {
            ApplyFloatingMotion(); // Mouvement flottant imprévisible
        }
    }

    // Déplacement vers une cible spécifique
    private void MoveTowards(Vector2 target)
    {
        // Déplacement vers la cible avec superposition du mouvement flottant
        Vector2 floatOffset = GetUnpredictableOffset();
        Vector2 newPosition = Vector2.MoveTowards(transform.position, target + floatOffset, speed * Time.deltaTime);

        // Déplacer l'objet via Rigidbody2D pour respecter les collisions et la physique
        rb.MovePosition(newPosition);
    }

    // Appliquer un mouvement flottant imprévisible autour de la position actuelle
    private void ApplyFloatingMotion()
    {
        Vector2 floatOffset = GetUnpredictableOffset();
        Vector2 newPosition = initialPosition + floatOffset;

        // Déplacer l'objet via Rigidbody2D pour respecter les collisions et la physique
        rb.MovePosition(newPosition);
    }

    // Calcul de l'offset pour le mouvement flottant imprévisible
    private Vector2 GetUnpredictableOffset()
    {
        // Génère des valeurs de bruit Perlin basées sur le temps
        float x = Mathf.PerlinNoise(Time.time * noiseFrequency, 0f) * 2f - 1f; // Valeur entre -1 et 1
        float y = Mathf.PerlinNoise(0f, Time.time * noiseFrequency) * 2f - 1f; // Valeur entre -1 et 1

        // Applique l'amplitude pour contrôler l'intensité du mouvement
        return new Vector2(x, y) * floatAmplitude;
    }

    private Vector2 GetRandomTarget(float radius)
    {
        // Génère un angle aléatoire en radians
        float angle = Random.Range(0f, Mathf.PI * 2);

        // Calcule les coordonnées en fonction du rayon et de l'angle
        float x = Mathf.Cos(angle) * radius;
        float y = Mathf.Sin(angle) * radius;

        // Retourne la position relative à la position actuelle
        return new Vector2(transform.position.x + x, transform.position.y + y);
    }
}
