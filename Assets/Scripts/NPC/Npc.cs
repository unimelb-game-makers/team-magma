// Author : Peiyu Wang @ Daphatus
// 28 01 2025 01 19

using System;
using Ink.Runtime;
using Narrative;
using UnityEngine;

namespace NPC
{
    [RequireComponent(typeof(Collider))]
    public class Npc : MonoBehaviour
    {
        [SerializeField] private NpcState _state;
        [Header("Ink Story JSON")] [SerializeField]
        private TextAsset inkJSONAsset; // Assign the compiled LvlPocLeaderStory.json here
        
        private Story _story;

        private NpcState _currentState = NpcState.Idle; public NpcState CurrentState => _currentState;
        
        private void Start()
        {
            // Load the Ink Story
            _story = new Story(inkJSONAsset.text);
        }
        
        private void OnMouseDown()
        {
            if (CurrentState == NpcState.Idle)
            {
                DialogueUi.Instance.TalkToNpc(_story);
            }
        }
        
    }

    public enum NpcState
    {
        Idle,
        Talking,
        Walking
    }
}