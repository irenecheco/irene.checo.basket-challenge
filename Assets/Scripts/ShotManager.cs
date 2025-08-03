using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotManager : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform ballPosition;
    [SerializeField] private float maxForce = 200f;
    [SerializeField] private Transform basketTransform;
    [SerializeField] private float arcHeight = 2.5f;
    [SerializeField] private ShotPositionManager shotPosManager;

    private GameObject currentBall;
    private Rigidbody ballRb;
    private bool shotInProgress = false;

    public void Shoot(float _normalizedPower)
    {
        Debug.Log("enter shoot");
        if (shotInProgress) return;

        Debug.Log("starts shooting");
        float _force = _normalizedPower * maxForce;

        Debug.Log($"force is {_force}");
        ballRb = currentBall.GetComponent<Rigidbody>();

        //Vector3 _direction = (currentBall.transform.forward + currentBall.transform.up).normalized;
        Vector3 _toBasket = basketTransform.position - currentBall.transform.position;
        _toBasket.y += arcHeight; // Add some arc

        Vector3 _direction = _toBasket.normalized;

        Debug.Log($"direction is {_direction}");

        /*ballRb.isKinematic = false;
        ballRb.AddForce(_direction * _force, ForceMode.Impulse);
        Debug.Log($"Velocity after AddForce: {ballRb.velocity}, force should be {_direction * _force}");*/

        ballRb.isKinematic = false;
        ballRb.WakeUp();

        // Delay 1 frame to ensure isKinematic = false is applied
        StartCoroutine(ApplyForceNextFrame(_direction * _force));        
    }

    public void SpawnBall()
    {
        currentBall = Instantiate(ballPrefab, ballPosition.position, Quaternion.identity);
    }

    void Update()
    {
        if(shotInProgress && currentBall != null)
        {
            if(currentBall.transform.position.y < 0.2f)
            {
                Destroy(currentBall);
                currentBall = null;
                shotInProgress = false;

                shotPosManager.ChangePos();
            }
        }
    }

    private IEnumerator ApplyForceNextFrame(Vector3 force)
    {
        yield return null;
        ballRb.AddForce(force, ForceMode.Impulse);
        Debug.Log($"Force applied: {force}");
        Debug.Log($"Velocity: {ballRb.velocity}");

        shotInProgress = true;

        //disable Inputs
    }
}
