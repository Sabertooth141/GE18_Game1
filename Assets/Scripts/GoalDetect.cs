using System;
using UnityEngine;

public class GoalDetect : MonoBehaviour
{
    public ParticleSystem goalEffect;
    
    public AudioSource audioSource;
    
    [SerializeField]
    private AudioClip goalSound;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            goalEffect.Play();
            audioSource.Play();
            GameEvents.TriggerPlayerEnterGoal();
        }
    }
}