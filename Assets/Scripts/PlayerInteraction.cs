using UnityEngine;
using TMPro; 

public class PlayerInteraction : MonoBehaviour
{
    [Header("Ayarlar")]
    [SerializeField] float _interactDistance = 3f;
    [SerializeField] LayerMask _interactLayer; 
    [SerializeField] Transform _holdPos; 
    [SerializeField] TextMeshProUGUI _interactionText; 

    private Item _heldItem;
    private Item _lookedItem;

    void Update()
    {
      
        HandleRaycast();

        
        HandleInput();
    }

    void HandleRaycast()
    {
       
        if (_heldItem != null)
        {
            _interactionText.text = "";
            return;
        }

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _interactDistance, _interactLayer))
        {
            Item item = hit.collider.GetComponent<Item>();
            if (item != null)
            {
                _lookedItem = item;
                _interactionText.text = "[E] " + item.itemName + " Al";
            }
        }
        else
        {
            _lookedItem = null;
            _interactionText.text = "";
        }
    }

    void HandleInput()
    {
        // ALMA VEYA KULLANMA (E)
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_heldItem == null && _lookedItem != null)
            {
                PickUpItem();
            }
            else if (_heldItem != null)
            {
                _heldItem.Use();
                _heldItem = null; 
            }
        }

       
        if (Input.GetKeyDown(KeyCode.Q) && _heldItem != null)
        {
            _heldItem.Drop();
            _heldItem = null;
        }
    }

    void PickUpItem()
    {
        _heldItem = _lookedItem;
        _heldItem.PickUp(_holdPos);
        _lookedItem = null;
    }
}