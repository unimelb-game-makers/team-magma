// Author : Peiyu Wang @ Daphatus
// 07 01 2025 01 08

using System;
using TMPro;

namespace Narrative
{
    using UnityEngine;
    using UnityEngine.UI;
    using Ink.Runtime;

    public class DialogueUi : Singleton<DialogueUi>, IUIHandler
    {

        [Header("UI Elements")] [SerializeField]
        private TextMeshProUGUI dialogueText; // The main text field for displaying dialogue
        
        [SerializeField] private GameObject choiceButtonPrefab; // A prefab for a button that represents a choice
        [SerializeField] private GameObject dialogueUiPanel; // A prefab for a button that represents a choice
        [SerializeField] private Transform choicePanel; // A container (e.g., VerticalLayoutGroup) for choice buttons
        
        
        
        private Story _story;
        
        private void Start()
        {
            HideUI();
        }
        
        public void TalkToNpc(Story story)
        {
            SetStory(story);
            ShowUI();
        }
        
        private void SetStory(Story story)
        {
            _story = story;
            RefreshView();
        }
        
        private void RefreshView()
        {
            // Clear out previous choices
            RemoveChoiceButtons();

            dialogueText.text = ""; // Clear the text field before adding new content

            while (_story.canContinue)
            {
                var nextLine = _story.Continue().Trim();
                // Append instead of overwriting
                dialogueText.text += nextLine + "\n";
            }

            // If there are choices, create choice buttons
            if (_story.currentChoices.Count > 0)
            {
                for (int i = 0; i < _story.currentChoices.Count; i++)
                {
                    CreateChoiceButton(_story.currentChoices[i]);
                }
            }
            else
            {
                //Add a button to close the dialogue
                CreateCloseButton();

                void CreateCloseButton()
                {
                    GameObject buttonGO = Instantiate(choiceButtonPrefab, choicePanel);
                    Button buttonComponent = buttonGO.GetComponent<Button>();
                    TextMeshProUGUI buttonText = buttonGO.GetComponentInChildren<TextMeshProUGUI>();
                    buttonText.text = "Close";
                    buttonComponent.onClick.AddListener(delegate { HideUI(); });
                }
            }
        }

        private void CreateChoiceButton(Choice choice)
        {
            // Instantiate a new button
            GameObject buttonGO = Instantiate(choiceButtonPrefab, choicePanel);
            Button buttonComponent = buttonGO.GetComponent<Button>();

            // Set button text
            TextMeshProUGUI buttonText = buttonGO.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = choice.text;

            // When the button is clicked, choose this choice
            buttonComponent.onClick.AddListener(delegate { OnChoiceSelected(choice); });
        }

        private void OnChoiceSelected(Choice choice)
        {
            // Tell the Ink story which choice was selected
            _story.ChooseChoiceIndex(choice.index);
            RefreshView();
        }

        private void RemoveChoiceButtons()
        {
            // Clear out old choice buttons
            for (int i = choicePanel.childCount - 1; i >= 0; i--)
            {
                Destroy(choicePanel.GetChild(i).gameObject);
            }
        }

        public void ShowUI()
        {
            dialogueUiPanel.SetActive(true);
        }

        public void HideUI()
        {
            dialogueUiPanel.SetActive(false);
        }

        public void ToggleUI()
        {
            dialogueUiPanel.SetActive(!dialogueUiPanel.activeSelf);
        }
    }
}