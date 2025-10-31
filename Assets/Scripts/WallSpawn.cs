using UnityEngine;
using Random = System.Random;

public class WallSpawn : MonoBehaviour
{
    private bool _isFalling;

    private float _initialSpeed;
    private float _fallAcc;
    private float _yToStop;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartFalling();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isFalling)
        {
            transform.position += new Vector3(0, (_initialSpeed) * Time.deltaTime, 0);
            _initialSpeed += _fallAcc;
            if (transform.position.y <= _yToStop)
            {
                transform.position = new Vector3(transform.position.x, _yToStop, transform.position.z);
                _isFalling = false;
            }
        }
    }

    private void StartFalling()
    {
        _isFalling = true;
    }

    public void Init(float inInitialSpeed, float inFallAcc, float inYToStop)
    {
        _initialSpeed = inInitialSpeed;
        _fallAcc = inFallAcc;
        _yToStop = inYToStop;
    }
}
