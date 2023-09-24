using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class PickupObject : MonoBehaviour, IInteractable
{
    public InteractableObject lootInfo;
    static public int totalLoot;
    int lootValue;
    [SerializeField] TextMeshProUGUI lootText;
    public void Interact()
    {
        Destroy(transform.parent.gameObject);
        lootValue = lootInfo.amount;
        totalLoot += lootValue;
        lootText.text = "Loot: " + totalLoot;
    }
}
