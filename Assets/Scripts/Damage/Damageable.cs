using UnityEngine;
using Player;
using Enemy;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;

namespace Damage
{
    public class Damageable : MonoBehaviour
    {
        // ***************
        // Not sure if we need this, leaving for now.
        [SerializeField] private Color hitColor = Color.red;
        [SerializeField] private float hitEffectDuration = 0.5f;
        [SerializeField] private float colorChangeDuration = 0.2f;
        [SerializeField] private Renderer[] renderers;

        [SerializeField] private AudioPlayer takeDamageAudioPlayer;
        
        private List<Color> originalColor;  // To store the original color of the cube
        // ***************
        [SerializeField] private GameObject HitEffectPrefab;

        private bool isInvulnerable = false;
        private Sequence _sequence;
        private IHealthManager healthManager;
        private const string ColorProp = "_BaseColor";  
        public void Awake()
        {
            originalColor = new();
            foreach (Renderer rend in renderers)
            {

                originalColor.Add(rend.material.color);

            }
            _sequence = DOTween.Sequence();


            // Get the health manager component.
            healthManager = GetComponent<IHealthManager>();
            //Debug.Log(healthManager);
            if (healthManager == null)
            {
                Debug.LogError("No IHealthManager implementation found on the GameObject.");
            }
        }

        public void TakeDamage(float damage)
        {
            if (!isInvulnerable)
            {
                //Debug.Log(healthManager);
                if (takeDamageAudioPlayer != null)
                {
                    takeDamageAudioPlayer.Play();
                }

                healthManager.TakeDamage(damage);

                if (healthManager.IsDead())
                {
                    //if is player character, dont destroy
                    if (gameObject.GetComponent<PlayerCharacter>() != null)
                    {
                        //Debug.Log("Player character is dead");
                        // Do nothing, player character should not be destroyed

                        // To add: A Die() method in player that just returns for now.
                    }
                    else if (gameObject.GetComponent<EnemyController>())
                    {
                        // if it is an enemy, call the enemy die method.
                        gameObject.GetComponent<EnemyController>().Die();
                    }
                    else
                    {
                        Destroy(gameObject);
                    }
                }
                else
                {
                    _sequence.Kill();

                    for (int i = 0; i < renderers.Count(); i++)
                    {
                        var mat = renderers[i].material;

                        DOTween.Kill(mat);

                        // Make a per-material flash sequence: red -> original
                        var flash = DOTween.Sequence()
                            .Append(mat.DOColor(hitColor, ColorProp, colorChangeDuration))
                            .Append(mat.DOColor(originalColor[i], ColorProp, colorChangeDuration));

                    }

                }
                if (HitEffectPrefab != null)
                {
                    GameObject effect = Instantiate(HitEffectPrefab, transform);
                    Destroy(effect, hitEffectDuration);

                }

            }
        }



        public void setIsInvulnerable(bool isInvuln)
        {
            isInvulnerable = isInvuln;
        }
        public bool getIsInvulnerable()
        {
            return isInvulnerable;
        }
    }
}