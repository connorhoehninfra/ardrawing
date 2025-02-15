using System.Collections;
using System.Collections.Generic;
using Meta.XR.MRUtilityKit;
using Oculus.Interaction;
using Oculus.Interaction.Surfaces;
using Unity.VisualScripting;
using UnityEngine;

public class FurnitureManager : MonoBehaviour
{
    public MRUKRoom MrUKRoom { private set; get; }
    public GameObject rayDetectablePrefab, UIGuide;
    public static FurnitureManager Instance;
    public Furniture SelectedFurniture;
    public GameObject guideUI;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }


    public void RoomCreatedListener(MRUKRoom mrUKRoom)
    {
        this.MrUKRoom = mrUKRoom;
        StartCoroutine(InitializeFurniture());
    }



    IEnumerator InitializeFurniture()
    {
        yield return new WaitForSeconds(1f);
        foreach (var child in MrUKRoom.Anchors)
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
        if (SelectedFurniture) return;
        SelectedFurniture = furniture;
        Quaternion correctionOffset = Quaternion.Euler(0, 180, 0);
        guideUI = Instantiate(UIGuide, position, rotation * correctionOffset);

    }

}
