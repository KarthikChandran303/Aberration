using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;
using Sequence = DG.Tweening.Sequence;

public class Paddle : MonoBehaviour
{
    [SerializeField] private GameObject test;
    
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
    public Sequence spinSequence;
    public Sequence testSeqSpin;

    private bool spin = false;

    private bool space = false;
    
    public bool isSpinning = false;
    
    private Vector2 axis; 
    public float Snappiness = 3.0f;

    private bool gamePadSpinButton = false;

    private float collisionVelocityRatio = 1.0f;

    private float direction = 1.0f;

    public enum States
    {
        Idle,
        MovingRight,
        MovingLeft,
        Spinning,
        Reflecting,
        Completed
    };

    public States CurrentState;
    
    private bool transitionState = false;

    public Sequence StateSequence;
    void Awake()
    {
        maxSpeed = normalSpeed;
        body = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        CurrentState = States.Idle;
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

        if (Gamepad.current != null)
        {
            gamePadSpinButton = Gamepad.current.buttonSouth.wasPressedThisFrame;
        }

        // | checks if spin has been consumed
        spin |= (Input.GetKeyDown(KeyCode.L) || gamePadSpinButton);

        space |= (Input.GetKeyDown(KeyCode.Space));
        //parent.position = lastUpdatePos;
        //transform.position += new Vector3(playerInput.x, playerInput.y, 0) * (Time.deltaTime * maxSpeed);
        // Bitwise OR (|) compares each bit of the two operands and produces a result where each bit of the output is set to 1 if at least one of the corresponding bits of the input operands is 1.
        //playerJump |= Input.GetButtonDown("Jump");    
    }

    private void FixedUpdate()
    {
        velocity = body.velocity;
        Move(playerInput);
        
        if (spin)
        {
            Spin();
        }
        
        if (CurrentState != States.Spinning)
        {
            maxSpeed = normalSpeed;
            RotateWithMovement();
        }
        else
        {
            maxSpeed = spinSpeed;
        }

        isSpinning = (CurrentState == States.Spinning);
        
        if (transitionState)
        {
            StateAnimator();
        }

        float finalVelocityX = Mathf.Lerp(body.velocity.x, velocity.x, collisionVelocityRatio);
        body.velocity = new Vector3(finalVelocityX, velocity.y, velocity.z);
        //body.velocity = body.velocity;
    }

    void StateAnimator()
    {
        StateSequence.Kill();
        StateSequence = SelectAnimation(CurrentState);

        transitionState = false;
    }

    Sequence SelectAnimation(States SelectState)
    {
        switch (SelectState)
        {
            case States.Idle:
                return DOTween.Sequence().Append(body.DORotate(new Vector3(0, 0, 0), 0.175f, RotateMode.Fast));
            case States.MovingRight:
                return DOTween.Sequence().Append(body.DORotate(new Vector3(0, 0, -7.5f), 0.175f, RotateMode.Fast));
            case States.MovingLeft:
                return DOTween.Sequence().Append(body.DORotate(new Vector3(0, 0, 7.5f), 0.175f, RotateMode.Fast));
            case States.Spinning:
                spinSequence = DOTween.Sequence().Append(body
                    .DORotate(new Vector3(0, 0, -(367.5f) * direction),
                        GameManager.instance.RotationAnimationDuration, RotateMode.FastBeyond360)
                    .SetRelative(true)
                    .SetEase(Ease.Linear));
                spinSequence.onComplete = () =>
                {
                    Debug.Log("SpinComp");
                    spin = false;
                    SetState(States.Completed);
                };
                spinSequence.onRewind = () =>
                {
                    Debug.Log("SpinComp");
                    spin = false;
                    SetState(States.Completed);
                };
                return spinSequence;
            case States.Reflecting:
                var reflect = DOTween.Sequence().Append(body
                    .DORotate(new Vector3(0, 0, -(367.5f) * direction * -1),
                        GameManager.instance.RotationAnimationDuration, RotateMode.FastBeyond360)
                    .SetRelative(true)
                    .SetEase(Ease.Linear));
                reflect.onComplete = () =>
                {
                    Debug.Log("SpinComp");
                    spin = false;
                    SetState(States.Completed);
                };
                return reflect;
            default:
                return null;
        }
    }
    void SetState(States newState)
    {
        CurrentState = newState;
        transitionState = true;
    }

    void RotateWithMovement()
    {
        if (Math.Abs(playerInput.x) < 0.3f)
        {
            if (CurrentState != States.Idle)
            {
                SetState(States.Idle);
            }
        }
        else if( Mathf.Sign(body.velocity.x) > 0.0f )
        {
            if (CurrentState != States.MovingRight)
            {
                SetState(States.MovingRight);
                direction = 1.0f;
            }
        }
        else
        {
            if (CurrentState != States.MovingLeft)
            {
                SetState(States.MovingLeft);
                direction = -1.0f;
            }
        }
    }

    void Spin()
    {
        if (CurrentState != States.Spinning)
        {
            SetState(States.Spinning);
        }
        maxSpeed = spinSpeed;
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("LeftBox") && direction < 0)
        {
            DOTween.Sequence(DOTween.To(() =>
                {
                    return collisionVelocityRatio = 0.0f;
                }, x => collisionVelocityRatio = x, 1.0f, GameManager.instance.CollisionFeedbackDuration).onComplete =
                () => collisionVelocityRatio = 1.0f);
            //test.onUpdate = () => { Debug.Log(collisionVelocityRatio); };
            float ratio = Mathf.InverseLerp(0.0f, maxSpeed, body.velocity.magnitude);
            body.AddForce(new Vector3(GameManager.instance.CollisionChangeVelocity, 0.0f, 0.0f), ForceMode.VelocityChange);
        }
        else if (collision.transform.CompareTag("RightBox") && direction > 0)
        {
            DOTween.Sequence(DOTween.To(() =>
                {
                    return collisionVelocityRatio = 0.0f;
                }, x => collisionVelocityRatio = x, 1.0f, GameManager.instance.CollisionFeedbackDuration).onComplete =
                () => collisionVelocityRatio = 1.0f);
            //test.onUpdate = () => { Debug.Log(collisionVelocityRatio); };
            float ratio = Mathf.InverseLerp(0.0f, maxSpeed, body.velocity.magnitude);
            body.AddForce(new Vector3(-GameManager.instance.CollisionChangeVelocity, 0.0f, 0.0f), ForceMode.VelocityChange);
        }
    }
}
