using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Ball : MonoBehaviour
{
    [SerializeField]
    private float speed = 10.0f;
    Vector3 velocity = Vector3.zero;

    private Rigidbody body;
    // Start is called before the first frame update
    void Awake()
    {
        body = GetComponent<Rigidbody>();
        SpawnBall();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
    }

    private void SpawnBall()
    {
        body.velocity = Vector3.zero;
        transform.position = new Vector3 (0.0f, 2.4f, 0.0f);
        StartCoroutine(StartDelay());
    }

    IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(1.0f);
        body.velocity = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(0.2f, 1.0f), 0.0f).normalized * speed;
    }
    
    private void OnCollisionEnter(Collision col)
    {
        Debug.Log(body.velocity);
        if (col.transform.CompareTag("Paddle") && body.velocity.y > 0)
        {
            float platSize = (col.transform.localScale.x / 2.0f);
            float hitPos = (transform.position.x - col.transform.position.x);
            float ratio = ( -hitPos / (platSize + 0.1f) );
            float angle = ((ratio * 65) + 90);
            
            Vector3 newVel = Vector3.zero;
            newVel.x = Mathf.Cos( Mathf.PI * angle / 180.0f );
            newVel.y = Mathf.Sin( Mathf.PI * angle / 180.0f );
            
            newVel = newVel.normalized * speed;
            // negate force
            //body.velocity = Vector3.zero;
            
            body.velocity = newVel;
        }
    }
}
