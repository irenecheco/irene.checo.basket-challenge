using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotPositionManager : MonoBehaviour
{
    [SerializeField] private List<Transform> ShootingPositions = new List<Transform>();
    [SerializeField] private GameObject MainCamera;
    [SerializeField] private ShotManager shotManager;

    private int _previousPos = 0;

    private void Start()
    {
        ChangePos();
    }

    public void ChangePos()
    {
        int _currentPos = Random.Range(0, ShootingPositions.Count);
        while (_currentPos == _previousPos)
        {
            _currentPos = Random.Range(0, ShootingPositions.Count);
        }
        MainCamera.transform.position = new Vector3(ShootingPositions[_currentPos].position.x, MainCamera.transform.position.y, ShootingPositions[_currentPos].position.z);
        MainCamera.transform.rotation = ShootingPositions[_currentPos].rotation;

        _previousPos = _currentPos;

        shotManager.SpawnBall();
    }
}
