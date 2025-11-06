using System.Collections;
using UnityEngine;
using Random = System.Random;

public class WallSpawn : MonoBehaviour
{
    [SerializeField] private GameObject wallSegment;

    private bool _isFalling;

    private float _initialSpeed;
    private float _fallAcc;

    private float _yToStop;
    private Material _wallMaterial;
    
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    private Coroutine _glowCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!wallSegment)
        {
            Debug.LogError("WallSegment needs to be set");
        }
        
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
        
        Renderer r =  wallSegment.GetComponent<Renderer>();
        if (r)
        {
            _wallMaterial = r.material;
            _wallMaterial.EnableKeyword("_EMISSION");
            _wallMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            
            StartGlow();
        }
    }

    private void StartGlow()
    {
        if (_glowCoroutine != null)
        {
            StopCoroutine(_glowCoroutine);
        }
        _glowCoroutine = StartCoroutine(GlowCoroutine());
    }

    private IEnumerator GlowCoroutine()
    {
        Color glowColor = Color.aquamarine;
        float glowIntensity = 2f;
        float duration = 2.5f;
        
        _wallMaterial.SetColor(EmissionColor, glowColor * glowIntensity);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float intensity = Mathf.Lerp(glowIntensity, 0f, t);
            
            _wallMaterial.SetColor(EmissionColor, glowColor * glowIntensity);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        _wallMaterial.SetColor(EmissionColor, Color.black);
    }
}