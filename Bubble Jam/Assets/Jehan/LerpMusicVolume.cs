using UnityEngine;

public class LerpVolume : MonoBehaviour
{
    [Header("Volume Configuration")]
    [SerializeField] private float startVolume = 0f; // Volume de départ
    [SerializeField] private float endVolume = 1f; // Volume d'arrivée
    [SerializeField] private float lerpDuration = 2f; // Durée de l'interpolation (en secondes)

    private AudioSource audioSource; // Référence à l'AudioSource
    private float lerpTime = 0f; // Temps écoulé pendant l'interpolation

    void Start()
    {
        // Récupérer l'AudioSource attaché au GameObject
        audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
        {
            // Définir le volume initial
            audioSource.volume = startVolume;
        }
        else
        {
            Debug.LogError("AudioSource component is missing!");
        }
    }

    void Update()
    {
        if (audioSource != null)
        {
            // Interpolation du volume
            lerpTime += Time.deltaTime / lerpDuration;
            audioSource.volume = Mathf.Lerp(startVolume, endVolume, lerpTime);

            // Si l'interpolation est terminée, arrêter l'update
            if (lerpTime >= 1f)
            {
                lerpTime = 1f; // Garantir que la valeur ne dépasse pas 1
            }
        }
    }
}
