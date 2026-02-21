using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class MoveController : MonoBehaviour
{
    private BoatInput boatInput;
    public float Speed = 3f;
    Vector2 moveInput;
    private Rigidbody rb;
    private Vector3 currentMoveForce;
    private bool isMoving = false;

    void Awake()
    {
        boatInput = new BoatInput();
        rb = GetComponent<Rigidbody>();
    }
    void OnEnable()
    {
        boatInput.Boat.Move.performed += OnMove;
        boatInput.Boat.Move.canceled += OnMove;
        boatInput.Boat.Enable();
    }
    void OnDisable()
    {
        boatInput.Boat.Move.performed -= OnMove;
        boatInput.Boat.Move.canceled -= OnMove;
        boatInput.Boat.Disable();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(isMoving)
        {
            rb.AddForce(currentMoveForce, ForceMode.Acceleration);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if(moveInput.sqrMagnitude > 0.01f)
        {
            currentMoveForce = new Vector3(moveInput.x, 0, moveInput.y) * Speed;
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
    }
}
