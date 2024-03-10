using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class AttackManager : MonoBehaviour
{
    [SerializeField] private GameObject laser;

    [SerializeField] private Attack[] attacks;
    
    [SerializeField] private bool phaseOne = true;

    private IObjectPool<GameObject> laserPool;

    private void Awake()
    {
        laserPool = new ObjectPool<GameObject>()
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AttackDelay());
    }
    
    IEnumerator AttackDelay()
    {
        while (phaseOne)
        {
            for (int i = 0; i < attacks.Length; i++)
            {
                Attack currentAttack = attacks[i];
                yield return new WaitForSeconds(currentAttack.delay);
                UnpackAttack(currentAttack.attackSettings);
                for (int j = 0; j < currentAttack.sameTimeAttacks.Length; j++)
                {
                    UnpackAttack(currentAttack.sameTimeAttacks[j]);
                }
            }
        }
    }

    private void UnpackAttack(Attack.AttackSettings attack)
    {
        if (attack.attackType == Attack.AttackSettings.AttackType.Laser)
        {
            float x = CameraBounds.BOTTOMLEFT.x + (CameraBounds.TOPRIGHT.x - CameraBounds.BOTTOMLEFT.x) * attack.spawnLocationX;
            float y = CameraBounds.BOTTOMLEFT.y + (CameraBounds.TOPRIGHT.y - CameraBounds.BOTTOMLEFT.y) * attack.spawnLocationY;
            Instantiate(laser, new Vector3(x, y, 30.0f), Quaternion.identity);
        }
        {
            
        }
    }

    [Serializable]
    public class Attack
    {
        public float delay = 0.0f;

        public AttackSettings attackSettings;
        
        public AttackSettings[] sameTimeAttacks;
        
        [Serializable]
        public class AttackSettings
        { 
            public enum AttackType
            {
                Laser,
                Random
            }

            public AttackType attackType;

            public enum SpawnLocationSetting
            {
                Manual,
                Random
            }

            public SpawnLocationSetting spawnLocationSetting;

            [Range(0,1)]
            public float spawnLocationX = 0.0f;
            [Range(0,1)]
            public float spawnLocationY = 0.5f;
        }
    }
}
