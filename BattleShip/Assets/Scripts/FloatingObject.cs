using System.IO.Compression;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class FloatingObject : MonoBehaviour
{
    public MeshRenderer waterRenderer;
    [Header("Bouncy Setting")]
    public float bouncyForce = 15f;
    public float waterDrag = 2f;
    public float waterAngularDrag = 1f;
    private Material waterMaterial;
    private Rigidbody rb;
    private float cachedSpeed;
    private Vector2 cachedScale;
    private float waterBaseY;
    private float initialDrag;
    private float initialAngularDrag;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        initialDrag = rb.linearDamping;
        initialAngularDrag = rb.angularDamping;
        if(waterRenderer != null)
        {
            waterMaterial = waterRenderer.sharedMaterial;
            waterBaseY = waterRenderer.transform.position.y;
            cachedSpeed = waterMaterial.GetFloat("_waveSpeed");
            cachedScale = waterMaterial.GetVector("_waveSize");
        }
    }

    void FixedUpdate()
    {
        if(waterMaterial == null) return;

        float x = transform.position.x;
        float z = transform.position.z;
        float waveHeight = GetWaveHeight(x, z);
        float diff = waveHeight - transform.position.y;
        if(diff > 0)
        {
            rb.AddForce(Vector3.up * bouncyForce * diff, ForceMode.Acceleration);
            rb.linearDamping = waterDrag;
            rb.angularDamping = waterAngularDrag;
        }
        else
        {
            rb.linearDamping = initialDrag;
            rb.angularDamping = initialAngularDrag;
        }
        Debug.Log($"Wave: {waveHeight:F2} | Obj Y: {transform.position.y:F2} | Diff: {diff:F2}");
    }

    float GetWaveHeight(float x, float z)
    {
        float noiseValue = CalculateGradientNoise(
            x * cachedScale.x + Time.time * cachedSpeed,
            z * cachedScale.y + Time.time * cachedSpeed
        );
        return waterBaseY + noiseValue;
    }

    float CalculateGradientNoise(float u, float v)
    {
        return (Mathf.PerlinNoise(u,v) * 2f) - 1f;
    }
}
