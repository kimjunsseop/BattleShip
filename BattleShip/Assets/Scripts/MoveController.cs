using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class MoveController : MonoBehaviour
{
    public Transform BoatAxis;
    public Transform CameraAxis;
    private BoatInput boatInput;
    public float Speed = 3f;
    Vector2 moveInput;
    Vector2 mouseInput;
    private Rigidbody rb;
    private Vector3 currentMoveForce;
    private bool isMoving = false;
    float MouseX;
    float MouseY;
    public float camSpeed = 1f;

    void Awake()
    {
        boatInput = new BoatInput();
        rb = GetComponent<Rigidbody>();
        MouseY = 4;
    }
    void OnEnable()
    {
        boatInput.Boat.Move.performed += OnMove;
        boatInput.Boat.Move.canceled += OnMove;
        boatInput.Boat.Mouse.performed += OnMouse;
        boatInput.Boat.Mouse.canceled += OnMouse;
        boatInput.Boat.Enable();
    }
    void OnDisable()
    {
        boatInput.Boat.Move.performed -= OnMove;
        boatInput.Boat.Move.canceled -= OnMove;
        boatInput.Boat.Mouse.performed -= OnMouse;
        boatInput.Boat.Mouse.canceled -= OnMouse;
        boatInput.Boat.Disable();
    }
    void Update()
    {
        HandleCamera();
    }
    void HandleCamera()
    {
        MouseX += mouseInput.x;
        MouseY += mouseInput.y * -1;
        //마우스 Y 상한 체크
        if (MouseY > 10)
            MouseY = 10;  //상한값저장
        //마우스 Y 하한 체크
        if(MouseY < 0)
            MouseY = 0; //하한 값으로 저장

        CameraAxis.rotation = Quaternion.Euler(new Vector3(
            CameraAxis.rotation.x + MouseY,
            CameraAxis.rotation.y + MouseX,
            0) * camSpeed);
    }
    void FixedUpdate()
    {
        HandleMove();
    }
    void HandleMove()
    {
        if (moveInput.sqrMagnitude > 0.01f)
        {
            // 카메라가 바라보는 방향 벡터 추출
            Vector3 forward = CameraAxis.forward;
            Vector3 right = CameraAxis.right;

            // 수평 이동을 위해 Y축 제거 및 정규화 (가장 중요)
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            // 입력값(WASD)과 카메라 방향을 조합하여 최종 이동 방향 결정
            // moveInput.y는 전진/후진(W/S), moveInput.x는 좌우(A/D)
            Vector3 moveDirection = (forward * moveInput.y + right * moveInput.x).normalized;
            // 3. 배의 몸체(BoatAxis)를 이동 방향으로 부드럽게 회전
            // Quaternion.Euler로 고정하는 대신 LookRotation과 Slerp를 사용합니다.
            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                // 0.1f 값을 조절하여 회전 속도를 변경하세요 (작을수록 부드러움)
                BoatAxis.rotation = Quaternion.Slerp(BoatAxis.rotation, targetRotation, 0.01f);
            }
            //  결정된 방향으로 힘 가하기
            rb.AddForce(moveDirection * Speed, ForceMode.Acceleration);
        }
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
    public void OnMouse(InputAction.CallbackContext context)
    {
        mouseInput = context.ReadValue<Vector2>();
    }
}
