using System;
using UnityEngine;

public class UIController : MonoBehaviour
{
    private GameObject _goalText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _goalText = GameObject.Find("GoalText");

        if (!_goalText)
        {
            Debug.LogError("No goal text found");
        }
        
        _goalText.SetActive(false);
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
        _goalText.SetActive(true);
    }
    
}
