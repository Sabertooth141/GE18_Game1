using System;
using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    public float speedX = 0.001f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(speedX, 0, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject);
        if (other.gameObject.tag != "Player")
        {
            speedX *= -1;
        }
    }
}
