using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class InputHandler : MonoBehaviour
{
    [Header("Swipe ratios")]
    [SerializeField, Range(0.5f, 1f)] private float maxSwipeHeightRatio = 0.65f;
    [SerializeField, Range(0f, 0.5f)] private float minSwipeHeightRatio = 0.01f;

    [Header("Inner Shot Outcome Thresholds")]
    [SerializeField, Range(0f, 1f)] public float missMaxThreshold = 0.35f;
    [SerializeField, Range(0f, 1f)] public float cleanMinThreshold = 0.45f;
    [SerializeField, Range(0f, 1f)] public float cleanMaxThreshold = 0.60f;
    [SerializeField, Range(0f, 1f)] public float backboardMissMinThreshold = 0.70f;
    [SerializeField, Range(0f, 1f)] public float backboardMinThreshold = 0.80f;
    [SerializeField, Range(0f, 1f)] public float backboardMaxThreshold = 0.95f;

    [Header("Outer Shot Outcome Thresholds")]
    [SerializeField, Range(0f, 1f)] public float missMaxThresholdOut = 0.35f;
    [SerializeField, Range(0f, 1f)] public float cleanMinThresholdOut = 0.45f;
    [SerializeField, Range(0f, 1f)] public float cleanMaxThresholdOut = 0.60f;
    [SerializeField, Range(0f, 1f)] public float backboardMissMinThresholdOut = 0.70f;
    [SerializeField, Range(0f, 1f)] public float backboardMinThresholdOut = 0.80f;
    [SerializeField, Range(0f, 1f)] public float backboardMaxThresholdOut = 0.95f;

    [SerializeField] private ShotManager shotManager;
    [SerializeField] private ShotPositionManager shotPositionManager;
    [SerializeField] private float swipeMaxTime = 2f;
    [SerializeField] private TextMeshProUGUI debugText;
    [SerializeField] private UiInputBar inputBar;

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
    private bool endedSwipe = false;

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

        inputBar.SetForce(ComputeForce());

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
            if (!endedSwipe)
            {
                endedSwipe = true;
                HandleSwipeEnd();                
                return;
            }            
        }

        if (swipeTimerOn)
        {
            if(remainingSwipeTime > 0)
            {
                remainingSwipeTime -= Time.deltaTime;
            }
            else
            {
                if (!endedSwipe)
                {
                    endedSwipe = true;
                    HandleSwipeEnd();                    
                    return;
                }
            }            
        }
    }

    private void OnTouchStart(InputAction.CallbackContext _context)
    {
        SwipeStart = inputActions.Player.TouchPosition.ReadValue<Vector2>();
        IsSwiping = true;
        remainingSwipeTime = swipeMaxTime;
        swipeTimerOn = true;
        endedSwipe = false;

        maxYDuringSwipe = SwipeStart.y;
        reachedPeak = false;
    }

    private void OnTouchEnd(InputAction.CallbackContext _context)
    {
        if (!endedSwipe)
        {
            endedSwipe = true;
            HandleSwipeEnd();            
        }
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

        Debug.Log($"normalized power è {_normalizedPower}");
        inputActions.Disable();

        shotManager.Shoot(DetermineShotType(_normalizedPower));
    }
    
    private float ComputeForce()
    {
        SwipeEnd = inputActions.Player.TouchPosition.ReadValue<Vector2>();
        Vector2 _swipeDelta = SwipeEnd - SwipeStart;
        float _verticalSwipeLength = Mathf.Max(0f, _swipeDelta.y);
        if (_verticalSwipeLength < minSwipeThreshold) return 0f;
        float _normalizedPower = Mathf.Clamp01(_verticalSwipeLength / maxSwipeHeight);        
        return _normalizedPower;
    }

    public void ResetInputs()
    {
        inputActions.Enable();
    }

    private ShotType DetermineShotType(float _normalizedPower)
    {
        if (shotPositionManager._previousPos <= 4) 
        {
            if (_normalizedPower < missMaxThreshold)
            {
                Debug.Log("miss");
                return ShotType.Miss;
            }
            else if (missMaxThreshold <= _normalizedPower && _normalizedPower < cleanMinThreshold)
            {
                Debug.Log("rim");
                return ShotType.Rim;
            }
            else if (cleanMinThreshold <= _normalizedPower && _normalizedPower < cleanMaxThreshold)
            {
                Debug.Log("clean");
                return ShotType.Clean;
            }
            else if (cleanMaxThreshold <= _normalizedPower && _normalizedPower < backboardMissMinThreshold)
            {
                Debug.Log("rim");
                return ShotType.Rim;
            }
            else if (backboardMissMinThreshold <= _normalizedPower && _normalizedPower < backboardMinThreshold)
            {
                Debug.Log("backboard miss");
                return ShotType.BackboardMiss;
            }
            else if (backboardMinThreshold <= _normalizedPower && _normalizedPower < backboardMaxThreshold)
            {
                Debug.Log("backboard");
                return ShotType.Backboard;
            }
            else
            {
                Debug.Log("long miss");
                return ShotType.LongMiss;
            }
        } else
        {
            if (_normalizedPower < missMaxThresholdOut)
            {
                Debug.Log("miss");
                return ShotType.Miss;
            }
            else if (missMaxThresholdOut <= _normalizedPower && _normalizedPower < cleanMinThresholdOut)
            {
                Debug.Log("rim");
                return ShotType.Rim;
            }
            else if (cleanMinThresholdOut <= _normalizedPower && _normalizedPower < cleanMaxThresholdOut)
            {
                Debug.Log("clean");
                return ShotType.Clean;
            }
            else if (cleanMaxThresholdOut <= _normalizedPower && _normalizedPower < backboardMissMinThresholdOut)
            {
                Debug.Log("rim");
                return ShotType.Rim;
            }
            else if (backboardMissMinThresholdOut <= _normalizedPower && _normalizedPower < backboardMinThresholdOut)
            {
                Debug.Log("backboard miss");
                return ShotType.BackboardMiss;
            }
            else if (backboardMinThresholdOut <= _normalizedPower && _normalizedPower < backboardMaxThresholdOut)
            {
                Debug.Log("backboard");
                return ShotType.Backboard;
            }
            else
            {
                Debug.Log("long miss");
                return ShotType.LongMiss;
            }
        }
                    
    }


}
