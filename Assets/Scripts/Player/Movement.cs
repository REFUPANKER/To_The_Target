using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Movement : MonoBehaviour
{
    public CinemachineCamera cam;
    public CharacterController controller;
    public bool CanMove = true;
    [SerializeField] Parachute pchute;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpHeight = 1.5f;
    [SerializeField] float gravity = -19.62f;
    [SerializeField] float mouseSensitivity = 100, clampMin = -70, clampMax = 70;
    [SerializeField] LayerMask groundMask;
    [SerializeField] Animator anims;

    Vector3 moveDirection;
    bool isGrounded;
    [SerializeField] CinemachineCamera camFPS;
    float rotx;
    void Update()
    {
        if (!CanMove) { return; }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        rotx -= mouseY;
        rotx = Mathf.Clamp(rotx, clampMin, clampMax);
        camFPS.transform.localRotation = Quaternion.Euler(rotx, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        isGrounded = controller.isGrounded;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        if (move.magnitude > 1f) { move.Normalize(); }

        moveDirection.x = move.x * moveSpeed;
        moveDirection.z = move.z * moveSpeed;

        if (isGrounded)
        {
            moveDirection.y = -2f;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                moveDirection.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        else { moveDirection.y += gravity * Time.deltaTime; }

        controller.Move(moveDirection * Time.deltaTime);
        anims.SetFloat("velocity", controller.velocity.magnitude);
    }
}