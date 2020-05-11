using UnityEngine.UI;
using System;
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

        private Dictionary<int, Weapon> weapons = new Dictionary<int, Weapon>();
        private Dictionary<int, Spell> spells = new Dictionary<int, Spell>();
        private Dictionary<int, Item> items = new Dictionary<int, Item>();

        [SerializeField] List<Weapon> weaponsList = new List<Weapon>();
        [SerializeField] List<Spell> spellsList = new List<Spell>();
        [SerializeField] List<Item> itemsList = new List<Item>();

        private Weapon eqquipedWeapon = null;
        private Spell eqquipedSpell = null;


        private void Start() 
        {
            FillAllDictionaries();  
            FillInventoryUI();  
        }

        public void TakeWeaponSO(Weapon weapon)
        {
            weaponsList.Add(weapon);
            FillWeaponDictionary();
        }

        public void TakeSpellSO(Spell spell)
        {
            spellsList.Add(spell);
            FillSpellDictionary();
        }

        public void TakeItemSO(Item item)
        {
            itemsList.Add(item);
            FillItemDictionary();
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

        private void FillWeaponDictionary()
        {
            int i = 1;
            foreach (Weapon weapon in weaponsList)
            {
                weapons.Add(i, weapon);
                i++;
            }
        }

        private void FillSpellDictionary()
        {
            int i = 1;
            foreach (Spell spell in spellsList)
            {
                spells.Add(i, spell);
                i++;
            }
        }

        private void FillItemDictionary()
        {
            int i = 1;
            foreach (Item item in itemsList)
            {
                items.Add(i, item);
                i++;
            }
        }

        private void FillAllDictionaries()
        {
            FillWeaponDictionary();
            FillSpellDictionary();
            FillItemDictionary();
        }

        private void FillInventoryUI()
        {
            GameObject temp;

            foreach (KeyValuePair<int, Weapon> weapon in weapons)
            {
                temp = Instantiate(ButtonPrefab, ContentWeapons.transform);
                temp.GetComponentInChildren<Text>().text = weapon.Value.name;
                temp.GetComponent<Button>().onClick.AddListener(delegate { EquipWeapon(weapon.Value); });
            }

            foreach (KeyValuePair<int, Spell> spell in spells)
            {
                temp = Instantiate(ButtonPrefab, ContentSpells.transform);
                temp.GetComponentInChildren<Text>().text = spell.Value.name;
                temp.GetComponent<Button>().onClick.AddListener(delegate { EquipSpell(spell.Value); });
            }
        }
    }
}