using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public List<Item> items = new List<Item>();
    
    private static List<float> dentalMaskResistance = new List<float> {0.1f,0.3f};
    private static List<float> N95MaskResistance = new List<float> { 0.15f, 0.3f };

    public void Start()
    {
        items.Add(new Item("Dental mask", dentalMaskResistance, 1.0f));
        items.Add(new Item("N95 mask", N95MaskResistance, 3.0f));
    }
    public void Update()
    {
        
    }
}
