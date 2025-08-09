using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class InputHandler : MonoBehaviour
{
    [Header("Swipe ratios")]
    [SerializeField, Range(0.5f, 1f)] private float maxSwipeHeightRatio = 0.75f;
    [SerializeField, Range(0f, 0.5f)] private float minSwipeHeightRatio = 0.1f;

    [Header("Shot Outcome Thresholds")]
    [SerializeField, Range(0f, 1f)] private float missMaxThreshold = 0.35f;
    [SerializeField, Range(0f, 1f)] private float cleanMinThreshold = 0.45f;
    [SerializeField, Range(0f, 1f)] private float cleanMaxThreshold = 0.60f;
    [SerializeField, Range(0f, 1f)] private float backboardMissMinThreshold = 0.70f;
    [SerializeField, Range(0f, 1f)] private float backboardMinThreshold = 0.80f;
    [SerializeField, Range(0f, 1f)] private float backboardMaxThreshold = 0.95f;

    [SerializeField] private ShotManager shotManager;
    [SerializeField] private float swipeMaxTime = 2f;
    [SerializeField] private TextMeshProUGUI debugText;

    public Vector2 SwipeStart { get; private set; }
    public Vector2 SwipeEnd { get; private set; }
    public bool IsSwiping { get; private set;  }

    private PlayerInputActions inputActions;
    private float maxSwipeHeight;
    private float minSwipeThreshold;
    private bool swipeTimerOn;
    private float remainingSwipeTime = 0;
    private float maxYDuringSwipe;
    private bool reachedPeak;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        maxSwipeHeight = Screen.height * maxSwipeHeightRatio;
        minSwipeThreshold = Screen.height * minSwipeHeightRatio;
    }

    private void OnEnable()
    {
        inputActions.Player.TouchPress.started += OnTouchStart;
        inputActions.Player.TouchPress.canceled += OnTouchEnd;
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Update()
    {
        if (!IsSwiping) return;

        Vector2 currentPos = inputActions.Player.TouchPosition.ReadValue<Vector2>();

        // Update peak height
        if (currentPos.y > maxYDuringSwipe)
        {
            maxYDuringSwipe = currentPos.y;
            reachedPeak = true;
        }

        // Detect downward movement after peak
        if (reachedPeak && currentPos.y < maxYDuringSwipe - 10f) // 10px tolerance
        {
            HandleSwipeEnd();
            return;
        }

        if (swipeTimerOn)
        {
            if(remainingSwipeTime > 0)
            {
                remainingSwipeTime -= Time.deltaTime;
            }
            else
            {
                HandleSwipeEnd();
            }            
        }
    }

    private void OnTouchStart(InputAction.CallbackContext _context)
    {
        SwipeStart = inputActions.Player.TouchPosition.ReadValue<Vector2>();
        IsSwiping = true;
        remainingSwipeTime = swipeMaxTime;
        swipeTimerOn = true;

        maxYDuringSwipe = SwipeStart.y;
        reachedPeak = false;
    }

    private void OnTouchEnd(InputAction.CallbackContext _context)
    {
        HandleSwipeEnd();
    }

    private void HandleSwipeEnd()
    {
        swipeTimerOn = false;
        SwipeEnd = inputActions.Player.TouchPosition.ReadValue<Vector2>();
        IsSwiping = false;

        Vector2 _swipeDelta = SwipeEnd - SwipeStart;
        float _verticalSwipeLength = Mathf.Max(0f, _swipeDelta.y);
        float _normalizedPower = Mathf.Clamp01(_verticalSwipeLength / maxSwipeHeight);

        if (_verticalSwipeLength < minSwipeThreshold) return;

        inputActions.Disable();

        shotManager.Shoot(DetermineShotType(_normalizedPower));
    }

    public void ResetInputs()
    {
        inputActions.Enable();
    }

    private ShotType DetermineShotType(float _normalizedPower)
    {
        if (_normalizedPower < missMaxThreshold)
        {
            Debug.Log("miss");
            debugText.text = "miss";
            return ShotType.Miss;
        }            
        else if (missMaxThreshold <= _normalizedPower && _normalizedPower < cleanMinThreshold)
        {
            Debug.Log("rim");
            debugText.text = "rim";
            return ShotType.Rim;
        }            
        else if (cleanMinThreshold <= _normalizedPower && _normalizedPower < cleanMaxThreshold)
        {
            Debug.Log("clean");
            debugText.text = "clean";
            return ShotType.Clean;
        }            
        else if (cleanMaxThreshold <= _normalizedPower && _normalizedPower < backboardMissMinThreshold)
        {
            Debug.Log("rim");
            debugText.text = "rim";
            return ShotType.Rim;
        }            
        else if (backboardMissMinThreshold <= _normalizedPower && _normalizedPower < backboardMinThreshold)
        {
            Debug.Log("backboard miss");
            debugText.text = "backboard miss";
            return ShotType.BackboardMiss;
        }            
        else if (backboardMinThreshold <= _normalizedPower && _normalizedPower < backboardMaxThreshold)
        {
            Debug.Log("backboard");
            debugText.text = "backboard";
            return ShotType.Backboard;
        }
        else
        {
            Debug.Log("long miss");
            debugText.text = "long miss";
            return ShotType.LongMiss;
        }            
    }


}
