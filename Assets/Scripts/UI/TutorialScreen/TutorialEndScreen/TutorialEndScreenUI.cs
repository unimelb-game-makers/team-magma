using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using Scenes;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UI
{
    public class TutorialEndScreenUI : MonoBehaviour
    {
        [SerializeField] private TutorialEndScreenManager tutorialEndScreenManager;
        [SerializeField] private GameObject quitButton;
        
        //bind the buttons to the functions
        private void Start()
        {
            quitButton.GetComponent<Button>().onClick.AddListener(OnQuitButtonClicked);
        }
    
        private void OnQuitButtonClicked()
        {
            StartMenuManager.Instance.OpenStartMenu();
            tutorialEndScreenManager.HideTutorialEndScreen();

            // Deselect the currently selected UI element
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
