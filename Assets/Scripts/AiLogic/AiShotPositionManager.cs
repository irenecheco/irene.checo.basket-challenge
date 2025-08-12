using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiShotPositionManager : MonoBehaviour
{
    [SerializeField] private List<ShootingPos> ShootingPositions = new List<ShootingPos>();
    [SerializeField] private AiShotManager aiShotManager;
    [SerializeField] private ShotPositionManager shotPosMan;

    public int _previousPos = 0;

    private void Start()
    {
        ChangePos();
    }

    public void ChangePos()
    {
        int _currentPos = Random.Range(0, ShootingPositions.Count);
        while (_currentPos == _previousPos || _currentPos == shotPosMan._previousPos)
        {
            _currentPos = Random.Range(0, ShootingPositions.Count);
        }

        //Debug.Log($"current position is {ShootingPositions[_currentPos].ShootingPosTransform.gameObject.name}");

        _previousPos = _currentPos;

        Spawn();
    }

    public void Spawn()
    {
        transform.position = new Vector3(ShootingPositions[_previousPos].ShootingPosTransform.position.x, 1.9f, ShootingPositions[_previousPos].ShootingPosTransform.position.z);
        transform.rotation = ShootingPositions[_previousPos].ShootingPosTransform.rotation;
        aiShotManager.SpawnBall();
    }
}
