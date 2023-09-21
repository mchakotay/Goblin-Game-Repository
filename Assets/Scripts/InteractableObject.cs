using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum EPickupType
{
    EPT_Money
}
public class InteractableObject : MonoBehaviour
{
    public EPickupType pickupType = EPickupType.EPT_Money;
    public float amount = 00;
}
