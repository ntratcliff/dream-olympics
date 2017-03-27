using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OopPlayer : MinigameBahaviour
{
    public bool IsAlive = true;

    public GameObject Mesh, Ragdoll;
    
    public void DoRagdoll()
    {
        if (IsAlive)
        {
            Mesh.SetActive(false);
            Ragdoll.SetActive(true);
            GetComponent<JoystickMovement>().enabled = false;
            GetComponent<CapsuleCollider>().enabled = false;

            // set velocity on child rigidbodies
            setVelocityRecursive(Ragdoll.transform, GetComponent<Rigidbody>().velocity);

            IsAlive = false;
        }
    }

    private void setVelocityRecursive(Transform root, Vector3 velocity)
    {
        Rigidbody rb = root.GetComponent<Rigidbody>();
        if (rb)
            rb.velocity = velocity;

        for (int i = 0; i < root.transform.childCount; i++)
        {
            setVelocityRecursive(root.transform.GetChild(i), velocity);
        }
    }
}
