using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static Vector2 Movement;
    public static Vector2 MousePosition;

    public static bool AttackPressed;
    public static bool WeaponSlot1Pressed;
    public static bool WeaponSlot2Pressed;

    private PlayerInput _playerInput;

    private InputAction _moveAction;
    private InputAction _attackAction;
    private InputAction _mousePositionAction;
    private InputAction _weaponSlot1Action;
    private InputAction _weaponSlot2Action;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();

        _moveAction = _playerInput.actions["Move"];
        _attackAction = _playerInput.actions["Attack"];
        _mousePositionAction = _playerInput.actions["MousePosition"];
        _weaponSlot1Action = _playerInput.actions["WeaponSlot1"];
        _weaponSlot2Action = _playerInput.actions["WeaponSlot2"];
    }

    private void Update()
    {
        Movement = _moveAction.ReadValue<Vector2>();
        MousePosition = _mousePositionAction.ReadValue<Vector2>();

        AttackPressed = _attackAction.WasPressedThisFrame();
        WeaponSlot1Pressed = _weaponSlot1Action.WasPressedThisFrame();
        WeaponSlot2Pressed = _weaponSlot2Action.WasPressedThisFrame();
    }
}
