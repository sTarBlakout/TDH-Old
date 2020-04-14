using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDH.Stats
{
    public class Health : MonoBehaviour
    {
        [SerializeField] float maxHealth = 100f;

        public Action OnDie, OnHit;

        private bool isDead = false;

        private float currentHealth = 100f;

        private void Start() 
        {
            currentHealth = maxHealth;
        }

        public void DecreaseHealth(float value)
        {
            if (isDead) return;

            currentHealth -= value;

            if (currentHealth > 0f)
            {
                OnHit();
            }
            else
            {
                Die();
            }
        }

        private void Die()
        {
            isDead = true;
            OnDie();
        }
    }
}