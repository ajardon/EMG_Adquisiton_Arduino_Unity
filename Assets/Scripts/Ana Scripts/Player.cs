using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    private CharacterController character;
    private Vector3 direction;

    public float jumpForce = 8f;
    public float gravity = 9.81f * 2f;
    public float crouchHeight = 0.1f; // Altura cuando el jugador está agachado
    private float originalHeight;     // Altura original del CharacterController
    private Vector3 originalCenter;

    private int jumpCount = 0;
    private int crouchCount = 0;
    private bool isCrouching = false;
    private bool isJumping = false;

    public int JumpCount => jumpCount;
    public int CrouchCount => crouchCount;

    private float crouchDuration = 2f; // Duración de estar agachado en segundos
    private float crouchTimer = 0f;

    private void Awake()
    {
        character = GetComponent<CharacterController>();
        originalHeight = character.height; // Guardar la altura original
        originalCenter = character.center;
    }

    public void ResetCounts()
    {
        jumpCount = 0;
        crouchCount = 0;
    }

    private void OnEnable()
    {
        direction = Vector3.zero;
    }

    private void Update()
    {
        direction += gravity * Time.deltaTime * Vector3.down;

        if (character.isGrounded)
        {
            direction = Vector3.down;

            if (ArduinoReaderUI.isContracted1 )
            {
                StandUp();
                Jump();
            }

            if (ArduinoReaderUI.isContracted2 && !isCrouching)
            {
                StartCrouch();
            }

            if (isCrouching)
            {
                HandleCrouchTimer();
            }
        }

        character.Move(direction * Time.deltaTime);
    }

    private void Jump()
    {
        Debug.Log("JUMP");
        direction = Vector3.up * jumpForce;
        isJumping = true;
        jumpCount++;
    }

    private void StartCrouch()
    {
        isCrouching = true;
        crouchTimer = crouchDuration; // Reiniciar el temporizador
        character.height = crouchHeight; // Reducir la altura del CharacterController
        character.center = new Vector3(originalCenter.x, crouchHeight / 2, originalCenter.z); // Ajustar el centro
        crouchCount++;
        Debug.Log($"Crouching: Height = {character.height}, Center = {character.center}");
    }

    private void HandleCrouchTimer()
    {
        crouchTimer -= Time.deltaTime;
        if (crouchTimer <= 0f)
        {
            StandUp();
        }
    }

    private void StandUp()
    {
        isCrouching = false;
        character.height = originalHeight; // Restaurar la altura original
        character.center = originalCenter;
        Debug.Log("Standing Up");
    }

    public bool IsCrouching()
    {
        return isCrouching;
    }

    public bool IsJumping()
    {
        return isJumping;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            GameManager.Instance.GameOver();
        }
    }
}


