using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public void Interact();
}
public class Interactor : MonoBehaviour
{
    public Transform interactorSource;
    public float interactRange;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray r = new Ray(interactorSource.position, interactorSource.forward);
            if (Physics.Raycast(r, out RaycastHit hitInfo, interactRange))
            {
                InteractWithObject(hitInfo.collider.gameObject);
            }
        }
    }
    void InteractWithObject(GameObject objectToInteractWith)
    {
        if (objectToInteractWith.TryGetComponent(out IInteractable interactableObj))
        {
            interactableObj.Interact();
        }
    }
}
