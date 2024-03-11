using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Laser : MonoBehaviour
{
    public float speed = 30.0f;
    
    private IObjectPool<GameObject> laserPool;
    public IObjectPool<GameObject> LaserPool
    {
        set => laserPool = value;
    }

    private MeshRenderer mr;
    private Material mat;
    
    static readonly int shPropTime = Shader.PropertyToID("_StartTime");
    
    Vector3 velocity;

    void Awake()
    {
        mat = GetComponent<MeshRenderer>().material;
    }
    void OnEnable()
    {
        velocity = new Vector3(0, 0, -1);
        
        mat.SetFloat(shPropTime, Time.time);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += velocity * (speed * Time.deltaTime);
    }

    public void Deactivate(float delay)
    {
        StartCoroutine(DeactivateRoutine(delay));
    }
    
    private IEnumerator DeactivateRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        laserPool.Release(gameObject);
    }
}
