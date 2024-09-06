using UnityEngine;
using System.Collections;

namespace Damage
{
    public class Damageable : DamageComponent
    {
        [SerializeField] private int health = 100;
        [SerializeField] private string tag = "Player";
        [SerializeField] private Color hitColor = Color.red;
        [SerializeField] private float hitEffectDuration = 0.5f;
        [SerializeField] private float colorChangeDuration = 0.2f;
        
        private Renderer rend;  // The Renderer component of the cube  // The color to change to when hit
        private Color originalColor;  // To store the original color of the cube

        private bool isInvulnerable = false;

        public void Start()
        {
            rend = GetComponent<Renderer>();
            originalColor = rend.material.color;
        }

        public void ApplyDamage(int damage)
        {
            if (!isInvulnerable) {
                health -= damage;
                Debug.Log(gameObject.name + " takes damage " + damage + "; Remain health: " + health);
                if (health <= 0)
                {
                    Debug.Log("Object dies");
                    Destroy(gameObject);
                } else
                {
                    StartCoroutine(HitEffect());
                }
            }
        }
        /**
         * To differentiate enemy or player
         */
        public string GetTag()
        {
            return tag;
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