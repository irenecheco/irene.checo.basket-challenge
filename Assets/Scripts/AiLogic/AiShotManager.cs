using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiShotManager : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private GameObject aiBallPrefab;
    [SerializeField] private Transform aiBallPosition;
    [SerializeField] private AiShotPositionManager aiShotPosMan;
    [SerializeField] private Transform basketTransform;
    [SerializeField] private float launchAngle = 45f;
    [SerializeField] private AiScoringSystem aiScoringSystem;
    [SerializeField] private AiFireballBonus aiFireballBonus;
    [SerializeField] private GameObject FireballFirePrefab;
    #endregion

    #region Variables
    private GameObject currentBall;
    private GameObject currentFire;
    private Rigidbody ballRb;
    private bool shotInProgress = false;
    private bool reset = false;
    #endregion

    public void Shoot(ShotType _shotType)
    {
        if (shotInProgress) return;

        Vector3 _targetPos = DetermineTargetPosition(_shotType);

        Vector3 velocity = CalculateVelocityFromAngle(currentBall.transform.position, _targetPos, launchAngle, Physics.gravity.magnitude);

        ballRb = currentBall.GetComponent<Rigidbody>();
        ballRb.isKinematic = false;
        ballRb.WakeUp();
        ballRb.velocity = velocity;

        shotInProgress = true;
    }

    public Vector3 CalculateVelocityFromAngle(Vector3 startPos, Vector3 targetPos, float launchAngleDegrees, float gravity)
    {
        Vector3 displacement = targetPos - startPos;
        Vector3 displacementXZ = new Vector3(displacement.x, 0, displacement.z);
        float horizontalDistance = displacementXZ.magnitude;
        float verticalDistance = displacement.y;
        float angleRad = launchAngleDegrees * Mathf.Deg2Rad;
        float gravityAbs = Mathf.Abs(gravity);

        float numerator = gravityAbs * horizontalDistance * horizontalDistance;
        float denominator = 2 * Mathf.Pow(Mathf.Cos(angleRad), 2) * (horizontalDistance * Mathf.Tan(angleRad) - verticalDistance);

        if (denominator <= 0)
        {
            Debug.LogWarning("Impossible shot with given angle and target");
            return Vector3.zero;
        }

        float initialSpeed = Mathf.Sqrt(numerator / denominator);
        Vector3 horizontalDirection = displacementXZ.normalized;

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

        float _depthBehindRim = -0.5f;
        float _verticalOffset = 0.25f;
        Vector3 _lateralOffset = Vector3.Cross(Vector3.up, _backboardNormal).normalized;
        float _sideOffset = Vector3.Dot(_toRim.normalized, _lateralOffset);
        _lateralOffset *= _sideOffset * 0.25f;

        Vector3 _backboardTarget = _rimPos + _reflected * _depthBehindRim + _lateralOffset;
        _backboardTarget.y = _rimPos.y + _verticalOffset;
        return _backboardTarget;
    }

    public void SpawnBall()
    {
        currentBall = Instantiate(aiBallPrefab, aiBallPosition.position, Quaternion.identity);
        if (aiFireballBonus.FireballActive) 
            StartFire();
    }

    public void StartFire()
    {
        currentFire = Instantiate(FireballFirePrefab, currentBall.transform);
    }

    public void EndFire()
    {
        Destroy(currentFire);
    }

    private Vector3 DetermineTargetPosition(ShotType _shotType)
    {
        Vector3 _targetPos = basketTransform.position;

        switch (_shotType)
        {
            case ShotType.Clean:
                break;

            case ShotType.Backboard:
                _targetPos = CalculateBackboardPos();
                break;

            case ShotType.Rim:
                float rimRadius = 0.14f;
                Vector3 rimOffset = new Vector3(
                    Random.Range(-rimRadius, rimRadius),
                    Random.Range(-rimRadius / 2, rimRadius / 2),
                    Random.Range(-rimRadius, rimRadius)
                );

                while ((Mathf.Abs(rimOffset.x) + Mathf.Abs(rimOffset.y) + Mathf.Abs(rimOffset.z)) < 0.13
                    || (Mathf.Abs(rimOffset.x) + Mathf.Abs(rimOffset.y) + Mathf.Abs(rimOffset.z)) > 0.24)
                {
                    rimOffset = new Vector3(
                    Random.Range(-rimRadius, rimRadius),
                    Random.Range(-rimRadius / 2, rimRadius / 2),
                    Random.Range(-rimRadius, rimRadius)
                );
                }

                _targetPos = basketTransform.position + rimOffset;
                break;

            case ShotType.BackboardMiss:

                float xMiss = Random.value < 0.5f
                ? Random.Range(-0.5f, -0.3f)
                : Random.Range(0.3f, 0.5f);

                float yMiss = Random.Range(0.2f, 0.4f);

                Vector3 missOffset = new Vector3(xMiss, yMiss, 0);

                Debug.Log($"miss offset is {missOffset}");
                _targetPos = CalculateBackboardPos() + missOffset;
                aiFireballBonus.ResetFireballBar();
                break;

            case ShotType.Miss:
                Vector3 shortOffset = new Vector3(
                Random.Range(-0.5f, 0.5f),
                Random.Range(-1.2f, -0.6f),
                Random.Range(-1.0f, -0.6f));

                _targetPos = basketTransform.position + shortOffset;
                aiFireballBonus.ResetFireballBar();
                break;

            case ShotType.LongMiss:
                Vector3 longMissOffset = new Vector3(
                    Random.Range(-0.3f, 0.3f),
                    Random.Range(0.3f, 0.7f),
                    Random.Range(0.5f, 1f)
                );

                _targetPos = basketTransform.position + longMissOffset;
                aiFireballBonus.ResetFireballBar();
                break;

            default:
                _targetPos = basketTransform.position;
                break;
        }

        return _targetPos;
    }

    void Update()
    {
        if(shotInProgress && currentBall != null)
        {
            if(currentBall.transform.position.y < 0.2f)
            {
                if (!reset)
                {
                    reset = true;
                    StartCoroutine(WaitAndReset());
                }                
            }
        }
    }

    private IEnumerator WaitAndReset()
    {
        yield return new WaitForSeconds(.5f);
        Destroy(currentBall);
        currentBall = null;
        shotInProgress = false;

        aiScoringSystem.ResetBall();
        reset = false;
    }
}
