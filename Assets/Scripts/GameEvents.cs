using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static event Action OnPlayerEnterGoal;

    public static void TriggerPlayerEnterGoal()
    {
        OnPlayerEnterGoal?.Invoke();
    }
}
