using UnityEngine.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using TDH.Combat;

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

        [SerializeField] Weapon[] weaponsArray;
        [SerializeField] Spell[] spellsArray;

        private Weapon eqquipedWeapon = null;
        private Spell eqquipedSpell = null;


        private void Start() 
        {
            FillDictionaries();  
            FillInventoryUI();  
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

        private void FillDictionaries()
        {
            int i = 1;

            foreach (Weapon weapon in weaponsArray)
            {
                weapons.Add(i, weapon);
                i++;
            }

            i = 1;

            foreach (Spell spell in spellsArray)
            {
                spells.Add(i, spell);
                i++;
            }
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