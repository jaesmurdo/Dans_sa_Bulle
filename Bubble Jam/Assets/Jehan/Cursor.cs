using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    public Texture2D customCursorTexture; // Texture pour le curseur personnalisé
    public Vector2 hotSpot = Vector2.zero; // Point chaud du curseur (là où le clic est enregistré)
    public CursorMode cursorMode = CursorMode.Auto; // Mode du curseur

    void Start()
    {
        // Vérifie si une texture est assignée
        if (customCursorTexture != null)
        {
            // Définit le curseur personnalisé avec la texture, le point chaud et le mode
            Cursor.SetCursor(customCursorTexture, hotSpot, cursorMode);
        }
        else
        {
            Debug.LogWarning("Aucune texture n'a été assignée pour le curseur personnalisé !");
        }
    }

    public void ResetCursor()
    {
        // Réinitialise le curseur au curseur par défaut
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }
}
