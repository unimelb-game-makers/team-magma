// Author : Peiyu Wang @ Daphatus
// 07 01 2025 01 08

using System;
using TMPro;
using System.Collections;
using System.Collections.Generic;

namespace Narrative
{
    using UnityEngine;
    using UnityEngine.UI;
    using Ink.Runtime;

    public class DialogueUI : Singleton<DialogueUI>, IUIHandler
    {

        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI dialogueText; // The main text field for displaying dialogue
        [SerializeField] private TextMeshProUGUI characterNameText; // The text field for displaying character name
        
        [SerializeField] private GameObject choiceButtonPrefab; // A prefab for a button that represents a choice
        [SerializeField] private GameObject dialogueUiPanel; // A prefab for a button that represents a choice
        [SerializeField] private Transform choicePanel; // A container (e.g., VerticalLayoutGroup) for choice buttons

        [SerializeField] private int charactersPerPage = 250;
        [SerializeField] private int maxLinesPerPage = 5;
        [SerializeField] private float typingSpeed = 0.01f;

        [SerializeField] private GameObject nextIndicator;
        [SerializeField] private GameObject closeIndicator;

        private string[] pages;
        private int currentPage = 0;

        private bool isTyping = false;  // Flag to track whether text is still being typed
        private Coroutine currentCoroutine;
        
        private Story _story;
        
        private void Start()
        {
            HideUI();
        }
        
        public void Talk(Story story, string knot) //JASPER WROTE THIS
        {
            // default name
            characterNameText.text = "???";

            SetStory(story, knot);
            ShowUI();
        }
        
        private void SetStory(Story story, string knot)
        {
            story.ChoosePathString(knot); //JASPER WROTE THIS . This is a knot provided by the StoryletsManager to start with; it has been selected by that manager as an appropriate piece of content to display to the player, as dictated by me (Jasper!) in the ink script
            _story = story;
            RefreshView();
        }
        
        private void RefreshView()
        {
            // Clear out previous choices
            RemoveChoiceButtons();
            nextIndicator.SetActive(false);
            closeIndicator.SetActive(false);

            dialogueText.text = ""; // Clear the text field before adding new content

            if (!_story.canContinue)
            {
                return;
            }

            // Load the entire dialogue into one string
            string completeDialogue = "";
            
            while (_story.canContinue)
            {
                var nextLine = _story.Continue().Trim();
                completeDialogue += nextLine + "\n";
            }

            // Split the complete dialogue into pages based on character limit
            pages = SplitIntoPages(completeDialogue);
            currentPage = 0;
            if (pages.Length > 0)
            {
                ShowCurrentPage();
            }
            else
            {
                HideUI();
            }
            
        }

