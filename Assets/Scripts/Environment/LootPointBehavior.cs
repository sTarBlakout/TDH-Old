using System.Collections;
using UnityEngine;
using TDH.Combat;
using TDH.Player;

namespace TDH.Environment
{
    public class LootPointBehavior : MonoBehaviour
    {
        private const float secondsToDestroy = 1f;
        private const float rotateStep = 360f;
        private const float upDownSpeed = 1.2f;

        private PlayerInventory player;

        private GameObject takeButton;

        [Header("Loot Info")]
        [SerializeField] ScriptableObject lootSO = null;

        [Header("Loot Point Behavior")]
        [SerializeField] float rotateSpeed;
        [SerializeField] float height;

        private bool isLootTaken = false;
        private Transform lootPoint;
        private GameObject instantiatedView;

        private void Awake() 
        {
            lootPoint = this.transform.GetChild(1);
            takeButton = transform.Find("Canvas").GetChild(0).gameObject;
        }

        void Start()
        {
            takeButton.SetActive(false);
            if (GetViewForLootToShow() != null)
                instantiatedView = Instantiate(GetViewForLootToShow(), lootPoint);
            LeanTween.moveY(lootPoint.gameObject, height, upDownSpeed).setLoopPingPong();
            LeanTween.rotateAround(lootPoint.gameObject, Vector3.up, rotateStep, rotateSpeed).setLoopClamp();
        }

        public void SetLoot(ScriptableObject loot)
        {
            if (lootSO != null) return;

            lootSO = loot;
            instantiatedView = Instantiate(GetViewForLootToShow(), lootPoint);
        }

        //Called by UI button
        public void PlayerTakeItem()
        {
            if (player == null) return;
            player.gameObject.GetComponent<Animator>().SetTrigger("PickUp");
            player.TakeLootSO(lootSO);
            StartCoroutine(ItemPickedUpCoroutine());
        }

        private IEnumerator ItemPickedUpCoroutine()
        {
            takeButton.SetActive(false);
            Destroy(instantiatedView);
            this.GetComponent<ParticleSystem>().Stop();
            yield return new WaitForSeconds(secondsToDestroy);
            Destroy(this.gameObject);
        }

        private GameObject GetViewForLootToShow()
        {
            if (lootSO is Weapon)
                return ((Weapon)lootSO).LootPointViewPref;
            else if (lootSO is Spell)
                return ((Spell)lootSO).LootPointViewPref;
            else if (lootSO is Item)
                return ((Item)lootSO).LootPointViewPref;
            else
                return null;
        }

        private void OnTriggerEnter(Collider other) 
        {
            if (other.gameObject.CompareTag("Player"))    
            {
                player = other.gameObject.GetComponent<PlayerInventory>();
                takeButton.transform.LookAt(Camera.main.transform);
                takeButton.SetActive(true);
            }
        }

        private void OnTriggerExit(Collider other) 
        {
            if (other.gameObject.CompareTag("Player"))    
            {
                takeButton.SetActive(false);
            }
        }
    }
}