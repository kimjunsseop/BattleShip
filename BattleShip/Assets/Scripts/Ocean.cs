using System.Runtime.InteropServices;
using UnityEngine;

public class Ocean : MonoBehaviour
{
    public float density = 1;
    public float drag = 1;
    public float angularDrag = 1f;
    public Collider coll {get; private set;}
    void Start()
    {
        coll = GetComponent<Collider>();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Boat boat))
        {
            // 배가 물 인지 구현
            boat.EnterOcean(this);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent(out Boat boat))
        {
            // 배가 물 밖 인지 구현
            boat.ExitOcean();
        }
    }
}
