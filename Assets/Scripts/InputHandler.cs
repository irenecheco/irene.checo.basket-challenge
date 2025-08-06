using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class InputHandler : MonoBehaviour
{
    [SerializeField, Range(0.5f, 1f)] private float maxSwipeHeightRatio = 0.75f;
    [SerializeField, Range(0f, 0.5f)] private float minSwipeHeightRatio = 0.1f;
    [SerializeField] private ShotManager shotManager;
    [SerializeField] private TextMeshProUGUI debugText;

    public Vector2 SwipeStart { get; private set; }
    public Vector2 SwipeEnd { get; private set; }
    public bool IsSwiping { get; private set;  }

    private PlayerInputActions inputActions;
    private float maxSwipeHeight;
    private float minSwipeThreshold;

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
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void OnTouchStart(InputAction.CallbackContext _context)
    {
        SwipeStart = inputActions.Player.TouchPosition.ReadValue<Vector2>();
        IsSwiping = true;
    }

    private void OnTouchEnd(InputAction.CallbackContext _context)
    {
        SwipeEnd = inputActions.Player.TouchPosition.ReadValue<Vector2>();
        IsSwiping = false;

        Vector2 _swipeDelta = SwipeEnd - SwipeStart;
        float _verticalSwipeLength = Mathf.Max(0f, _swipeDelta.y);
        float _normalizedPower = Mathf.Clamp01(_verticalSwipeLength / maxSwipeHeight);

        Debug.Log($"Vertical swipe: {_verticalSwipeLength}, normalized power: {_normalizedPower}");
        debugText.text = $"Vertical swipe: {_verticalSwipeLength}, normalized power: {_normalizedPower}";

        if (_verticalSwipeLength < minSwipeThreshold) return;

        shotManager.Shoot(_normalizedPower, ShotType.Backboard);

        //shotManager.Shoot(_normalizedPower);        
    }


}
