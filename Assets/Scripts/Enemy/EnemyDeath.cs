using System.Collections;
using TDH.Environment;
using UnityEngine;

namespace TDH.EnemyAI
{
    public class EnemyDeath : MonoBehaviour
    {
        [SerializeField] GameObject particlePrefab = null;
        [SerializeField] GameObject liquidPrefab = null;
        [SerializeField] float timeBeforeAnyChanges = 1f;
        [SerializeField] float timeBeforeStartDisappear = 1f;
        [SerializeField] float disappearingTime = 1f;
        [SerializeField] float changeStep = 10;
        [SerializeField] float goingUnderGroundStep = 0.1f;

        [Header("Loot")]
        [SerializeField] GameObject lootPointPref;
        [SerializeField] ScriptableObject[] lootOnDeath;

        private GameObject liquidParticle;
        private float timeToParticleStop;
        private float timeToDisappear;
        private Coroutine dyingMoveCoroutine = null;
        private int iteration = 0;

        void Start()
        {
            timeToParticleStop = Time.time + disappearingTime;
            timeToDisappear = Time.time + disappearingTime + 2f;
            StartCoroutine(DyingParticleBehavior());
        }

        private IEnumerator DyingParticleBehavior()
        {
            yield return new WaitForSeconds(timeBeforeAnyChanges);
            liquidParticle = Instantiate(liquidPrefab, this.transform);
            liquidParticle.transform.parent = null;
            yield return new WaitForSeconds(timeBeforeStartDisappear);
            GameObject particle = Instantiate(particlePrefab, this.transform);
            particle.transform.parent = null;
            ParticleSystem system = particle.transform.GetComponent<ParticleSystem>();
            ParticleSystem.EmissionModule emission = system.emission;
            float step = 1;
            while (Time.time < timeToParticleStop)
            {
                emission.rateOverTime = step;
                step += changeStep;
                iteration++;
                yield return new WaitForSeconds(0.5f);
                if (dyingMoveCoroutine == null && iteration == 2)
                {
                    dyingMoveCoroutine = StartCoroutine(DyingMove());
                }
            }
            yield return new WaitForSeconds(0.2f);
            system.Stop();
        }

        private IEnumerator DyingMove()
        {
            while (Time.time < timeToDisappear)
            {
                this.transform.position -= new Vector3(0f, goingUnderGroundStep, 0f);
                yield return new WaitForSeconds(0.1f);
            }
            liquidParticle.transform.GetComponent<ParticleSystem>().Stop();
            SpawnLootPoint();
            Destroy(this.gameObject);
        }

        private void SpawnLootPoint()
        {
            if (lootPointPref == null || lootOnDeath.Length == 0) return;

            GameObject lootPoint = Instantiate(lootPointPref, this.transform);
            lootPoint.GetComponent<LootPointBehavior>().SetLoot(lootOnDeath[0]);
            lootPoint.transform.parent = null;
            lootPoint.transform.position = new Vector3(
                lootPoint.transform.position.x,
                lootPoint.transform.position.y + 0.7f,
                lootPoint.transform.position.z);
        }
    }
}