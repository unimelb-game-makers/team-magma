using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using Scenes;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UI
{
    public class TutorialInstructionScreenUI : Singleton<TutorialInstructionScreenUI>
    {
        private void Update()
        {
            // If the Enter key is pressed
            if (Input.GetKeyDown(KeyCode.Return))
            {
                TutorialInstructionScreenManager.Instance.ShowMoveScreen();

                // May have to remove this; test
                // Deselect the currently selected UI element
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }
}
