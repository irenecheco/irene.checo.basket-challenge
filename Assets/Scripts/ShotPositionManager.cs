using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShootingPos
{
    public Transform ShootingPosTransform;
    public float MaxForce;
}

public class ShotPositionManager : MonoBehaviour
{
    [SerializeField] private List<ShootingPos> ShootingPositions = new List<ShootingPos>();
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
        MainCamera.transform.position = new Vector3(ShootingPositions[_currentPos].ShootingPosTransform.position.x, MainCamera.transform.position.y, ShootingPositions[_currentPos].ShootingPosTransform.position.z);
        MainCamera.transform.rotation = ShootingPositions[_currentPos].ShootingPosTransform.rotation;

        _previousPos = _currentPos;

        shotManager.SpawnBall(ShootingPositions[_previousPos].MaxForce);
    }
}
