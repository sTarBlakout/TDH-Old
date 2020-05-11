using UnityEngine;
using TDH.Combat;
using TDH.Player;

namespace TDH.Environment
{
    public class LootPointBehavior : MonoBehaviour
    {
        private const float rotateStep = 360f;
        private const float upDownSpeed = 1.2f;

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

        private void Awake() 
        {
            lootPoint = this.transform.GetChild(1);
            takeButton = transform.Find("Canvas").GetChild(0).gameObject;
            viewToInstantiate = GetViewForLootToShow();
        }

        void Start()
        {
            takeButton.SetActive(false);
            Instantiate(viewToInstantiate, lootPoint);
            LeanTween.moveY(lootPoint.gameObject, height, upDownSpeed).setLoopPingPong();
            LeanTween.rotateAround(lootPoint.gameObject, Vector3.up, rotateStep, rotateSpeed).setLoopClamp();
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