using UnityEngine;
using System.Collections;
using TMPro; // TextMeshPro kullandığımız için bunu ekledik

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    [SerializeField] float _walkSpeed = 7.0f;
    [SerializeField] float _runSpeed = 11.0f;
    [SerializeField] float _jumpSpeed = 8.0f;
    [SerializeField] float _gravity = 25.0f;
    [Range(0, 1)] [SerializeField] float _airControl = 0.2f;

    [Header("Bakış Ayarları")]
    [SerializeField] Transform _cameraTransform;
    [SerializeField] float _sensitivity = 2f;
    [SerializeField] float _lookLimit = 85f;

    [Header("Kafa Sallanma (HeadBob)")]
    [SerializeField] float _bobFrequency = 10f;  
    [SerializeField] float _bobHorizontalAmount = 0.1f; 
    [SerializeField] float _bobVerticalAmount = 0.04f;   
    
    [Header("Buff Ayarları")]
    [SerializeField] float _speedBuffMultiplier = 1.8f;
    [SerializeField] float _jumpBuffMultiplier = 1.5f;
    [SerializeField] float _dashForce = 50f; 

    [Header("UI Referansları")]
    [SerializeField] private TextMeshProUGUI _infoText; // Ekranın altında çıkacak yazı objesi

    private float _defaultYPos;
    private float _timer;
    private CharacterController _controller;
    private Vector3 _velocity;
    private float _rotationX;
    private float _jumpBufferCounter;
    private float _originalWalkSpeed, _originalRunSpeed, _originalJumpSpeed;

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        if (_cameraTransform == null) _cameraTransform = Camera.main.transform;
        _defaultYPos = _cameraTransform.localPosition.y;
        
        _originalWalkSpeed = _walkSpeed;
        _originalRunSpeed = _runSpeed;
        _originalJumpSpeed = _jumpSpeed;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (_infoText != null) _infoText.text = ""; // Başlangıçta yazıyı temizle
    }

    void Update()
    {
        HandleRotation();
        HandleMovement();
    }

    void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * _sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * _sensitivity;
        transform.Rotate(Vector3.up * mouseX);
        _rotationX -= mouseY;
        _rotationX = Mathf.Clamp(_rotationX, -_lookLimit, _lookLimit);
        _cameraTransform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
    }

    void HandleMovement()
    {
        if (Input.GetButtonDown("Jump")) _jumpBufferCounter = 0.2f;
        else _jumpBufferCounter -= Time.deltaTime;

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isRunning ? _runSpeed : _walkSpeed;
        Vector3 move = (transform.forward * Input.GetAxisRaw("Vertical") + transform.right * Input.GetAxisRaw("Horizontal")).normalized;

        if (_controller.isGrounded)
        {
            // Lerp kullanarak Dash kuvvetinin aniden sıfırlanmasını engelliyoruz
            _velocity.x = Mathf.Lerp(_velocity.x, move.x * currentSpeed, Time.deltaTime * 10f);
            _velocity.z = Mathf.Lerp(_velocity.z, move.z * currentSpeed, Time.deltaTime * 10f);

            if (_velocity.y < 0) _velocity.y = -2f;
            if (_jumpBufferCounter > 0) { _velocity.y = _jumpSpeed; _jumpBufferCounter = 0; }
            
            ApplyHeadBob(isRunning, currentSpeed);
        }
        else
        {
            _velocity.x = Mathf.Lerp(_velocity.x, move.x * currentSpeed, _airControl * Time.deltaTime * 10f);
            _velocity.z = Mathf.Lerp(_velocity.z, move.z * currentSpeed, _airControl * Time.deltaTime * 10f);
        }

        _velocity.y -= _gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }

    void ApplyHeadBob(bool isRunning, float speed)
    {
        if (Mathf.Abs(_velocity.x) > 0.1f || Mathf.Abs(_velocity.z) > 0.1f)
        {
            _timer += Time.deltaTime * speed * 0.5f;
            _cameraTransform.localPosition = new Vector3(
                Mathf.Cos(_timer * _bobFrequency / 2f) * _bobHorizontalAmount,
                _defaultYPos + Mathf.Sin(_timer * _bobFrequency) * _bobVerticalAmount, 0);
        }
        else
        {
            _timer = 0;
            _cameraTransform.localPosition = Vector3.Lerp(_cameraTransform.localPosition, new Vector3(0, _defaultYPos, 0), Time.deltaTime * 10f);
        }
    }

    public void ApplyBuff(Item.ItemType type)
    {
        switch (type)
        {
            case Item.ItemType.Misir: 
                StartCoroutine(SpeedBuff(10f)); 
                break;
            case Item.ItemType.MaatTuyu: 
                StartCoroutine(JumpBuff(5f)); 
                break;
            case Item.ItemType.AnubisPencesi: 
                Dash(); 
                break;
            case Item.ItemType.BilgelikTableti: 
                // Konsol yerine ekranda yazı gösteriyoruz
                ShowInfo("Bilgelik Tableti: iyice düşün kanka benden demesi...", 5f); 
                break;
        }
    }

    // Yazıyı ekranda belirli süre gösteren Coroutine
    public void ShowInfo(string message, float duration)
    {
        if (_infoText == null) return;
        StopCoroutine("InfoTextRoutine"); // Eski yazı varsa durdur
        StartCoroutine(InfoTextRoutine(message, duration));
    }

    IEnumerator InfoTextRoutine(string message, float duration)
    {
        _infoText.text = message;
        yield return new WaitForSeconds(duration);
        _infoText.text = "";
    }

    IEnumerator SpeedBuff(float d) { _walkSpeed *= _speedBuffMultiplier; _runSpeed *= _speedBuffMultiplier; yield return new WaitForSeconds(d); _walkSpeed = _originalWalkSpeed; _runSpeed = _originalRunSpeed; }
    IEnumerator JumpBuff(float d) { _jumpSpeed *= _jumpBuffMultiplier; yield return new WaitForSeconds(d); _jumpSpeed = _originalJumpSpeed; }
    
    void Dash() 
    { 
        Vector3 dashDir = transform.forward;
        _velocity.x = dashDir.x * _dashForce;
        _velocity.z = dashDir.z * _dashForce;
        _velocity.y = 2f; 
        Debug.Log("Dash! Güç: " + _dashForce);
    }
}