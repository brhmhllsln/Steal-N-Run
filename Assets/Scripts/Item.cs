using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemName = "Esya";
    private Rigidbody _rb;
    private Collider _col;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<Collider>();
    }

    public void PickUp(Transform holdPos)
    {
        
        _rb.isKinematic = true;
        _col.enabled = false;

      
        transform.SetParent(holdPos);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Drop()
    {
       
        transform.SetParent(null);
        _rb.isKinematic = false;
        _col.enabled = true;

       
        _rb.AddForce(transform.parent != null ? transform.parent.forward * 5f : Vector3.up, ForceMode.Impulse);
    }

    public void Use()
    {
        Debug.Log(itemName + " kullanıldı!");
        Destroy(gameObject);
    }
}