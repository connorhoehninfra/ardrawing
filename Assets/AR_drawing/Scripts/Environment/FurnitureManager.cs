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
    public GameObject rayPrefab;

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
            // if (child.VolumeBounds.HasValue)
            if (child.HasAnyLabel(MRUKAnchor.SceneLabels.TABLE))
            {
                // var colliderSurface = child.AddComponent<ColliderSurface>();
                // var pointableElement = child.AddComponent<PointableElement>();
                // var rayInteractable = child.AddComponent<RayInteractable>();
                // var collider = child.gameObject.GetComponentInChildren<Collider>();
                // var pointableUnityEventWrapper = child.AddComponent<PointableUnityEventWrapper>();


                // if (!collider) Debug.LogError("Couldnt find collider");
                // colliderSurface.InjectCollider(collider);
                // rayInteractable.InjectOptionalPointableElement(pointableElement);
                // rayInteractable.InjectSurface(colliderSurface);
                // pointableUnityEventWrapper.InjectPointable(rayInteractable);

                // child.AddComponent<Furniture>();
                Instantiate(rayPrefab, child.transform);
                var colliderSurface = child.GetComponentInChildren<ColliderSurface>();
                var collider = child.gameObject.GetComponentInChildren<Collider>();
                colliderSurface.InjectCollider(collider);

            }
        }
    }
}
