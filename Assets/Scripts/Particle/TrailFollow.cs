using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDH.Particles 
{
    public class TrailFollow : MonoBehaviour
    {
        [SerializeField] Transform transformToFollow = null;

        private void Start() 
        {
            if (transformToFollow == null)
            {
                transformToFollow = transform.parent;
            }
            this.transform.parent = null;    
        }

        private void Update() 
        {
            if (transformToFollow != null)    
            {
                this.transform.position = transformToFollow.position;
            }
        }
    }
}