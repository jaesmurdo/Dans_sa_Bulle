using UnityEngine;

public class ButtonMovement : MonoBehaviour
{
    public float speed = 100f; // Vitesse de déplacement vers la cible
    public float floatAmplitude = 30f; // Amplitude du mouvement flottant
    public float noiseFrequency = 1f; // Fréquence des variations aléatoires

    public float randomTargetRadius = 100f; // Rayon pour les cibles aléatoires
    private Vector2 currentTarget; // Stocke la cible actuelle
    private RectTransform rectTransform; // Référence au RectTransform du bouton

    private Vector2 initialPosition; // Position initiale pour le mouvement flottant
    private float timer = 0f; // Gère les changements de cible
    public float targetChangeInterval = 2f; // Intervalle de changement de cible (en secondes)

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        // Définit la position initiale
        initialPosition = rectTransform.anchoredPosition;

        // Génère une première cible aléatoire
        currentTarget = GetRandomTarget(randomTargetRadius);

        // Initialise une fréquence de bruit aléatoire
        noiseFrequency = Random.Range(0.5f, 2.0f);
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Change la cible à intervalles réguliers
        if (timer >= targetChangeInterval)
        {
            currentTarget = GetRandomTarget(randomTargetRadius);
            timer = 0f;
        }

        // Applique le mouvement vers la cible avec des variations aléatoires
        MoveTowards(currentTarget);
    }

    private void MoveTowards(Vector2 target)
    {
        // Génère un mouvement flottant aléatoire
        Vector2 floatOffset = GetUnpredictableOffset();

        // Combine le mouvement vers la cible et l'offset
        Vector2 newPosition = Vector2.MoveTowards(rectTransform.anchoredPosition, target + floatOffset, speed * Time.deltaTime);

        // Met à jour la position du bouton
        rectTransform.anchoredPosition = newPosition;
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

        // Retourne une position relative à la position initiale
        return initialPosition + new Vector2(x, y);
    }
}