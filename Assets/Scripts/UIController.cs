using System;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private GameObject goalImage;
    [SerializeField]
    private GameObject goalText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        goalImage.SetActive(false);
        goalText.SetActive(false);
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerEnterGoal += HandleGoalReached;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerEnterGoal -= HandleGoalReached;
    }

    private void HandleGoalReached()
    {
        goalImage.SetActive(true);
        Invoke(nameof(DeactivateImage), 1f);
    }

    private void DeactivateImage()
    {
        goalImage.SetActive(false);
    }
    
}
