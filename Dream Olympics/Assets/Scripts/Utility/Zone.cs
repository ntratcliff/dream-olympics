using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Zone : MonoBehaviour
{
    public string Tag;
    public List<GameObject> ObjectsInZone;

    public Material UnoccupiedMat, OccupiedMat;
    private MeshRenderer meshRenderer;

    public void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        ObjectsInZone = new List<GameObject>();
    }

    public bool HasObjects
    {
        get
        {
            return ObjectsInZone.Count > 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == Tag)
        {
            ObjectsInZone.Add(other.gameObject);

            if (meshRenderer && meshRenderer.sharedMaterial != OccupiedMat)
                meshRenderer.sharedMaterial = OccupiedMat;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == Tag)
        {
            ObjectsInZone.Remove(other.gameObject);

            if(meshRenderer && !HasObjects)
            {
                meshRenderer.sharedMaterial = UnoccupiedMat;
            }
        }
    }
}
