using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RimCollisionDetection : MonoBehaviour
{
    [SerializeField] private ScoringSystem scoringSystem;
    [SerializeField] private AiScoringSystem aiScoringSystem;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "Ball")
        {
            scoringSystem.UpdateTouchRim();
            GetComponent<AudioSource>().Play();
        }
        else if (collision.collider.tag == "AiBall")
        {
            aiScoringSystem.UpdateTouchRim();
        }
    }
}
