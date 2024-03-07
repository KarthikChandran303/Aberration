using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    private Rigidbody body;
    
    [SerializeField, Range(0f, 100f)]
    float maxSpeed = 10f;
    
    [SerializeField, Range(0f, 100f)]
    float maxAcceleration = 10f;
    
    Vector3 velocity = Vector3.zero;
    Vector2 playerInput;
    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);
        // Bitwise OR (|) compares each bit of the two operands and produces a result where each bit of the output is set to 1 if at least one of the corresponding bits of the input operands is 1.
        //playerJump |= Input.GetButtonDown("Jump");
    }
}
