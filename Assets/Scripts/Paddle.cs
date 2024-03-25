using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine.InputSystem;

public class Paddle : MonoBehaviour
{
    private Rigidbody body;
    
    float maxSpeed = 8f;
    
    [SerializeField, Range(0f, 100f)]
    float normalSpeed = 8f;
    
    [SerializeField, Range(0f, 100f)]
    float spinSpeed = 12f;

    [SerializeField] private bool useAcceleration = false;
    
    [SerializeField, Range(0f, 100f)]
    float maxAcceleration = 10f;
    
    Vector3 velocity = Vector3.zero;
    Vector2 playerInput;

    private TweenerCore<Quaternion, Vector3, QuaternionOptions> spinTween;
    private Sequence seq;

    private bool spin = false;

    public bool isSpinning = false;
    
    private Vector2 axis; 
    public float Snappiness = 3.0f;

    private Transform parent;
    private Vector3 lastUpdatePos;
    // Start is called before the first frame update
    void Awake()
    {
        maxSpeed = normalSpeed;
        body = GetComponent<Rigidbody>();
        parent = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        playerInput.x = Input.GetAxisRaw("Horizontal");
        playerInput.y = Input.GetAxisRaw("Vertical");
        
        // remap input (smoothing)
        axis.x = Mathf.Lerp( axis.x, playerInput.x, Snappiness * Time.deltaTime);
        axis.y = Mathf.Lerp( axis.y, playerInput.y, Snappiness * Time.deltaTime);
        
        playerInput = Vector2.ClampMagnitude(axis, 1f);
        spin |= (Input.GetKeyDown(KeyCode.L) || Gamepad.current.buttonSouth.wasPressedThisFrame);

        lastUpdatePos = transform.position;
        parent.position = lastUpdatePos;
        //transform.position += new Vector3(playerInput.x, playerInput.y, 0) * (Time.deltaTime * maxSpeed);
        // Bitwise OR (|) compares each bit of the two operands and produces a result where each bit of the output is set to 1 if at least one of the corresponding bits of the input operands is 1.
        //playerJump |= Input.GetButtonDown("Jump");    
    }

    private void LateUpdate()
    {
        transform.position = lastUpdatePos;
    }

    private void FixedUpdate()
    {
        velocity = body.velocity;
        Move(playerInput);
        if (spin)
        {
            Spin();
        }

        if (seq != null)
        {
            isSpinning = seq.active;
        }

        if (!isSpinning)
        {
            maxSpeed = normalSpeed;
            RotateWithMovement();
        }
        else
        {
            maxSpeed = spinSpeed;
        }

        body.velocity = velocity;
    }

    void RotateWithMovement()
    {
        if (Math.Abs(playerInput.x) < 0.3f)
        {
            body.DORotate(new Vector3(0, 0, 0), 0.175f, RotateMode.Fast);
        }
        else
        {
            body.DORotate(new Vector3(0, 0, -7.5f * Mathf.Sign(body.velocity.x)), 0.175f, RotateMode.Fast);
        }
        
    }

    void Spin()
    {
        if (seq == null || !seq.active)
        {
            seq = DOTween.Sequence().Append(body
                .DORotate(new Vector3(0, 0, -367.5f * Mathf.Sign(body.velocity.x)), 0.4f, RotateMode.FastBeyond360)
                .SetRelative(true)
                .SetEase(Ease.Linear));
            maxSpeed = spinSpeed;
            // seq.Append(body
            //     .DORotate(new Vector3(0, 0, 30 * Mathf.Sign(body.velocity.x)), 0.15f, RotateMode.FastBeyond360)
            //     .SetRelative(true)
            //     .SetEase(Ease.Linear));
            // seq.Append(body
            //     .DORotate(new Vector3(0, 0, -10 * Mathf.Sign(body.velocity.x)), 0.075f, RotateMode.FastBeyond360)
            //     .SetRelative(true)
            //     .SetEase(Ease.Linear));
        }
        spin = false;
    }

    void Move(Vector2 playerInput)
    {
        Vector3 desiredVelocity = new Vector3(playerInput.x, playerInput.y, 0) * maxSpeed;
        if (useAcceleration)
        {
            float maxSpeedChange = maxAcceleration * Time.deltaTime;
            velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
            velocity.y = Mathf.MoveTowards(velocity.y, desiredVelocity.y, maxSpeedChange);
        }
        else
        {
            velocity.x = desiredVelocity.x;
            velocity.y = desiredVelocity.y;
        }
    }
}
