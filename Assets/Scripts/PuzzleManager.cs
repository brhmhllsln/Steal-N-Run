using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PuzzleManager : MonoBehaviour
{
    [Header("Bulmaca Ayarları")]
    [SerializeField] private int[] _correctSequence;
    [SerializeField] private PressurePlate[] _allPlates;
    
    [Header("Kapı Ayarları")]
    [SerializeField] private GameObject _leftDoor;
    [SerializeField] private GameObject _rightDoor;
    [SerializeField] private float _openAngle = 90f;
    [SerializeField] private float _openSpeed = 2f;  
    [SerializeField] private float _closeSpeed = 5f; 

    private List<int> _currentInput = new List<int>();
    private bool _isSolved = false;
    private bool _shouldOpen = false;
    private bool _shouldClose = false;

        private Quaternion _leftDoorTargetRot;
    private Quaternion _rightDoorTargetRot;

    void Start()
    {
      
        _leftDoorTargetRot = _leftDoor.transform.localRotation;
        _rightDoorTargetRot = _rightDoor.transform.localRotation;
    }

    void Update()
    {
       
        if (_shouldOpen)
        {
            MoveDoors(_openSpeed);
        }
        else if (_shouldClose)
        {
            MoveDoors(_closeSpeed);
        }
    }

    void MoveDoors(float speed)
    {
        _leftDoor.transform.localRotation = Quaternion.Slerp(
            _leftDoor.transform.localRotation, _leftDoorTargetRot, Time.deltaTime * speed);

        _rightDoor.transform.localRotation = Quaternion.Slerp(
            _rightDoor.transform.localRotation, _rightDoorTargetRot, Time.deltaTime * speed);
    }

    public void PlatePressed(int index)
    {
        if (_isSolved) return;
        _currentInput.Add(index);

        if (_currentInput[_currentInput.Count - 1] != _correctSequence[_currentInput.Count - 1])
        {
            StartCoroutine(ResetPuzzleRoutine());
        }
        else if (_currentInput.Count == _correctSequence.Length)
        {
            _isSolved = true;
            TriggerOpen();
        }
    }

    void TriggerOpen()
    {
       
        _leftDoorTargetRot = Quaternion.Euler(0, -_openAngle, 0);
        _rightDoorTargetRot = Quaternion.Euler(0, _openAngle, 0);
        _shouldOpen = true;
    }

    public void CloseAndLockDoors()
    {
       
        _leftDoorTargetRot = Quaternion.Euler(0, 0, 0);
        _rightDoorTargetRot = Quaternion.Euler(0, 0, 0);
        
        _shouldOpen = false;
        _shouldClose = true;
        
       
        _isSolved = true;
    }

    
    IEnumerator ResetPuzzleRoutine()
    {
        foreach (var plate in _allPlates) plate.ShowError();
        yield return new WaitForSeconds(1f);
        foreach (var plate in _allPlates) plate.ResetPlate();
        _currentInput.Clear();
    }
}