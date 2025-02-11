using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Oculus.Interaction.Surfaces;
using Unity.VisualScripting;
using UnityEngine;
using Meta.XR.MRUtilityKit;

public class FurnitureManager : MonoBehaviour
{
    public GameObject rayDetectablePrefab, UIGuide;
    public static FurnitureManager Instance;
    public Furniture SelectedFurniture;
    public GameObject guideUI;
    public MRUK mruk;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        mruk = GameObject.FindObjectOfType<MRUK>();
    }

    private void Start()
    {
        this.mruk.RegisterSceneLoadedCallback(RoomCreatedListener);
    }


    public void RoomCreatedListener()
    {
        Debug.Log("Furniture Mgr: Room created");
        StartCoroutine(InitializeFurniture(this.mruk.GetCurrentRoom()));
    }

    IEnumerator InitializeFurniture(MRUKRoom mrUKRoom)
    {
        Debug.Log("Furniture Mgr: Initializing furniture");
        yield return new WaitForSeconds(1f);
        foreach (var child in mrUKRoom.Anchors)
        {
            Debug.Log("Furniture Mgr: Checking anchor with label " + child.Label);
            if (child.HasAnyLabel(MRUKAnchor.SceneLabels.TABLE))
            {
                Debug.Log("Furniture Mgr: Found table");
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
