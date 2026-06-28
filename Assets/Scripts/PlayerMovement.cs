using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float gravity = -9.81f;

    private CharacterController controller;
    private Vector3 velocity;

    [Header("Footsteps Audio")]
    [SerializeField] private AudioSource footstepAudioSource; 
    [SerializeField] private AudioClip[] footstepClips; // Acá vas a meter tus 5 audios
    [SerializeField] private float stepInterval = 0.5f;

    private float stepTimer;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        // Chequea si el jugador se está moviendo con WASD en este frame
        bool isMoving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);

        // Si se mueve y está tocando el suelo (asumiendo que uses una variable tipo isGrounded o similar)
        // Si no usas "isGrounded", borra esa parte y deja solo: if (isMoving)
        if (isMoving)
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f)
            {
                PlayRandomFootstep();
                stepTimer = stepInterval; // Reseteamos el temporizador
            }
        }
        else
        {
            stepTimer = 0f; // Si se frena, el próximo paso suena instantáneo al caminar
        }
    }

    private void PlayRandomFootstep()
    {
        if (footstepClips == null || footstepClips.Length == 0 || footstepAudioSource == null) return;

        // Elegimos un índice al azar entre 0 y 4
        int randomIndex = Random.Range(0, footstepClips.Length);

        // Reproduce ese pasito de forma única sin pisar los anteriores si se superponen un milisegundo
        footstepAudioSource.PlayOneShot(footstepClips[randomIndex]);
    }
}