using System.Collections;
using TDH.EnemyAI;
using TDH.Player;
using TDH.Stats;
using UnityEngine;

namespace TDH.Combat
{
    public class DamageAreaBehavior : MonoBehaviour
    {
        [SerializeField] float timeToDestroy = 0f;

        private void Start() 
        {
            StartCoroutine(DestroyByTime());
        }

        private IEnumerator DestroyByTime()
        {
            yield return new WaitForSeconds(timeToDestroy);
            Destroy(this.gameObject);
        }

        private void OnTriggerEnter(Collider other) 
        {
            if(other.gameObject.CompareTag("Enemy"))    
            {
                Weapon playerWeapon = GameObject.FindWithTag("Player").GetComponent<PlayerFighter>().GetCurrentWeapon();
                Vector3 dir = other.transform.position - this.transform.position;
                other.gameObject.transform.GetComponent<IEnemy>().SetHitVelocity(dir.normalized, playerWeapon.GetPowerfullHit());
                other.gameObject.transform.GetComponent<Health>().DecreaseHealth(playerWeapon.GetPowerfullDamage());
                other.gameObject.transform.GetComponent<EnemyBehaviorAI>().SetLastHitPosition(this.transform.position);
            }
        }
    }
}