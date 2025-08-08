using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackboardCollisionDetection : MonoBehaviour
{
    [SerializeField] private ScoringSystem scoringSystem;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Ball")
        {
            scoringSystem.UpdateTouchBackboard();
        }
    }
}
