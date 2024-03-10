using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Laser : MonoBehaviour
{
    Vector3 velocity;
    // Start is called before the first frame update
    void OnEnable()
    {
        velocity = new Vector3(0, 0, -35.0f);
        // rotate 90 degrees
        transform.Rotate(0, 90, 0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += velocity * Time.deltaTime;
    }

    IEnumerator DeactivateRoutine()
    {
    }
}
