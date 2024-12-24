using System.Collections;
using System.Collections.Generic;
using Meta.XR.MRUtilityKit;
using Oculus.Interaction;
using Oculus.Interaction.Surfaces;
using Unity.VisualScripting;
using UnityEngine;

public class FurnitureManager : MonoBehaviour
{
    MRUKRoom mrUKRoom;
    public GameObject rayDetectablePrefab, UIGuide;
    public static FurnitureManager Instance;
    private Furniture selectedFurniture;
    public Furniture SelectedFurniture { set { selectedFurniture = value; } get { return selectedFurniture; } }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }


    public void RoomCreatedListener(MRUKRoom mrUKRoom)
    {
        this.mrUKRoom = mrUKRoom;
        StartCoroutine(InitializeFurniture());
    }



    IEnumerator InitializeFurniture()
    {
        yield return new WaitForSeconds(1f);
        foreach (var child in mrUKRoom.Anchors)
        {
            if (child.HasAnyLabel(MRUKAnchor.SceneLabels.TABLE))
            {
                Instantiate(rayDetectablePrefab, child.transform);
                var colliderSurface = child.GetComponentInChildren<ColliderSurface>();
                var collider = child.gameObject.GetComponentInChildren<Collider>();
                colliderSurface.InjectCollider(collider);

            }
        }
    }



    public void RegisterAsSelected(Furniture furniture, Vector3 position, Quaternion rotation)
    {
        if (selectedFurniture) return;
        selectedFurniture = furniture;
        Quaternion correctionOffset = Quaternion.Euler(0, 180, 0);
        Instantiate(UIGuide, position, rotation * correctionOffset);

    }

}
