using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private int _plateIndex;

    [Header("Görsel Ayarlar")]
    [SerializeField] private float _sinkAmount = 20f; 
    [SerializeField] private float _sinkSpeed = 10f;   
    [SerializeField] private Color _activeColor = Color.white; 
    [SerializeField] private Color _errorColor = Color.red;   

    private MeshRenderer _renderer;
    private Color _initialColor;
    private Vector3 _startPos;
    private Vector3 _targetPos;
    private bool _isActivated = false;
    private PuzzleManager _manager;

    void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        _initialColor = _renderer.material.color;
        
       
        _startPos = transform.localPosition;
        _targetPos = _startPos;
        
        _manager = FindAnyObjectByType<PuzzleManager>();
    }

    void Update()
    {
       
        transform.localPosition = Vector3.Lerp(transform.localPosition, _targetPos, Time.deltaTime * _sinkSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
      
        if (other.CompareTag("Player") && !_isActivated)
        {
            _isActivated = true;
            
           
            _renderer.material.color = _activeColor;
            
            
            _targetPos = _startPos + new Vector3(0, -_sinkAmount, 0);
            
          
            if (_manager != null)
            {
                _manager.PlatePressed(_plateIndex);
            }
        }
    }

      public void ResetPlate()
    {
        _isActivated = false;
        _renderer.material.color = _initialColor;
        
       
        _targetPos = _startPos;
    }

  
    public void ShowError()
    {
        _renderer.material.color = _errorColor;
    }
}