// Assets/scripts/battle/demos/BattleLoopDemo.cs
using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem; // New Input System
#endif

namespace Dreamshade.Battle
{
    /// Attach this to the same GameObject as BattleController.
    /// Space = step one increment in manual mode.
    /// Keys 1/2/3 = 0.5x / 1x / 2x speed when not in manual mode.
    public sealed class BattleLoopDemo : MonoBehaviour
    {
        [SerializeField] private BattleController controller;

        [Header("Participants")]
        [SerializeField] private int daggerAutoCost = 10;
        [SerializeField] private int greatswordAutoCost = 20;

        [Header("Simulation")]
        [SerializeField] private bool manualStep = true;
        [SerializeField] private int incrementsPerSpace = 1;

        private void Awake()
        {
            if (!controller) controller = GetComponent<BattleController>();

            controller.ClearParticipants();
            controller.AddRecruit(new RecruitInBattle("Dagger Recruit", daggerAutoCost));
            controller.AddRecruit(new RecruitInBattle("Greatsword Recruit", greatswordAutoCost));

            Debug.Log("BattleLoopDemo ready. ManualStep=" + manualStep +
                      ". Press Space to step; 1/2/3 to set speed (when manualStep = false).");
        }

        private void Update()
        {
            if (!controller) return;

            // Manual stepping disables the controller's realtime loop.
            controller.paused = manualStep;

            if (manualStep)
            {
                if (WasSpacePressedThisFrame())
                {
                    controller.StepIncrements(Mathf.Max(1, incrementsPerSpace));
                }
            }
            else
            {
                // Adjust speed with number keys when running live.
                float? newSpeed = ReadSpeedHotkey();
                if (newSpeed.HasValue) controller.speed = newSpeed.Value;
            }
        }

        private bool WasSpacePressedThisFrame()
        {
            #if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            return Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
            #else
            return Input.GetKeyDown(KeyCode.Space);
            #endif
        }

        private float? ReadSpeedHotkey()
        {
            #if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            var kb = Keyboard.current;
            if (kb == null) return null;

            if (kb.digit1Key.wasPressedThisFrame) return 0.5f;
            if (kb.digit2Key.wasPressedThisFrame) return 1f;
            if (kb.digit3Key.wasPressedThisFrame) return 2f;
            return null;
            #else
            if (Input.GetKeyDown(KeyCode.Alpha1)) return 0.5f;
            if (Input.GetKeyDown(KeyCode.Alpha2)) return 1f;
            if (Input.GetKeyDown(KeyCode.Alpha3)) return 2f;
            return null;
            #endif
        }

        // Optional: right-click the component in Inspector → "Step One Increment".
        [ContextMenu("Step One Increment")]
        private void ContextStepOne()
        {
            if (!controller) return;
            controller.paused = true;
            controller.StepIncrements(1);
        }
    }
}
