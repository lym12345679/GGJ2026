using System;
using System.Collections;
using System.Collections.Generic;
using Game.Level;
using UnityEngine;

public class ProtectGround : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LevelsManager.Instance.Restart();
        }
    }

    // private void OnCollisionEnter2D(Collision2D other)
    // {
    //     if (other.collider.CompareTag("Player"))
    //     {
    //         LevelsManager.Instance.Restart();
    //     }
    // }
}
