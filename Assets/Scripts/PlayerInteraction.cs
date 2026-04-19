using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] float _dist = 3f;
    [SerializeField] LayerMask _layer;
    [SerializeField] Transform _holdPos;
    [SerializeField] TextMeshProUGUI _uiText;

    private Item _held, _looked;

    void Update()
    {
        if (_held == null)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, _dist, _layer))
            {
                _looked = hit.collider.GetComponent<Item>();
                if (_looked) _uiText.text = "[E] " + _looked.itemName;
            }
            else { _looked = null; _uiText.text = ""; }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_held == null && _looked != null) { _held = _looked; _held.PickUp(_holdPos); }
            else if (_held != null) { _held.Use(); _held = null; }
        }

        if (Input.GetKeyDown(KeyCode.Q) && _held != null) { _held.Drop(); _held = null; }
    }
}