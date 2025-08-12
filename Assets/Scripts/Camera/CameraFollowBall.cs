using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowBall : MonoBehaviour
{
    [SerializeField] private float extraHeight = 1.5f;
    [SerializeField] private float followSpeed = .5f;
    [SerializeField] private float followDuration = 1f; // total follow time
    [SerializeField] private float easeOutStart = 0.6f; // when to start slowing down
    [SerializeField] private Transform hoopPos;

    public Transform Ball;

    private Vector3 initialOffset;
    private Vector3 hoopOffset = new Vector3(0f, .3f, 0f);
    private bool isFollowing = false;
    private float followTimer;

    public void StartFollowing()
    {
        if (Ball == null) return;
        initialOffset = transform.position - Ball.position; // Store starting offset
        followTimer = 0f;
        isFollowing = true;
    }

    private void LateUpdate()
    {
        if (!isFollowing || Ball == null) return;

        followTimer += Time.deltaTime;

        // Calculate easing factor
        float speedFactor = 1f;
        if (followTimer >= easeOutStart)
        {
            float easeT = Mathf.InverseLerp(easeOutStart, followDuration, followTimer);
            speedFactor = Mathf.Lerp(1f, 0f, easeT); // from full speed to stop
        }

        // Target position
        Vector3 desiredPos = Ball.position + initialOffset;
        desiredPos.y = Mathf.Max(desiredPos.y, Ball.position.y + extraHeight);

        // Smooth move
        transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * followSpeed * speedFactor);

        // Look at the ball
        transform.LookAt(hoopPos.position + hoopOffset);

        // Stop after duration
        if (followTimer >= followDuration)
        {
            isFollowing = false;
        }
    }
}
