using UnityEngine;

namespace TDH.Environment
{
    [CreateAssetMenu(fileName = "Item", menuName = "Items/New Item", order = 0)]
    public class Item : ScriptableObject 
    {
        [Header("Important")]
        [SerializeField] ItemType type;
        [SerializeField] GameObject lootPointViewPref;

        [Header("Gate Key")]
        [SerializeField] int uniqueKeyCode;

        public GameObject LootPointViewPref { get => lootPointViewPref; }
        public int UniqueKeyCode { get => uniqueKeyCode; }
    }

    public enum ItemType
    {
        GATE_KEY
    }
}