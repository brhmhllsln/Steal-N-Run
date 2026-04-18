using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
   
    [Header("Hız Ayarları")]
    [SerializeField] float _walkSpeed = 7.0f;
    [SerializeField] float _runSpeed = 11.0f;
    [SerializeField] float _jumpSpeed = 8.0f;
    [SerializeField] float _gravity = 25.0f;
    [Range(0, 1)] 
    [SerializeField] float _airControl = 0.2f;


    [Header("Bakış Ayarları")]
    [SerializeField] Transform _cameraTransform;
    [SerializeField] float _sensitivity = 2f;
    [SerializeField] float _lookLimit = 85f;

    [Header("Kafa Sallanma (HeadBob)")]
   
    [SerializeField] float _bobFrequency = 10f;  
    [SerializeField] float _bobHorizontalAmount = 0.1f; 
    [SerializeField] float _bobVerticalAmount = 0.04f;   
    
    private float _defaultYPos;
    private float _timer;

    
    [Header("Akıcılık")]
    [SerializeField] float _jumpBufferTime = 0.2f;
    private float _jumpBufferCounter;

    private CharacterController _controller;
    private Vector3 _velocity;
    private float _rotationX;

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        if (_cameraTransform == null) _cameraTransform = Camera.main.transform; // Otomatik bulma
        _defaultYPos = _cameraTransform.localPosition.y;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleRotation();
        HandleJumpBuffer();
        HandleMovement();
        ApplyHeadBob();
    }
    
    // ... (HandleRotation, HandleJumpBuffer, HandleMovement fonksiyonları aynen kalıyor) ...
    void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * _sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * _sensitivity;

        transform.Rotate(Vector3.up * mouseX);
        _rotationX -= mouseY;
        _rotationX = Mathf.Clamp(_rotationX, -_lookLimit, _lookLimit);
        _cameraTransform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
    }

    void HandleJumpBuffer()
    {
        if (Input.GetButtonDown("Jump")) _jumpBufferCounter = _jumpBufferTime;
        else _jumpBufferCounter -= Time.deltaTime;
    }

    void HandleMovement()
{
    bool isRunning = Input.GetKey(KeyCode.LeftShift);
    float currentSpeed = isRunning ? _runSpeed : _walkSpeed;

    float h = Input.GetAxisRaw("Horizontal");
    float v = Input.GetAxisRaw("Vertical");

    // İstenen hareket yönü
    Vector3 targetMove = (transform.forward * v + transform.right * h).normalized;

    if (_controller.isGrounded)
    {
        // Yerdeyken tam hız ve kontrol
        _velocity.x = targetMove.x * currentSpeed;
        _velocity.z = targetMove.z * currentSpeed;

        if (_velocity.y < 0) _velocity.y = -2f;

        if (_jumpBufferCounter > 0)
        {
            _velocity.y = _jumpSpeed;
            _jumpBufferCounter = 0;
        }
    }
    else
    {
        // --- HAVADAYKEN KONTROL KISITLAMA ---
        // Mevcut yatay hızı koru ama oyuncunun girdisine göre küçük bir miktar ekle
        float airX = Mathf.Lerp(_velocity.x, targetMove.x * currentSpeed, _airControl * Time.deltaTime * 10f);
        float airZ = Mathf.Lerp(_velocity.z, targetMove.z * currentSpeed, _airControl * Time.deltaTime * 10f);
        
        _velocity.x = airX;
        _velocity.z = airZ;
    }

    // Yerçekimi her zaman uygulanır
    _velocity.y -= _gravity * Time.deltaTime;

    _controller.Move(_velocity * Time.deltaTime);
}

   
    void ApplyHeadBob()
    {
      
        if (!_controller.isGrounded) return;

        bool isMoving = Mathf.Abs(_velocity.x) > 0.1f || Mathf.Abs(_velocity.z) > 0.1f;

        if (isMoving)
        {
         
            _timer += Time.deltaTime * (Input.GetKey(KeyCode.LeftShift) ? _runSpeed : _walkSpeed) * 0.5f;

            Vector3 newPos = _cameraTransform.localPosition;

            newPos.x = Mathf.Cos(_timer * _bobFrequency / 2f) * _bobHorizontalAmount;

            newPos.y = _defaultYPos + Mathf.Sin(_timer * _bobFrequency) * _bobVerticalAmount;
   
            _cameraTransform.localPosition = newPos;
        }
        else
        {
           
            _timer = 0;
            Vector3 resetPos = _cameraTransform.localPosition;
            resetPos.x = Mathf.Lerp(resetPos.x, 0f, Time.deltaTime * 10f);
            resetPos.y = Mathf.Lerp(resetPos.y, _defaultYPos, Time.deltaTime * 10f);
            _cameraTransform.localPosition = resetPos;
        }
    }
}