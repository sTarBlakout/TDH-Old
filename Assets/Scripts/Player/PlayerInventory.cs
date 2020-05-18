using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TDH.Combat;
using TDH.Environment;

namespace TDH.Player
{
    public class PlayerInventory : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] GameObject ContentWeapons;
        [SerializeField] GameObject ContentSpells;
        [SerializeField] GameObject ButtonPrefab;

        public Action<Weapon> OnWeaponEquip;
        public Action<Spell> OnSpellEquip;

        [Header("Inventory")]
        [SerializeField] List<Weapon> weaponsList = new List<Weapon>();
        [SerializeField] List<Spell> spellsList = new List<Spell>();
        [SerializeField] List<Item> itemsList = new List<Item>();

        private Weapon eqquipedWeapon = null;
        private Spell eqquipedSpell = null;


        private void Start() 
        { 
            FillInventoryUI();  
        }

        public void RemoveItem(Item item)
        {   
            itemsList.Remove(item);
        }

        public void RemoveSpell(Spell spell)
        {   
            spellsList.Remove(spell);
        }

        public void RemoveWeapon(Weapon weapon)
        {   
            weaponsList.Remove(weapon);
        }

        public Item GetKey(int id)
        {
            return itemsList.Find(item => item.UniqueKeyCode == id);
        }

        public void TakeWeaponSO(Weapon weapon)
        {
            weaponsList.Add(weapon);
            FillInventoryUI();
        }

        public void TakeSpellSO(Spell spell)
        {
            spellsList.Add(spell);
            FillInventoryUI();
        }

        public void TakeItemSO(Item item)
        {
            itemsList.Add(item);
            FillInventoryUI();
        }

        public Weapon GetWeapon(string name)
        {
            return weaponsList.Find( weapon => weapon.name == name );
        }

        private void EquipWeapon(Weapon weapon)
        {
            if (eqquipedWeapon == weapon)
            {
                Debug.Log("You try to equip same weapon!");
                return;
            }
            eqquipedWeapon = weapon;
            OnWeaponEquip(eqquipedWeapon);
        }

        private void EquipSpell(Spell spell)
        {
            if (eqquipedSpell == spell)
            {
                Debug.Log("You try to equip same spell!");
                return;
            }
            eqquipedSpell = spell;
            OnSpellEquip(eqquipedSpell);
        }

        private void FillInventoryUI()
        {
            foreach (Transform child in ContentWeapons.transform)
                Destroy(child.gameObject);

            foreach (Transform child in ContentSpells.transform)
                Destroy(child.gameObject);

            GameObject temp;

            foreach (Weapon weapon in weaponsList)
            {
                temp = Instantiate(ButtonPrefab, ContentWeapons.transform);
                temp.GetComponentInChildren<Text>().text = weapon.name;
                temp.GetComponent<Button>().onClick.AddListener(delegate { EquipWeapon(weapon); });
            }

            foreach (Spell spell in spellsList)
            {
                temp = Instantiate(ButtonPrefab, ContentSpells.transform);
                temp.GetComponentInChildren<Text>().text = spell.name;
                temp.GetComponent<Button>().onClick.AddListener(delegate { EquipSpell(spell); });
            }
        }
    }
}