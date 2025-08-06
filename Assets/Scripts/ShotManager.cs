using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShotType
{
    Clean,
    Backboard,
    Rim,
    Miss,
    BackboardMiss
}

public class ShotManager : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform ballPosition;
    [SerializeField] private Transform basketTransform;
    [SerializeField] private Transform backboardTransform;
    [SerializeField] private float arcHeight = 2.5f;
    [SerializeField] private float launchAngle = 45f; // ⬅️ This replaces arcHeight
    [SerializeField] private ShotPositionManager shotPosManager;

    private GameObject currentBall;
    private Rigidbody ballRb;
    private bool shotInProgress = false;
    private float maxForce = 8f;

    public void Shoot(float _normalizedPower, ShotType _shotType)
    {
        Debug.Log("enter shoot");
        if (shotInProgress) return;

        Vector3 _targetPos = new Vector3();

        switch (_shotType)
        {
            case ShotType.Clean:
                _targetPos = basketTransform.position;
                break;
            case ShotType.Backboard:
                Vector3 rimPos = basketTransform.position;
                Vector3 toRim = rimPos - currentBall.transform.position;

                Vector3 backboardNormal = -basketTransform.forward;

                Vector3 reflected = Vector3.Reflect(toRim.normalized, backboardNormal);

                // Adjust how far back you aim
                float depthBehindRim = -0.5f; // decrease for side shots
                float verticalOffset = 0.25f;

                // Tweak lateral offset based on angle
                Vector3 lateralOffset = Vector3.Cross(Vector3.up, backboardNormal).normalized;
                float sideOffset = Vector3.Dot(toRim.normalized, lateralOffset);
                lateralOffset *= sideOffset * 0.25f; // Scale the lateral shift

                Vector3 target = rimPos + reflected * depthBehindRim + lateralOffset;
                target.y = rimPos.y + verticalOffset;
                _targetPos = target;
                break;
            case ShotType.Rim:
                float rimRadius = 0.14f; // example rim radius in meters
                Vector3 rimOffset = new Vector3(
                    Random.Range(-rimRadius, rimRadius),
                    Random.Range(-rimRadius / 2, rimRadius / 2),
                    Random.Range(-rimRadius, rimRadius)
                );
                
                while((Mathf.Abs(rimOffset.x) + Mathf.Abs(rimOffset.y) + Mathf.Abs(rimOffset.z)) < 0.13 
                    || (Mathf.Abs(rimOffset.x) + Mathf.Abs(rimOffset.y) + Mathf.Abs(rimOffset.z)) > 0.24)
                {
                    rimOffset = new Vector3(
                    Random.Range(-rimRadius, rimRadius),
                    Random.Range(-rimRadius / 2, rimRadius / 2),
                    Random.Range(-rimRadius, rimRadius)
                );
                }

                _targetPos = basketTransform.position + rimOffset; // random rim offset
                break;
            case ShotType.BackboardMiss:
                //set target pos
                break;
            case ShotType.Miss:
                Vector3 shortOffset = new Vector3(
                Random.Range(-0.7f, 0.7f),
                Random.Range(-1.0f, 0),
                Random.Range(-0.7f, 0.7f));

                while(Mathf.Abs(shortOffset.x) < 0.4f || Mathf.Abs(shortOffset.y) < 0.4f 
                    || Mathf.Abs(shortOffset.z) < 0.4f || Mathf.Abs(shortOffset.x) == Mathf.Abs(shortOffset.y)
                    || Mathf.Abs(shortOffset.y) == Mathf.Abs(shortOffset.z) || Mathf.Abs(shortOffset.x) == Mathf.Abs(shortOffset.z))
                {
                    shortOffset = new Vector3(
                    Random.Range(-0.6f, 0.6f),
                    Random.Range(-1.0f, 0),
                    Random.Range(-0.6f, 0.6f));
                }

                _targetPos = basketTransform.position + shortOffset; // short or side miss
                break;
            default:
                break;
        }

        //Vector3 velocity = CalculateArcVelocity(currentBall.transform.position, basketTransform.position, arcHeight, Physics.gravity.magnitude);
        Vector3 velocity = CalculateVelocityFromAngle(currentBall.transform.position, _targetPos, launchAngle, Physics.gravity.magnitude);

        /*Debug.Log("starts shooting");
        float _force = _normalizedPower * maxForce;

        Debug.Log($"force is {_force}");*/
        ballRb = currentBall.GetComponent<Rigidbody>();

        //Vector3 _direction = (currentBall.transform.forward + currentBall.transform.up).normalized;
        /*Vector3 _toBasket = basketTransform.position - currentBall.transform.position;
        _toBasket.y += arcHeight; // Add some arc

        Vector3 _direction = _toBasket.normalized;

        Debug.Log($"direction is {_direction}");*/

        ballRb.isKinematic = false;
        ballRb.WakeUp();
        ballRb.velocity = velocity;
        shotInProgress = true;

        // Delay 1 frame to ensure isKinematic = false is applied
        //StartCoroutine(ApplyForceNextFrame(_direction * _force));
        //StartCoroutine(ApplyForceNextFrame(velocity));
    }

    public Vector3 CalculateVelocityFromAngle(Vector3 startPos, Vector3 targetPos, float launchAngleDegrees, float gravity)
    {
        Vector3 displacement = targetPos - startPos;

        // Separate horizontal and vertical displacement
        Vector3 displacementXZ = new Vector3(displacement.x, 0, displacement.z);
        float horizontalDistance = displacementXZ.magnitude;
        float verticalDistance = displacement.y;

        float angleRad = launchAngleDegrees * Mathf.Deg2Rad;
        float gravityAbs = Mathf.Abs(gravity); // usually gravity = -9.81f

        // Calculate initial speed (magnitude of velocity)
        // Using formula:
        // V = sqrt( g * d^2 / (2 * cos^2(angle) * (d * tan(angle) - h)) )
        float numerator = gravityAbs * horizontalDistance * horizontalDistance;
        float denominator = 2 * Mathf.Pow(Mathf.Cos(angleRad), 2) * (horizontalDistance * Mathf.Tan(angleRad) - verticalDistance);

        // Check denominator to avoid divide by zero or sqrt of negative
        if (denominator <= 0)
        {
            Debug.LogWarning("Impossible shot with given angle and target");
            return Vector3.zero;
        }

        float initialSpeed = Mathf.Sqrt(numerator / denominator);

        // Direction on horizontal plane
        Vector3 horizontalDirection = displacementXZ.normalized;

        // Calculate velocity components
        Vector3 velocity = horizontalDirection * initialSpeed * Mathf.Cos(angleRad);
        velocity.y = initialSpeed * Mathf.Sin(angleRad);

        return velocity;
    }

    public void SpawnBall(float _maxForce)
    {
        currentBall = Instantiate(ballPrefab, ballPosition.position, Quaternion.identity);
        maxForce = _maxForce;
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
        ballRb.velocity = force;
        //ballRb.AddForce(force, ForceMode.Impulse);
        Debug.Log($"Force applied: {force}");
        Debug.Log($"Velocity: {ballRb.velocity}");

        shotInProgress = true;

        //disable Inputs
    }
}
