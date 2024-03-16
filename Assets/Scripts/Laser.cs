using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Laser : MonoBehaviour
{
    public float speed = 30.0f;
    
    [SerializeField]
    private GameObject paddlePiece;
    
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
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Paddle"))
        {
            // get collision point
            Vector3 hitPoint = other.ClosestPointOnBounds(transform.position);
            
            // random rotation
            Quaternion rot = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));

            GameObject piece = Instantiate(paddlePiece, hitPoint, rot);
            Vector3 dir = (Camera.main.transform.position - hitPoint).normalized;
            dir.y = 0.80f;
            dir = dir * 8.0f;
            piece.GetComponent<Rigidbody>().AddForce(dir, ForceMode.Impulse);
            piece.GetComponent<Rigidbody>().AddTorque(Random.insideUnitSphere * 20.0f, ForceMode.Impulse);
            
            //GameObject paddleParent = other.transform.parent.gameObject;
            // move paddle parent to bounds of child
            //paddleParent.transform.position = new Vector3(hitPoint.x, paddleParent.transform.position.y, hitPoint.z);
        }
    }
}
