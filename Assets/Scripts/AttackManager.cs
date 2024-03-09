using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    [SerializeField] private GameObject laser;

    [SerializeField] private Attack[] attacks;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AttackDelay());
    }
    
    IEnumerator AttackDelay()
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

    private void UnpackAttack(Attack.AttackSettings attack)
    {
        Debug.Log(attack.attackType.ToString());
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

            public Vector3 spawnLocation;
        }
    }
}
