using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    
    [SerializeField] bool USE_MANUAL_BOUNDS;

    [SerializeField] private float WIDTH;

    [SerializeField] private float HEIGHT;
    
    [SerializeField] private GameObject Bar;

    [Header("Player Settings")] public float RotationAnimationDuration = 0.4f;   
    
    [Header("Hit Stop Settings")]
    public float BallPaddleCollision = 0.075f;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (USE_MANUAL_BOUNDS)
        {
            SetGenBounds(10.0f, WIDTH, HEIGHT);
        }
        else
        {
            SetGenBounds(10.0f, Camera.main.pixelWidth, Camera.main.pixelHeight);
        }
    }
    
    public void SetGenBounds(float depth, float width, float height)
    {
        CameraBounds.updateBoundsManual(depth, width, height);
        GenerateBox();
    }
    
    private void GenerateBox()
    {
        float height = CameraBounds.TOPRIGHT.y - CameraBounds.BOTTOMLEFT.y;
        float width = CameraBounds.TOPRIGHT.x - CameraBounds.BOTTOMLEFT.x;
        
        GameObject right = Instantiate(Bar, new Vector3(CameraBounds.TOPRIGHT.x + Bar.transform.localScale.x/2.0f, CameraBounds.TOPRIGHT.y - height/2.0f, CameraBounds.TOPRIGHT.z), Quaternion.identity);
        right.transform.localScale = new Vector3(right.transform.localScale.x, height, right.transform.lossyScale.z);
        right.tag = "RightBox";
        
        GameObject left = Instantiate(Bar, new Vector3(CameraBounds.BOTTOMLEFT.x - Bar.transform.localScale.x/2.0f, CameraBounds.BOTTOMLEFT.y + height/2.0f, CameraBounds.BOTTOMLEFT.z), Quaternion.identity);
        left.transform.localScale = new Vector3(left.transform.localScale.x, height, left.transform.lossyScale.z);
        left.tag = "LeftBox";
        
        GameObject bottom = Instantiate(Bar, new Vector3(CameraBounds.BOTTOMLEFT.x + width/2.0f, CameraBounds.BOTTOMLEFT.y - Bar.transform.localScale.y/2.0f, CameraBounds.BOTTOMLEFT.z), Quaternion.identity);
        bottom.transform.localScale = new Vector3(width, bottom.transform.lossyScale.y, bottom.transform.lossyScale.z);
        bottom.tag = "BottomBox";
        
        GameObject top = Instantiate(Bar, new Vector3(CameraBounds.TOPRIGHT.x - width/2.0f, CameraBounds.TOPRIGHT.y + Bar.transform.localScale.y/2.0f, CameraBounds.TOPRIGHT.z), Quaternion.identity);
        top.transform.localScale = new Vector3(width, top.transform.lossyScale.y, top.transform.lossyScale.z);
        top.tag = "TopBox";
    }
    
    
}