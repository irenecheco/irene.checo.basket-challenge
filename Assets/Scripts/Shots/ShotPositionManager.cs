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
    [SerializeField] private UiInputBar uiInputBar;
    [SerializeField] private AiShotPositionManager aiShotPosMan;

    public int _previousPos = 0;

    private void Start()
    {
        ChangePos();
    }

    public void ChangePos()
    {
        int _currentPos = Random.Range(0, ShootingPositions.Count);
        while (_currentPos == _previousPos ||_currentPos == aiShotPosMan._previousPos)
        {
            _currentPos = Random.Range(0, ShootingPositions.Count);
        }

        //Debug.Log($"current position is {ShootingPositions[_currentPos].ShootingPosTransform.gameObject.name}");

        _previousPos = _currentPos;
        if (GameManager.Instance.CurrentState == GameState.Playing) uiInputBar.SetBarRanges();

        Spawn();
    }

    public void Spawn()
    {
        MainCamera.transform.position = new Vector3(ShootingPositions[_previousPos].ShootingPosTransform.position.x, 1.9f, ShootingPositions[_previousPos].ShootingPosTransform.position.z);
        MainCamera.transform.rotation = ShootingPositions[_previousPos].ShootingPosTransform.rotation;
        shotManager.SpawnBall();
    }
}
