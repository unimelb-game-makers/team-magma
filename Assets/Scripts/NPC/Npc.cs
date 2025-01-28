// Author : Peiyu Wang @ Daphatus
// 28 01 2025 01 19

using System;
using System.Collections;
using Ink.Runtime;
using Narrative;
using UnityEngine;

namespace NPC
{
    [RequireComponent(typeof(Collider))]
    public class Npc : MonoBehaviour
    {
        [Header("Ink Story JSON")] [SerializeField]
        private TextAsset inkJSONAsset; // Assign the compiled LvlPocLeaderStory.json here
        
        private Story _story;
        [SerializeField] private float _interactionRange = 5f;

        [SerializeField] private NpcState _currentState; public NpcState CurrentState => _currentState;
        
        [SerializeField] private Transform [] PatrolPoints;
        
        private void Start()
        {
            // Load the Ink Story
            agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            
            _story = new Story(inkJSONAsset.text);
        }
        
        
        private GameObject _player = null;
        private Coroutine _playerInRangeCheckCoroutine = null;
        
        private void OnMouseDown()
        {
            InteractWithNpc();
        }
        
        public void InteractWithNpc()
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


        #region Ai

        private UnityEngine.AI.NavMeshAgent agent;

        public void Update()
        {
            
            if (CurrentState == NpcState.Walking)
            {
                Debug.Log("Patrolling");
                Patrol();
            }
        }

        public void Patrol()
        {
            if (PatrolPoints.Length == 0) return;
            if (agent.remainingDistance < 0.5f)
            {
                agent.destination = PatrolPoints[UnityEngine.Random.Range(0, PatrolPoints.Length)].position;
            }
        }

        #endregion
    }

    public enum NpcState
    {
        Idle,
        Talking,
        Walking
    }
}