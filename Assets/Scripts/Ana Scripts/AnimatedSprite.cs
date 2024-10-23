using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AnimatedSprite : MonoBehaviour
{
    public Sprite[] runningSprites;     // Sprites para la animación de correr
    public Sprite[] crouchingSprites;   // Sprites para la animación de agacharse

    private Sprite[] currentSprites;    // Sprites actuales en uso
    private SpriteRenderer spriteRenderer;
    private int frame;
    private Player player;              // Referencia al script del jugador para obtener el estado

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GetComponent<Player>();
    }

    private void OnEnable()
    {
        currentSprites = runningSprites; // Iniciamos con los sprites de correr
        Invoke(nameof(Animate), 0f);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void Update()
    {
        // Actualizar el conjunto de sprites según el estado del jugador
        if (player.IsCrouching()) // Método que debes implementar en Player para chequear si está agachado
        {
            currentSprites = crouchingSprites;
        }
        else
        {
            currentSprites = runningSprites;
        }
    }

    private void Animate()
    {
        frame++;

        if (frame >= currentSprites.Length)
        {
            frame = 0;
        }

        if (frame >= 0 && frame < currentSprites.Length)
        {
            spriteRenderer.sprite = currentSprites[frame];
        }

        Invoke(nameof(Animate), 1f / GameManager.Instance.gameSpeed);
    }
}

