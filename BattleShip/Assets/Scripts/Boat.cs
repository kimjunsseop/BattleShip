using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour
{
    public Rigidbody rb;
    public Collider coll;
    public float volume;
    public float customVolume = 0;
    public float waterDrag = 3f;
    public float waterAngularDrag = 1f;
    private float airDrag;
    private float airAngularDrag;
    public bool isOcean;
    public float dampeningFactor = 0.1f;
    public Ocean ocean;
    public bool simulateWaterTurbulence;
    public float turbulenceStrenght = 1;
    public float[] randTimeOffset = new float[6];
    private float time;
    public float floatStrength = 2;
    public List<Transform> floaters = new List<Transform>();
    void Start()
    {
        coll = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        airDrag = rb.linearDamping;
        airAngularDrag = rb.angularDamping;

        if(customVolume != 0)
        {
            volume = customVolume;
        }
        else
        {
            volume = CalculateVolume();
        }
        if(floaters == null || floaters.Count == 0)
        {
            CreateFloaters();
        }
    }
    public void Awake()
    {
        randTimeOffset = new float[6];
        for(int i = 0; i < 6; i++)
        {
            randTimeOffset[i] = Random.Range(0,6f);
        }
    }

    void FixedUpdate()
    {
        time += Time.fixedDeltaTime / 4;
        if(isOcean)
        {
            OceanUpdate();
        }
    }

    public void OceanUpdate()
    {
        foreach(Transform floater in floaters)
        {
            float difference = floater.position.y - ocean.coll.bounds.max.y;
            if(difference < 0)
            {
                Vector3 bouncy = (Vector3.up * floatStrength * Mathf.Abs(difference) * Physics.gravity.magnitude * volume * ocean.density);
                // 난류
                if (simulateWaterTurbulence)
                {
                    bouncy += GenerateTurbulence();
                }
                rb.AddForceAtPosition(bouncy, floater.position, ForceMode.Force);
                rb.AddForceAtPosition(rb.linearVelocity * (dampeningFactor / floaters.Count) * volume, floater.position, ForceMode.Force);
            }
        }
    }

    // 난류
    public Vector3 GenerateTurbulence()
    {
        Vector3 turbulence = new Vector3(Mathf.PerlinNoise(time + randTimeOffset[0], time + randTimeOffset[1]) * 2 - 1,
                                    0,
                                    Mathf.PerlinNoise(time + randTimeOffset[4], time + randTimeOffset[5]) * 2 - 1);

            return turbulence * turbulenceStrenght;
    }

    public void EnterOcean(Ocean entered)
    {
        ocean = entered;
        isOcean = true;
        waterDrag = ocean.drag;
        waterAngularDrag = ocean.angularDrag;
        rb.linearDamping = waterDrag;
        rb.angularDamping = waterAngularDrag;
    }

    public void ExitOcean()
    {
        isOcean = false;
        rb.linearDamping = airDrag;
        rb.angularDamping = airAngularDrag;
    }

    public float CalculateVolume()
    {
        return coll.bounds.size.x * coll.bounds.size.y * coll.bounds.size.z;
    }

    void CreateFloaters()
    {
        floaters = new List<Transform>();
        Vector3[] corners = DefineCorners();
        foreach (Vector3 corner in corners)
        {
            // 빈 오브젝트 생성 및 이름 설정
            GameObject floaterObj = new GameObject("AutoFloater");
            
            // 위치 설정 (월드 좌표 = 배 위치 + 회전된 로컬 좌표)
            // transform.TransformDirection을 써야 배가 회전해도 부표가 같이 회전합니다.
            floaterObj.transform.position = transform.position + transform.TransformDirection(corner);
                
            // 배의 자식으로 설정 (배가 움직일 때 같이 움직이게 함)
            floaterObj.transform.parent = this.transform;
                
            floaters.Add(floaterObj.transform);
        }
    }
    public Vector3[] DefineCorners()
    {
        // 기존과 동일하지만 로컬 좌표 기준으로 계산하도록 유도
        // bounds.extents는 이미 size / 2와 같습니다.
        Vector3 ext = coll.bounds.extents; 

        // 배가 뒤집히지 않게 하기 위해 Y값(높이)의 절반만 사용하거나 
        // 아예 바닥면에만 배치하고 싶다면 ext.y 대신 0이나 음수값을 넣을 수 있습니다.
        return new Vector3[] {
                new Vector3(ext.x, -ext.y, ext.z),
                new Vector3(-ext.x, -ext.y, ext.z),
                new Vector3(ext.x, -ext.y, -ext.z),
                new Vector3(-ext.x, -ext.y, -ext.z),
                new Vector3(ext.x, ext.y, ext.z),
                new Vector3(-ext.x, ext.y, ext.z),
                new Vector3(ext.x, ext.y, -ext.z),
                new Vector3(-ext.x, ext.y, -ext.z)
        };
    }
}