        private void DisplayChoices()
        {
            if (_story.currentChoices.Count > 0 && (pages.Length - 1) == currentPage)
            {
                for (int i = 0; i < _story.currentChoices.Count; i++)
                {
                    CreateChoiceButton(_story.currentChoices[i]);
                }
            }
            else if ((pages.Length - 1) > currentPage)
            {
                nextIndicator.SetActive(true);
            }
            else
            {
                closeIndicator.SetActive(true);
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
            nextIndicator.SetActive(false);
            closeIndicator.SetActive(false);
            dialogueUiPanel.SetActive(false);
        }

        public void ToggleUI()
        {
            dialogueUiPanel.SetActive(!dialogueUiPanel.activeSelf);
        }

        private string[] SplitIntoPages(string text)
        {
            List<string> result = new List<string>();
            List<string> currentPageLines = new List<string>();
            string currentSpeaker = "???";
            string nextSpeaker = "???";
            int charactersInCurrentPage = 0;

            string[] lines = text.Split('\n');
            
            foreach (string line in lines)
            {
                // Skip empty lines
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                // Check for speaker tag
                bool isNewSpeaker = false;
                string content = line;
                
                Debug.Log(_story.currentTags);

                // Check if this line has a speaker tag
                foreach (string tag in _story.currentTags)
                {
                    if (tag.StartsWith("speaker:"))
                    {
                        nextSpeaker = tag.Substring(8).Trim();
                        Debug.Log(nextSpeaker);
                        if (nextSpeaker != currentSpeaker)
                        {
                            isNewSpeaker = true;
                        }
                        break;
                    }
                }

                // If speaker changed and we have content, start new page
                if (isNewSpeaker && currentPageLines.Count > 0)
                {
                    result.Add(currentSpeaker + "|" + string.Join("\n", currentPageLines));
                    currentPageLines.Clear();
                    charactersInCurrentPage = 0;
                }

                currentSpeaker = nextSpeaker;

                // Handle line wrapping
                if (line.Length > charactersPerPage)
                {
                    List<string> wrappedLines = WrapLongLine(line);
                    foreach (var wrappedLine in wrappedLines)
                    {
                        if (charactersInCurrentPage + wrappedLine.Length > charactersPerPage ||
                            currentPageLines.Count >= maxLinesPerPage)
                        {
                            result.Add(currentSpeaker + "|" + string.Join("\n", currentPageLines));
                            currentPageLines.Clear();
                            charactersInCurrentPage = 0;
                        }

                        currentPageLines.Add(wrappedLine);
                        charactersInCurrentPage += wrappedLine.Length + 1;
                    }
                }
                else
                {
                    if (charactersInCurrentPage + line.Length > charactersPerPage ||
                        currentPageLines.Count >= maxLinesPerPage)
                    {
                        result.Add(currentSpeaker + "|" + string.Join("\n", currentPageLines));
                        currentPageLines.Clear();
                        charactersInCurrentPage = 0;
                    }

                    currentPageLines.Add(line);
                    charactersInCurrentPage += line.Length + 1;
                }
            }

            // Add final page
            if (currentPageLines.Count > 0)
            {
                
                result.Add(currentSpeaker + "|" + string.Join("\n", currentPageLines));
            }

            return result.ToArray();
        }

        // Helper method to wrap long lines into smaller parts
        private List<string> WrapLongLine(string longLine)
        {
            List<string> wrappedLines = new List<string>();

            // Break the long line into smaller chunks that fit within the max allowed character count
            for (int i = 0; i < longLine.Length; i += charactersPerPage)
            {
                int length = (int) Mathf.Min(charactersPerPage, longLine.Length - i);
                wrappedLines.Add(longLine.Substring(i, length));
            }

            return wrappedLines;
        }

        private void ShowCurrentPage()
        {
            nextIndicator.SetActive(false);
            closeIndicator.SetActive(false);

            // Update speaker name before showing text
            string textWithoutSpeaker = UpdateSpeakerForCurrentPage(pages[currentPage]);
            
            StartTyping(textWithoutSpeaker);
            DisplayChoices();
        }

        private string UpdateSpeakerForCurrentPage(string text)
        {
            Debug.Log(text);
            // Reset speaker at start of each page
            characterNameText.text = "???";

            if (text.Contains("|"))
            {
                Debug.Log("hihih");
                string characterName = text.Substring(0, text.IndexOf("|")).Trim();
                characterNameText.text = characterName;
                return text.Substring(text.IndexOf("|") + 1).Trim();
            }
            return text;
        }

        // Next Page Button
        public void NextPage()
        {
            if (nextIndicator.activeSelf)
            {
                currentPage++;
                ShowCurrentPage();
            }
            else if (closeIndicator.activeSelf)
            {
                HideUI();
            }
        }

        // Method to start typing the text
        public void StartTyping(string text)
        {
            if (isTyping)
            {
                // If already typing, stop the current typing coroutine and show full text
                StopCoroutine(currentCoroutine);
                dialogueText.text = text;  // Show the full text immediately
                isTyping = false;  // Set the flag to false since the text is fully revealed
            }
            else
            {
                // Start the typewriter effect
                currentCoroutine = StartCoroutine(TypeText(text));
            }
        }

        // Coroutine for typewriter effect
        private IEnumerator TypeText(string text)
        {
            isTyping = true;  // Set the flag to true when typing starts
            dialogueText.text = "";  // Clear the text initially

            foreach (char letter in text)
            {
                dialogueText.text += letter;  // Add one letter at a time
                yield return new WaitForSeconds(typingSpeed);
            }

            isTyping = false;  // Set the flag to false when typing is complete
        }

        // Method to handle player clicking during typing
        public void OnPlayerClick()
        {
            

            if (isTyping)
            {
                // Stop the typing coroutine and show the full text
                StopCoroutine(currentCoroutine);

                // Update speaker name before showing text
                string textWithoutSpeaker = UpdateSpeakerForCurrentPage(pages[currentPage]);

                dialogueText.text = textWithoutSpeaker;  // Show the full text of the current page
                isTyping = false;
            }
            else
            {
                // Proceed to the next page or handle any other logic when not typing
                NextPage();
            }
        }
    }
}