using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RockWallBehavior : MonoBehaviour
{
    private List<Transform> rocks = new List<Transform>();

    void Start()
    {
        foreach (Transform rock in transform)  
        {     
            Rigidbody rb = rock.gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rocks.Add(rock);
        }
    
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.CompareTag("Player"))    
        {
            foreach (Transform rock in rocks)
            {
                rock.parent = null;
                Rigidbody rb = rock.GetComponent<Rigidbody>();
                BoxCollider bc = rock.gameObject.AddComponent<BoxCollider>();
                bc.size = new Vector3(bc.size.x * 0.6f, bc.size.y * 0.6f, bc.size.z * 0.6f);
                rb.useGravity = true;
                rb.AddForce(transform.right * Random.Range(200f, 400f));
                transform.GetComponent<NavMeshObstacle>().enabled = false;
                transform.GetComponent<BoxCollider>().enabled = false;
            }
        }
    }
}
