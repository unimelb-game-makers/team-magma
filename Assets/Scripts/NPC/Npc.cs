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

        [SerializeField] private string _storylets_prefix = "story_"; //JASPER WROTE THIS

        [SerializeField] private float _interactionRange = 5f;

        [SerializeField] private NpcState _currentState; public NpcState CurrentState => _currentState;
        
        [SerializeField] private Transform [] PatrolPoints;
        
        private StoryletsManager _storylets_manager = null; //JASPER WROTE THIS

        private void Start()
        {
            // Load the Ink Story
            agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            
            _story = new Story(inkJSONAsset.text);
            _storylets_prefix = _storylets_prefix + "_"; //JASPER WROTE THIS because when I'm messing around in the editor, I WILL forget to add the underscore at the end of the prefixes that the Ink script requires, so this is just for ease-of-use to save us debugging headaches
            _storylets_manager = new StoryletsManager(_story);//JASPER WROTE THIS
            _storylets_manager.AddStorylets(_storylets_prefix);//JASPER WROTE THIS
            _storylets_manager.Refresh();//JASPER WROTE THIS
        }
        
        
        private GameObject _player = null;
        private Coroutine _playerInRangeCheckCoroutine = null;
        
        /* Not working for some reason so comment it for now
        private void OnMouseDown()
        {
            InteractWithNpc();
        }
        */
        
        public void InteractWithNpc()
        {
            if (!IsPlayerInRange()) return;
            
            if (CurrentState == NpcState.Idle)
            {
                string _storylet_to_play = _storylets_manager.PickPlayableStorylet(); //JASPER WROTE THIS . This provides a KnotID string which, presumably we can pass to the Ink story to play content from?
                DialogueUi.Instance.TalkToNpc(_story, _storylet_to_play);
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

        void Update()
        {
            _storylets_manager?.Tick();//JASPER WROTE THIS

            if (CurrentState == NpcState.Walking)
            {
                Debug.Log("Patrolling");
                Patrol();
            }

            if (Input.GetMouseButtonDown(0))  // Left mouse button
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    // Get the root object of the hit collider (the NPC object)
                    GameObject rootObject = hit.collider.transform.root.gameObject;

                    // Check if the root object is this object
                    if (rootObject == gameObject)
                    {
                        InteractWithNpc();
                    }
                }
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