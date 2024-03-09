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

    private Collider bodyCol;
    
    private MaterialPropertyBlock mpb;

    static readonly int shPropColor = Shader.PropertyToID("_BaseColor");

    private bool colliderEnabled = true;
    public MaterialPropertyBlock Mpb
    {
        get
        {
            if (mpb == null)
            {
                mpb = new MaterialPropertyBlock();
            }
            return mpb;
        }
    }

    public GameObject paddle;

    private MeshRenderer meshRenderer;
    // Start is called before the first frame update
    void Awake()
    {
        body = GetComponent<Rigidbody>();
        bodyCol = paddle.GetComponent<Collider>();
        meshRenderer = paddle.GetComponent<MeshRenderer>();
        SpawnBall();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        bool isFalling = body.velocity.y < 0.0f;
    
        // Only update if there's a change in state
        if (isFalling != colliderEnabled)
        {
            colliderEnabled = isFalling;
            Physics.IgnoreLayerCollision(6, 7, !colliderEnabled);
            // Set the color based on the state
            Color color = isFalling ? Color.green : Color.red;
            Mpb.SetColor(shPropColor, color);
            meshRenderer.SetPropertyBlock(Mpb);
        }
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
    
    // This function is called after collison physics are applied. So a rigidbody's velocity may not turn out to be as expected.
    private void OnCollisionEnter(Collision col)
    {
        
        if (col.transform.CompareTag("Paddle"))
        {
            Vector3 newVel = Vector3.zero;
            if (paddle.GetComponent<Paddle>().isSpinning)
            {
                newVel = Vector3.up * (speed + 4.0f);
            }
            else
            {
                float platSize = (col.transform.localScale.x / 2.0f);
                float hitPos = (transform.position.x - col.transform.position.x);
                float ratio = (-hitPos / (platSize + 0.1f));
                float angle = ((ratio * 65) + 90);
                
                newVel.x = Mathf.Cos(Mathf.PI * angle / 180.0f);
                newVel.y = Mathf.Sin(Mathf.PI * angle / 180.0f);

                newVel = newVel.normalized * speed;
            }
            body.velocity = newVel;
        }

        if (col.transform.CompareTag("TopBox"))
        {
            body.velocity = body.velocity.normalized * speed;
        }
        
    }
}
