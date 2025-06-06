using UnityEngine;
using System.Collections;
using Player;
using Enemy;

namespace Damage
{
    public class Damageable : MonoBehaviour
    {
        // ***************
        // Not sure if we need this, leaving for now.
        [SerializeField] private Color hitColor = Color.red;
        [SerializeField] private float hitEffectDuration = 0.5f;
        [SerializeField] private float colorChangeDuration = 0.2f;

        [SerializeField] private AudioPlayer takeDamageAudioPlayer;
        
        private Renderer rend;  // The Renderer component of the cube  // The color to change to when hit
        private Color originalColor;  // To store the original color of the cube
        // ***************
        
        private bool isInvulnerable = false;

        private IHealthManager healthManager;

        public void Awake()
        {
            rend = GetComponent<Renderer>();
            if (rend != null)
            {
                originalColor = rend.material.color;
            }

            // Get the health manager component.
            healthManager = GetComponent<IHealthManager>();
            //Debug.Log(healthManager);
            if (healthManager == null)
            {
                //Debug.LogError("No IHealthManager implementation found on the GameObject.");
            }
        }

        public void TakeDamage(float damage)
        {
            if (!isInvulnerable) {
                //Debug.Log(healthManager);
                if (takeDamageAudioPlayer != null) {
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
                    } else
                    {
                        Destroy(gameObject);
                    }
                }
                else if (rend != null)
                {
                    StartCoroutine(HitEffect());
                }
            }
        }

        private IEnumerator HitEffect()
        {
            // Gradually change the color to hitColor
            yield return StartCoroutine(ChangeColor(originalColor, hitColor, colorChangeDuration));

            // Wait for the rest of the hitEffectDuration
            yield return new WaitForSeconds(hitEffectDuration - colorChangeDuration);

            // Gradually change the color back to originalColor
            yield return StartCoroutine(ChangeColor(hitColor, originalColor, colorChangeDuration));
        }

        private IEnumerator ChangeColor(Color startColor, Color endColor, float duration)
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                rend.material.color = Color.Lerp(startColor, endColor, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            rend.material.color = endColor;  // Ensure the color is set to endColor
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