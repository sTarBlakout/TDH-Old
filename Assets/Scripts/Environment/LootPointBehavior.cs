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
        [SerializeField] LootType type;
        [SerializeField] Item itemSO;
        [SerializeField] Weapon weaponSO;
        [SerializeField] Spell spellSO;

        [Header("Loot Point Behavior")]
        [SerializeField] float rotateSpeed;
        [SerializeField] float height;

        private bool isLootTaken = false;
        private Transform lootPoint;
        private GameObject viewToInstantiate;
        private GameObject instantiatedView;

        private void Awake() 
        {
            lootPoint = this.transform.GetChild(1);
            takeButton = transform.Find("Canvas").GetChild(0).gameObject;
            viewToInstantiate = GetViewForLootToShow();
        }

        void Start()
        {
            takeButton.SetActive(false);
            instantiatedView = Instantiate(viewToInstantiate, lootPoint);
            LeanTween.moveY(lootPoint.gameObject, height, upDownSpeed).setLoopPingPong();
            LeanTween.rotateAround(lootPoint.gameObject, Vector3.up, rotateStep, rotateSpeed).setLoopClamp();
        }

        //Called by UI button
        public void PlayerTakeItem()
        {
            if (player == null) return;
            switch (type)
            {
                case LootType.ITEM:
                    player.TakeItemSO(itemSO);
                    break;
                case LootType.WEAPON:
                    player.TakeWeaponSO(weaponSO);
                    break;
                case LootType.SPELL:
                    player.TakeSpellSO(spellSO);
                    break;
                default:
                    break;
            }
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
            switch (type)
            {
                case LootType.ITEM:
                    return itemSO.LootPointViewPref;
                case LootType.WEAPON:
                    return weaponSO.LootPointViewPref;
                case LootType.SPELL:
                    return spellSO.LootPointViewPref;
                default:
                    return null;
            }
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

    public enum LootType
    {
        WEAPON,
        SPELL,
        ITEM
    }
}