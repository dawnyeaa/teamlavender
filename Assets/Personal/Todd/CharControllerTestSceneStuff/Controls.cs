//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.4
//     from Assets/Personal/Todd/CharControllerTestSceneStuff/Controls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @Controls : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @Controls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""player"",
            ""id"": ""955be93d-7b31-45d9-b352-3b68d07484db"",
            ""actions"": [
                {
                    ""name"": ""move"",
                    ""type"": ""Value"",
                    ""id"": ""6da265d8-0921-42de-9ddd-cef8a38bd8b8"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""push"",
                    ""type"": ""Button"",
                    ""id"": ""02bc57d9-d96b-4f34-ac1c-cdea968f97bb"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""look"",
                    ""type"": ""Value"",
                    ""id"": ""26c2c1fe-6273-4232-918b-b25f3afd152f"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""brake"",
                    ""type"": ""Button"",
                    ""id"": ""0c56de17-4ed7-4434-9c95-0b6656e6a271"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""switch"",
                    ""type"": ""Button"",
                    ""id"": ""a3203d16-d2d6-4010-8687-ecb7eac347e7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""rightStick"",
                    ""type"": ""PassThrough"",
                    ""id"": ""2b5fdc0f-4303-4b92-a6a6-29bba3fd010f"",
                    ""expectedControlType"": ""Stick"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""debug.reset"",
                    ""type"": ""Button"",
                    ""id"": ""7fe96717-757f-4620-a12e-71f4cc6f6422"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""crouch"",
                    ""type"": ""Button"",
                    ""id"": ""c1e794bd-ab4a-40cd-b0ac-8ba0b7cc842f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ollie"",
                    ""type"": ""Button"",
                    ""id"": ""ecd1b5fc-f94b-45e4-956a-b332c5a2cc42"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""debug.die"",
                    ""type"": ""Button"",
                    ""id"": ""de8f906f-b125-4de3-9875-bf93861ae5e2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""f7d20f4d-8bd7-42cf-b892-22ac91f4bc64"",
                    ""path"": ""<Gamepad>/leftStick/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""gamepad"",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""f34da913-eeed-4979-8993-a6b711af0de9"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""2750fd05-1dfd-43e0-bf4b-e299212d30a6"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KB + M"",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""cea1042c-0c4e-4d5d-b779-57d9fa0c14ff"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KB + M"",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""581d9da0-12f2-4532-a13c-e05c221d0f07"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KB + M"",
                    ""action"": ""push"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""632ee420-e050-4699-99bb-cb69ad6eee1b"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""gamepad"",
                    ""action"": ""push"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5839f0d4-ede6-418b-9042-212fdc8edae1"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KB + M"",
                    ""action"": ""look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""secondary look"",
                    ""id"": ""80991ccc-9ddb-44d4-b213-35b27031e915"",
                    ""path"": ""OneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""look"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""0006dda7-9b17-4d3a-9489-0428356e820d"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""gamepad"",
                    ""action"": ""look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""binding"",
                    ""id"": ""60f9244a-246b-4353-acba-e6deb1af0a75"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""gamepad"",
                    ""action"": ""look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""7ba19ec0-699e-4d3b-90f6-f82969743aed"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KB + M"",
                    ""action"": ""brake"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""019a8676-99bf-4cc0-95df-f64619e53bfa"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""gamepad"",
                    ""action"": ""brake"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b38bb221-c96f-48bc-83bb-c8bb9534b12f"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KB + M"",
                    ""action"": ""switch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1e2176e7-e048-4738-8436-234430c8e915"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""gamepad"",
                    ""action"": ""switch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1d43e8e9-c993-43e8-b961-73d5511b98da"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""gamepad"",
                    ""action"": ""rightStick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2c3ba339-daeb-42ad-9783-c57ac7f13633"",
                    ""path"": ""<Keyboard>/backspace"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KB + M"",
                    ""action"": ""debug.reset"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3313ee40-49ae-401c-a59a-cb7912aab2e2"",
                    ""path"": ""<Gamepad>/select"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""gamepad"",
                    ""action"": ""debug.reset"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9191e409-ffea-4065-8605-771b209ab411"",
                    ""path"": ""<Gamepad>/rightStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""gamepad"",
                    ""action"": ""crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9313f699-0839-4aed-831a-555ed45236f8"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""gamepad"",
                    ""action"": ""ollie"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8c2eea51-ccf6-43b1-89c3-ef5d42a1c68a"",
                    ""path"": ""<Keyboard>/delete"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KB + M"",
                    ""action"": ""debug.die"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""KB + M"",
            ""bindingGroup"": ""KB + M"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""gamepad"",
            ""bindingGroup"": ""gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": true,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // player
        m_player = asset.FindActionMap("player", throwIfNotFound: true);
        m_player_move = m_player.FindAction("move", throwIfNotFound: true);
        m_player_push = m_player.FindAction("push", throwIfNotFound: true);
        m_player_look = m_player.FindAction("look", throwIfNotFound: true);
        m_player_brake = m_player.FindAction("brake", throwIfNotFound: true);
        m_player_switch = m_player.FindAction("switch", throwIfNotFound: true);
        m_player_rightStick = m_player.FindAction("rightStick", throwIfNotFound: true);
        m_player_debugreset = m_player.FindAction("debug.reset", throwIfNotFound: true);
        m_player_crouch = m_player.FindAction("crouch", throwIfNotFound: true);
        m_player_ollie = m_player.FindAction("ollie", throwIfNotFound: true);
        m_player_debugdie = m_player.FindAction("debug.die", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // player
    private readonly InputActionMap m_player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_player_move;
    private readonly InputAction m_player_push;
    private readonly InputAction m_player_look;
    private readonly InputAction m_player_brake;
    private readonly InputAction m_player_switch;
    private readonly InputAction m_player_rightStick;
    private readonly InputAction m_player_debugreset;
    private readonly InputAction m_player_crouch;
    private readonly InputAction m_player_ollie;
    private readonly InputAction m_player_debugdie;
    public struct PlayerActions
    {
        private @Controls m_Wrapper;
        public PlayerActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @move => m_Wrapper.m_player_move;
        public InputAction @push => m_Wrapper.m_player_push;
        public InputAction @look => m_Wrapper.m_player_look;
        public InputAction @brake => m_Wrapper.m_player_brake;
        public InputAction @switch => m_Wrapper.m_player_switch;
        public InputAction @rightStick => m_Wrapper.m_player_rightStick;
        public InputAction @debugreset => m_Wrapper.m_player_debugreset;
        public InputAction @crouch => m_Wrapper.m_player_crouch;
        public InputAction @ollie => m_Wrapper.m_player_ollie;
        public InputAction @debugdie => m_Wrapper.m_player_debugdie;
        public InputActionMap Get() { return m_Wrapper.m_player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @move.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @move.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @move.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @push.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPush;
                @push.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPush;
                @push.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPush;
                @look.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLook;
                @look.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLook;
                @look.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLook;
                @brake.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnBrake;
                @brake.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnBrake;
                @brake.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnBrake;
                @switch.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSwitch;
                @switch.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSwitch;
                @switch.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSwitch;
                @rightStick.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRightStick;
                @rightStick.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRightStick;
                @rightStick.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRightStick;
                @debugreset.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDebugreset;
                @debugreset.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDebugreset;
                @debugreset.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDebugreset;
                @crouch.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCrouch;
                @crouch.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCrouch;
                @crouch.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCrouch;
                @ollie.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnOllie;
                @ollie.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnOllie;
                @ollie.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnOllie;
                @debugdie.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDebugdie;
                @debugdie.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDebugdie;
                @debugdie.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDebugdie;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @move.started += instance.OnMove;
                @move.performed += instance.OnMove;
                @move.canceled += instance.OnMove;
                @push.started += instance.OnPush;
                @push.performed += instance.OnPush;
                @push.canceled += instance.OnPush;
                @look.started += instance.OnLook;
                @look.performed += instance.OnLook;
                @look.canceled += instance.OnLook;
                @brake.started += instance.OnBrake;
                @brake.performed += instance.OnBrake;
                @brake.canceled += instance.OnBrake;
                @switch.started += instance.OnSwitch;
                @switch.performed += instance.OnSwitch;
                @switch.canceled += instance.OnSwitch;
                @rightStick.started += instance.OnRightStick;
                @rightStick.performed += instance.OnRightStick;
                @rightStick.canceled += instance.OnRightStick;
                @debugreset.started += instance.OnDebugreset;
                @debugreset.performed += instance.OnDebugreset;
                @debugreset.canceled += instance.OnDebugreset;
                @crouch.started += instance.OnCrouch;
                @crouch.performed += instance.OnCrouch;
                @crouch.canceled += instance.OnCrouch;
                @ollie.started += instance.OnOllie;
                @ollie.performed += instance.OnOllie;
                @ollie.canceled += instance.OnOllie;
                @debugdie.started += instance.OnDebugdie;
                @debugdie.performed += instance.OnDebugdie;
                @debugdie.canceled += instance.OnDebugdie;
            }
        }
    }
    public PlayerActions @player => new PlayerActions(this);
    private int m_KBMSchemeIndex = -1;
    public InputControlScheme KBMScheme
    {
        get
        {
            if (m_KBMSchemeIndex == -1) m_KBMSchemeIndex = asset.FindControlSchemeIndex("KB + M");
            return asset.controlSchemes[m_KBMSchemeIndex];
        }
    }
    private int m_gamepadSchemeIndex = -1;
    public InputControlScheme gamepadScheme
    {
        get
        {
            if (m_gamepadSchemeIndex == -1) m_gamepadSchemeIndex = asset.FindControlSchemeIndex("gamepad");
            return asset.controlSchemes[m_gamepadSchemeIndex];
        }
    }
    public interface IPlayerActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnPush(InputAction.CallbackContext context);
        void OnLook(InputAction.CallbackContext context);
        void OnBrake(InputAction.CallbackContext context);
        void OnSwitch(InputAction.CallbackContext context);
        void OnRightStick(InputAction.CallbackContext context);
        void OnDebugreset(InputAction.CallbackContext context);
        void OnCrouch(InputAction.CallbackContext context);
        void OnOllie(InputAction.CallbackContext context);
        void OnDebugdie(InputAction.CallbackContext context);
    }
}
