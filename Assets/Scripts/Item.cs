using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemType { Misir, MaatTuyu, AnubisPencesi, BilgelikTableti, Normal }
    public string itemName = "Esya";
    public ItemType type = ItemType.Normal;

    private Rigidbody _rb;
    private Collider _col;

    void Awake() { _rb = GetComponent<Rigidbody>(); _col = GetComponent<Collider>(); }

    public void PickUp(Transform holdPos)
    {
        _rb.isKinematic = true;
        _col.enabled = false;
        transform.SetParent(holdPos);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    public void Drop()
    {
        transform.SetParent(null);
        _rb.isKinematic = false;
        _col.enabled = true;
        _rb.AddForce(Camera.main.transform.forward * 5f, ForceMode.Impulse);
    }

    public void Use()
    {
        PlayerMovement pm = FindAnyObjectByType<PlayerMovement>();
        if (pm != null && type != ItemType.Normal) pm.ApplyBuff(type);
        Destroy(gameObject);
    }
}