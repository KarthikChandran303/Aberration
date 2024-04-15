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
    // Start is called before the first frame update
    void Awake()
    {
        maxSpeed = normalSpeed;
        body = GetComponent<Rigidbody>();
        //var s = DOTween.Sequence(test.transform.GetComponent<Rigidbody>().DORotate(new Vector3(0.0f, 0.0f, 180.0f), 2.0f, RotateMode.Fast).SetLoops(-1).SetDelay(2.0f).SetUpdate(false));
        //s.Goto(1.0f);
        //parent = transform.parent;
    }

    private void Start()
    {
        testSeqSpin = DOTween.Sequence().Append(body.DORotate(new Vector3(0.0f, 0.0f, 280.0f), 1.0f));
        //testSeq.SetAutoKill(false);
        testSeqSpin.Pause();
        testSeqSpin.onComplete = () =>
        {
            space = false;
            testSeqSpin.Rewind();
        };
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("RX:" + transform.localRotation.x + "RY:" + transform.localRotation.y + "RZ:" + transform.localRotation.z);
        //Debug.Log(collisionVelocityRatio);
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

    // private void LateUpdate()
    // {
    //     transform.position = lastUpdatePos;
    // }

    private void FixedUpdate()
    {
        velocity = body.velocity;
        Move(playerInput);
        if (spin)
        {
            Spin();
        }

        if (spinSequence != null)
        {
            isSpinning = spinSequence.active;
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
        
        
        body.velocity = Vector3.Lerp(body.velocity, velocity, collisionVelocityRatio);
        //body.velocity = body.velocity;
    }

    void RotateWithMovement()
    {
        if (Math.Abs(playerInput.x) < 0.3f)
        {
            body.DORotate(new Vector3(0, 0, 0), 0.7f, RotateMode.Fast);
        }
        else
        {
            body.DORotate(new Vector3(0, 0, -7.5f * Mathf.Sign(body.velocity.x)), 0.175f, RotateMode.Fast);
        }
    }

    void Spin()
    {
        if (spinSequence == null || !spinSequence.active)
        {
            
            spinSequence = DOTween.Sequence().Append(body
                .DORotate(new Vector3(0, 0, -(367.5f) * Mathf.Sign(body.velocity.x)),
                    GameManager.instance.RotationAnimationDuration, RotateMode.FastBeyond360)
                .SetRelative(true)
                .SetEase(Ease.Linear));
            spinSequence.onComplete = () =>
            {
                spin = false;
                Debug.Log("DoneSpin");
            };
            //spinSequence.SetUpdate(UpdateType.Fixed, true);
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
    }
    
    void TestSpin()
    {
        
        if (!testSeqSpin.IsPlaying())
        {
            Debug.Log("hey");
            testSeqSpin.Play();
        }
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
        if (collision.transform.CompareTag("BottomBox"))
        {
            Debug.Log("poki");
            var test = DOTween.Sequence(DOTween.To(() =>
                {
                    return collisionVelocityRatio = 0.0f;
                }, x => collisionVelocityRatio = x, 1.0f, 1.5f).onComplete =
                () => collisionVelocityRatio = 1.0f);
            //test.onUpdate = () => { Debug.Log(collisionVelocityRatio); };
            float ratio = Mathf.InverseLerp(0.0f, maxSpeed, body.velocity.magnitude);
            //body.velocity = new Vector3(-collision.relativeVelocity.normalized.x, 1.0f, 0.0f).normalized * maxSpeed * 5.0f;
            //body.AddForce(new Vector3(0.0f, 15.0f, 0.0f), ForceMode.VelocityChange);
        }
    }
}
