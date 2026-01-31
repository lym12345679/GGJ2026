using System;
using System.Collections;
using System.Collections.Generic;
using Game.Level;
using UnityEngine;

public class Door : MonoBehaviour
{
    private bool hasTriggered = false;
    public int NextLevel;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasTriggered)
        {
            hasTriggered = true;
            LevelsManager.Instance.NextLevel(NextLevel);
        }
    }
}
