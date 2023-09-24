using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestOpen : MonoBehaviour, IInteractable
{
    private Animator chestAnimatorRef;
    private bool open = false;
    public void Interact()
    {
        open = !open;
        chestAnimatorRef = transform.GetComponent<Animator>();
        chestAnimatorRef.SetBool("open", open);
    }
}
