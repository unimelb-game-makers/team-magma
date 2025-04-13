// Author : Peiyu Wang @ Daphatus
// 08 04 2025 04 45

using System;
using Scenes;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UI
{
    public class DefeatMenuUI : Singleton<DefeatMenuUI>
    {
        [SerializeField] private GameObject continueButton;
        [SerializeField] private GameObject settingsButton;
        [SerializeField] private GameObject quitButton;
        
        //bind the buttons to the functions
        private void Start()
        {
            continueButton.GetComponent<Button>().onClick.AddListener(OnContinueButtonClicked);
            settingsButton.GetComponent<Button>().onClick.AddListener(OnSettingsButtonClicked);
            quitButton.GetComponent<Button>().onClick.AddListener(OnQuitButtonClicked);
        }

        public void OnContinueButtonClicked()
        {
            GameManager.Instance.ReloadLevel();
            SceneFadeManager.Instance.StartCoroutine(SceneFadeManager.Instance.FadeIn());
            PauseManager.ResumeGame();

            // Deselect the currently selected UI element
            EventSystem.current.SetSelectedGameObject(null);
        }
        private void OnSettingsButtonClicked()
        {
            SettingsManager.Instance.OpenSettings();

            // Deselect the currently selected UI element
            EventSystem.current.SetSelectedGameObject(null);
        }
        private void OnQuitButtonClicked()
        {
            StartMenuManager.Instance.OpenStartMenu();
            DefeatScreenManager.Instance.HideDefeatScreen();

            // Deselect the currently selected UI element
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}