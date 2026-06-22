using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public static Vector2 Movement { get; private set; }
    public static Vector2 MousePosition { get; private set; }

    public static bool AttackPressed { get; private set; }
    public static bool WeaponSlot1Pressed { get; private set; }
    public static bool WeaponSlot2Pressed { get; private set; }

    private PlayerInput _playerInput;

    private InputAction _moveAction;
    private InputAction _attackAction;
    private InputAction _mousePositionAction;
    private InputAction _weaponSlot1Action;
    private InputAction _weaponSlot2Action;

    private void Awake()
    {
        SetUpSingleton();

        if (Instance != this)
        {
            return;
        }

        GetInputActions();
    }

    private void Update()
    {
        ReadContinuousInput();
        ReadButtonInput();
    }

    private void OnDisable()
    {
        ResetInput();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void SetUpSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("A duplicate InputManager was found and removed.", gameObject);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void GetInputActions()
    {
        _playerInput = GetComponent<PlayerInput>();

        _moveAction = FindInputAction("Move");
        _attackAction = FindInputAction("Attack");
        _mousePositionAction = FindInputAction("MousePosition");
        _weaponSlot1Action = FindInputAction("WeaponSlot1");
        _weaponSlot2Action = FindInputAction("WeaponSlot2");
    }

    private InputAction FindInputAction(string actionName)
    {
        InputAction action = _playerInput.actions.FindAction(actionName);

        if (action == null)
        {
            Debug.LogError(
                "Input action '" + actionName + "' was not found.",
                gameObject
            );
        }

        return action;
    }

    private void ReadContinuousInput()
    {
        Movement = _moveAction != null ? _moveAction.ReadValue<Vector2>() : Vector2.zero;
        MousePosition = _mousePositionAction != null ? _mousePositionAction.ReadValue<Vector2>() : Vector2.zero;
    }

    private void ReadButtonInput()
    {
        AttackPressed = _attackAction != null && _attackAction.WasPressedThisFrame();
        WeaponSlot1Pressed = _weaponSlot1Action != null && _weaponSlot1Action.WasPressedThisFrame();
        WeaponSlot2Pressed = _weaponSlot2Action != null && _weaponSlot2Action.WasPressedThisFrame();
    }

    private void ResetInput()
    {
        Movement = Vector2.zero;
        MousePosition = Vector2.zero;

        AttackPressed = false;
        WeaponSlot1Pressed = false;
        WeaponSlot2Pressed = false;
    }
}