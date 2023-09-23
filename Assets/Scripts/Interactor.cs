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
    private bool isInteractable = false;
    private bool open = false;
    private Animator chestAnimatorRef;
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
                Debug.DrawRay(interactorSource.position, interactorSource.forward);
                Debug.Log("Our ray is hitting something");
                Debug.Log(hitInfo.collider.gameObject.name);
                InteractWithObject(hitInfo.collider.gameObject);
            }
        }
        if (isInteractable == true)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                open = !open;
                chestAnimatorRef.SetBool("open", open);
            }
        }
    }
    void InteractWithObject(GameObject objectToInteractWith)
    {
        if (objectToInteractWith.TryGetComponent(out IInteractable interactableObj))
        {

            Debug.Log(objectToInteractWith.name);
            interactableObj.Interact();
        }

        if (objectToInteractWith.CompareTag("Chest"))
        {
            isInteractable = true;
            Transform chestRef = objectToInteractWith.transform.parent.Find("ChestA");
            Animator chestAnimator = chestRef.GetComponent<Animator>();
            chestAnimatorRef = chestAnimator;
        }
    }
}
