using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Scenes;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UI
{
    public class SuccessScreenUI : Singleton<SuccessScreenUI>
    {
        [SerializeField] private GameObject continueButton;
        [SerializeField] private GameObject quitButton;
        
        //bind the buttons to the functions
        private void Start()
        {
            continueButton.GetComponent<Button>().onClick.AddListener(OnContinueButtonClicked);
            quitButton.GetComponent<Button>().onClick.AddListener(OnQuitButtonClicked);
        }
    
        public void OnContinueButtonClicked()
        {
            GameManager.Instance.LoadNextLevel();
            SuccessScreenManager.Instance.HideSuccessScreen();
            PauseManager.ResumeGame();

            // Deselect the currently selected UI element
            EventSystem.current.SetSelectedGameObject(null);
        }
        private void OnQuitButtonClicked()
        {
            StartMenuManager.Instance.OpenStartMenu();
            SuccessScreenManager.Instance.HideSuccessScreen();

            // Deselect the currently selected UI element
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
