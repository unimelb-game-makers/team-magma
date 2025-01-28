// Author : Peiyu Wang @ Daphatus
// 28 01 2025 01 19

using System;
using System.Collections;
using Ink.Runtime;
using Narrative;
using Player;
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
        [SerializeField] private float _interactionRange = 5f;

        private NpcState _currentState = NpcState.Idle; public NpcState CurrentState => _currentState;
        
        private void Start()
        {
            // Load the Ink Story
            _story = new Story(inkJSONAsset.text);
        }
        
        
        
        
        private GameObject _player = null;
        private Coroutine _playerInRangeCheckCoroutine = null;
        
        private void OnMouseDown()
        {
            if (!IsPlayerInRange()) return;
            
            if (CurrentState == NpcState.Idle)
            {
                DialogueUi.Instance.TalkToNpc(_story);
            }
            
            _playerInRangeCheckCoroutine = StartCoroutine(PlayerInRangeCheck());
        }

        private bool IsPlayerInRange()
        {
            if(_player == null) _player = GameObject.FindGameObjectWithTag("Player");
            //check distance 
            return Vector3.Distance(_player.transform.position, transform.position) < _interactionRange;
        }
        
        
        /// <summary>
        /// Coroutine to check if player is in range
        /// </summary>
        /// <returns></returns>
        private IEnumerator PlayerInRangeCheck()
        {
            while (true)
            {
                if (!IsPlayerInRange())
                {
                    StopCoroutine(_playerInRangeCheckCoroutine);
                    _playerInRangeCheckCoroutine = null;
                    DialogueUi.Instance.HideUI();
                    yield break;
                }
                yield return new WaitForSeconds(1f);
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