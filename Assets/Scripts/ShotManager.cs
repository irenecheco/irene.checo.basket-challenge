using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShotType
{
    Clean,
    Backboard,
    Rim,
    Miss,
    BackboardMiss,
    LongMiss
}

public class ShotManager : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform ballPosition;
    [SerializeField] private Transform basketTransform;
    [SerializeField] private Transform backboardTransform;
    [SerializeField] private float arcHeight = 2.5f;
    [SerializeField] private float launchAngle = 45f; // ⬅️ This replaces arcHeight
    [SerializeField] private ScoringSystem scoringSystem;
    [SerializeField] private InputHandler inputHandler;

    private GameObject currentBall;
    private Rigidbody ballRb;
    private bool shotInProgress = false;
    private float maxForce = 8f;

    public void Shoot(ShotType _shotType)
    {
        if (shotInProgress) return;

        Vector3 _targetPos = new Vector3();

        switch (_shotType)
        {
            case ShotType.Clean:
                _targetPos = basketTransform.position;
                break;

            case ShotType.Backboard:
                _targetPos = CalculateBackboardPos();
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

                float xMiss = Random.value < 0.5f
                ? Random.Range(-0.5f, -0.3f)
                : Random.Range(0.3f, 0.5f); // either far left or far right

                float yMiss = Random.Range(0.2f, 0.4f); // always high

                Vector3 missOffset = new Vector3(xMiss, yMiss, 0);

                Debug.Log($"miss offset is {missOffset}");
                _targetPos = CalculateBackboardPos() + missOffset;
                break;

            case ShotType.Miss:
                Vector3 shortOffset = new Vector3(
                Random.Range(-0.5f, 0.5f),
                Random.Range(-1.2f, -0.6f),
                Random.Range(-1.0f, -0.6f));

                _targetPos = basketTransform.position + shortOffset; // short or side miss
                break;

            case ShotType.LongMiss:
                // offset to push shot too far
                Vector3 longMissOffset = new Vector3(
                    Random.Range(-0.3f, 0.3f),     // small horizontal variation
                    Random.Range(0.3f, 0.7f),      // definitely high
                    Random.Range(0.5f, 1f)       // definitely deep (goes past board)
                );

                // final target position
                _targetPos = basketTransform.position + longMissOffset;
                break;

            default:
                _targetPos = basketTransform.position;
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

    public Vector3 CalculateBackboardPos()
    {
        Vector3 _rimPos = basketTransform.position;
        Vector3 _toRim = _rimPos - currentBall.transform.position;

        Vector3 _backboardNormal = -basketTransform.forward;

        Vector3 _reflected = Vector3.Reflect(_toRim.normalized, _backboardNormal);

        // Adjust how far back you aim
        float _depthBehindRim = -0.5f; // decrease for side shots
        float _verticalOffset = 0.25f;

        // Tweak lateral offset based on angle
        Vector3 _lateralOffset = Vector3.Cross(Vector3.up, _backboardNormal).normalized;
        float _sideOffset = Vector3.Dot(_toRim.normalized, _lateralOffset);
        _lateralOffset *= _sideOffset * 0.25f; // Scale the lateral shift

        Vector3 _backboardTarget = _rimPos + _reflected * _depthBehindRim + _lateralOffset;
        _backboardTarget.y = _rimPos.y + _verticalOffset;

        return _backboardTarget;
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
                inputHandler.ResetInputs();

                scoringSystem.ResetBall();
            }
        }
    }

    private IEnumerator ApplyForceNextFrame(Vector3 force)
    {
        yield return null;
        ballRb.velocity = force;
        //ballRb.AddForce(force, ForceMode.Impulse);

        shotInProgress = true;

        //disable Inputs
    }
}
