using System.Collections;
using UnityEngine;

namespace TDH.Particles 
{
    public class DestoryParticle : MonoBehaviour
    {
        [SerializeField] float timeToStop = 0f;

        private bool coroutineStarted = false;
        private bool isParticleStopped = false;

        private float spawnTime;

        private void Start() 
        {
            spawnTime = Time.time;
        }

        void Update()
        {
            if (timeToStop != 0f)
            {
                if (Time.time >= timeToStop + spawnTime && !isParticleStopped)
                {
                    isParticleStopped = true;
                    transform.GetComponent<ParticleSystem>().Stop();
                }
            }
            if (transform.GetComponent<ParticleSystem>().isStopped == true && !coroutineStarted)
            {
                coroutineStarted = true;
                StartCoroutine(DestroyParticle());
            }
        }

        private IEnumerator DestroyParticle()
        {
            yield return new WaitForSeconds(1f);
            Destroy(this.gameObject);
        }
    }
}
