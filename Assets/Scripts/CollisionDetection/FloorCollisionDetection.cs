using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorCollisionDetection : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Ball")
        {
            GetComponent<AudioSource>().Play();
        } 
    }
}
