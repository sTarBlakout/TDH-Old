using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDH.UI
{
    public class InventoryButton : MonoBehaviour
    {
        [SerializeField] GameObject InventoryPanel;

        public void ActivateInventoryPanel(bool activate)
        {
            InventoryPanel.SetActive(activate);
        }
    }
}