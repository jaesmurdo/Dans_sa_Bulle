using UnityEngine;

public class ShakeEffectController : MonoBehaviour
{
    [Header("Shake Configuration")]
    public float lerpDuration = 2f; // Durée totale du Lerp
    public AnimationCurve intensityCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f); // Courbe d'intensité de la secousse
    public float maxShakeIntensity = 1f; // Intensité maximale de la secousse
    public float shakeFrequency = 20f; // Fréquence des oscillations de la secousse

    private float elapsedLerpTime = 0f; // Temps écoulé pour le Lerp
    private Vector3 originalPosition; // Position d'origine de la bulle
    private bool isShaking = false; // Indique si le secouement est actif

    void Start()
    {
        // Enregistre la position d'origine
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        if (isShaking)
        {
            // Progression du Lerp
            elapsedLerpTime += Time.deltaTime;
            float lerpFactor = Mathf.Clamp01(elapsedLerpTime / lerpDuration);

            // Calcul de l'intensité actuelle à partir de la courbe
            float intensity = intensityCurve.Evaluate(lerpFactor) * maxShakeIntensity;

            // Appliquer une secousse aléatoire basée sur l'intensité et la fréquence
            Vector3 shakeOffset = new Vector3(
                Mathf.Sin(Time.time * shakeFrequency) * intensity,
                Mathf.Cos(Time.time * shakeFrequency) * intensity,
                0f
            );

            transform.localPosition = originalPosition + shakeOffset;

            // Arrêter la secousse une fois la durée terminée
            if (lerpFactor >= 1f)
            {
                StopShake();
            }
        }
    }

    // Méthode pour activer la secousse
    public void StartShake()
    {
        if (!isShaking)
        {
            isShaking = true;
            elapsedLerpTime = 0f; // Réinitialise le temps écoulé
            originalPosition = transform.localPosition; // Enregistre la position d'origine
        }
    }

    // Méthode pour arrêter la secousse
    public void StopShake()
    {
        isShaking = false;
        transform.localPosition = originalPosition; // Réinitialise à la position d'origine
    }
}
