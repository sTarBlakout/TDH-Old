using System.Collections;
using TDH.EnemyAI;
using UnityEngine;

namespace TDH.Core
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] GameObject enemyToSpawn;

        private bool isCurrentEnemyALive = false;
        private GameObject enemyInScene = null;

        private void Start() 
        {
            StartCoroutine(CheckForEnemies());
        }

        private IEnumerator CheckForEnemies()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                if (!isCurrentEnemyALive)
                {
                    enemyInScene = Instantiate(enemyToSpawn, this.transform.position, Quaternion.identity);
                    isCurrentEnemyALive = true;
                }

                if (enemyInScene.transform.GetComponent<EnemyBehaviorAI>().GetCurrentEnemyState() == CurrentEnemyState.DEAD)
                {
                    isCurrentEnemyALive = false;
                }
            }
        }
    }
}