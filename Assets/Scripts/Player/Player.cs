using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectParent
{
    public static Player Instance { get; set; }

    public event EventHandler OnPickedSomething;
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs 
    {
        public BaseCounter selectedCounter; 
    }


    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask counterLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;

    private Vector2 _inputVector;
    private Vector3 _moveDir;
    private Vector3 _lastInteractDir;
    private bool _isWalking;
    private BaseCounter _selectedCounter;
    private KitchenObject _kitchenObject;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
        gameInput.onInteractAlternateAction += GameInput_onInteractAlternateAction;
    }

    private void GameInput_onInteractAlternateAction(object sender, EventArgs e)
    {
        if (!KitchenGameManager.Instance.IsGamePlaying()) return;

        if (_selectedCounter != null)
        {
            _selectedCounter.InteractAlternate(this);
        }
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        if (!KitchenGameManager.Instance.IsGamePlaying()) return;

        if (_selectedCounter != null)
        {
            _selectedCounter.Interact(this);
        }
    }

    private void Update()
    {
        HandleMovement();
        HandleInteractions();
    }

    public bool IsWalking()
    {
        return _isWalking;
    }

    private void HandleInteractions()
    {
        _inputVector = gameInput.GetMoveMentVectorNormalized();

        _moveDir = new Vector3(_inputVector.x, 0f, _inputVector.y);

        if (_moveDir != Vector3.zero)
        {
            _lastInteractDir = _moveDir;
        }

        float interactDistance = 2f;
        if (Physics.Raycast(transform.position, _lastInteractDir, out RaycastHit raycastHit, interactDistance, counterLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                // Has ClearCounter
                if (baseCounter != _selectedCounter)
                {
                    SetSelectedCounter(baseCounter);
                }
            }
            else
            {
                SetSelectedCounter(null);
            }
        }
        else
        {
            SetSelectedCounter(null);
        }
    }

    private void HandleMovement()
    {
        float _moveDistance = moveSpeed * Time.deltaTime;
        float _playerRadius = .7f;
        float _playerHeight = 2f;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * _playerHeight, _playerRadius, _moveDir, _moveDistance);

        if (!canMove)
        {
            // Cannot move towards moveDir

            // Attempt only X movement 
            Vector3 moveDirX = new Vector3(_moveDir.x, 0, 0).normalized;
            canMove = (_moveDir.x < -.5f || _moveDir.x > +.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * _playerHeight, _playerRadius, moveDirX, _moveDistance);

            if (canMove)
            {
                // Can move only on the X
                _moveDir = moveDirX;
            }
            else
            {
                // Cannot move only on the X

                // Attempt only Z movement
                Vector3 moveDirZ = new Vector3(0, 0, _moveDir.z).normalized;
                canMove = (_moveDir.z < -.5f || _moveDir.z > +.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * _playerHeight, _playerRadius, moveDirZ, _moveDistance);

                if (canMove)
                {
                    // Can move only on the Z
                    _moveDir = moveDirZ;
                }
                else
                {
                    // Cannot move in any direction
                }
            }
        }

        if (canMove)
        {
            transform.position += _moveDir * moveSpeed * Time.deltaTime;
        }

        _isWalking = _moveDir != Vector3.zero;

        transform.forward = Vector3.Slerp(transform.forward, _moveDir, Time.deltaTime * rotateSpeed);
    }

    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        _selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectedCounter = _selectedCounter
        });
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        _kitchenObject = kitchenObject;

        if (kitchenObject != null)
        {
            OnPickedSomething?.Invoke(this, EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject()
    {
        return _kitchenObject;
    }

    public void ClearKitchenObject()
    {
        _kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return _kitchenObject != null;
    }
}
