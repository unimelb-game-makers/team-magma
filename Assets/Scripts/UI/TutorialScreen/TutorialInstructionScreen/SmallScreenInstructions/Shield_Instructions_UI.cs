using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI {
    public class Shield_Instructions_UI : MonoBehaviour
    {
        [SerializeField] private TutorialInstructionScreenManager tutorialInstructionScreenManager;

            private bool hasChangedTape = false;

            private void Update()
            {
                if (Input.GetMouseButtonDown(1)) hasChangedTape = true;

                if (hasChangedTape)
                {
                    tutorialInstructionScreenManager.HideSmallShieldAttackScreen();

                    // Deselect the currently selected UI element
                    EventSystem.current.SetSelectedGameObject(null);
                }
            }
    }
}
