// Author : Peiyu Wang @ Daphatus
// 23 03 2025 03 48

using UnityEngine;

namespace System.Level
{
    [RequireComponent(typeof(Collider))]
    public class SceneLoader : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                GameManager.Instance.LoadNextLevel();
            }
        }
    }
}