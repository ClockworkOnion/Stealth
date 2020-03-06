using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Items", order = 1)]
public class ItemObject : ScriptableObject
{
    public int price;
    public PlayerItems itemEnum;
}
