using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float dashSpeed = 10f;
    [SerializeField] float gravity = -20f;

    [Header("Camera")]
    [SerializeField] Transform cameraRoot;
    [SerializeField] float mouseSensitivity = 0.15f;
    [SerializeField] float pitchMin = -40f;
    [SerializeField] float pitchMax = 60f;

    CharacterController cc;
    float verticalVelocity;
    float pitch;
    float yaw;

    public bool IsDashing { get; private set; }
    public bool IsAlive { get; private set; } = true;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        yaw = transform.eulerAngles.y;
    }

    void Update()
    {
        if (!IsAlive || GameManager.Instance.State != GameManager.GameState.Playing)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }

        RotateCamera();
        Move();
    }

    void RotateCamera()
    {
        Vector2 delta = Mouse.current.delta.ReadValue();
        yaw   += delta.x * mouseSensitivity;
        pitch -= delta.y * mouseSensitivity;
        pitch  = Mathf.Clamp(pitch, pitchMin, pitchMax);

        transform.rotation       = Quaternion.Euler(0f, yaw, 0f);
        cameraRoot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void Move()
    {
        var kb = Keyboard.current;
        float h = (kb.dKey.isPressed ? 1f : 0f) - (kb.aKey.isPressed ? 1f : 0f);
        float v = (kb.wKey.isPressed ? 1f : 0f) - (kb.sKey.isPressed ? 1f : 0f);

        IsDashing = kb.leftShiftKey.isPressed;
        float speed = IsDashing ? dashSpeed : walkSpeed;

        Vector3 move = (transform.right * h + transform.forward * v).normalized * speed;

        if (cc.isGrounded)
            verticalVelocity = -2f;
        verticalVelocity += gravity * Time.deltaTime;
        move.y = verticalVelocity;

        cc.Move(move * Time.deltaTime);
    }

    public void Die()
    {
        IsAlive = false;
    }
}
