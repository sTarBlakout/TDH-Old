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

        public Item GetKey(int id)
        {
            return itemsList.Find(item => item.UniqueKeyCode == id);
        }

        public void TakeWeaponSO(Weapon weapon)
        {
            weaponsList.Add(weapon);
        }

        public void TakeSpellSO(Spell spell)
        {
            spellsList.Add(spell);
        }

        public void TakeItemSO(Item item)
        {
            itemsList.Add(item);
        }

        public Weapon GetWeapon(string name)
        {
            foreach (Weapon weapon in weaponsList)
            {
                if (weapon.name == name)
                {
                    return weapon;
                }
            }

            return null;
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