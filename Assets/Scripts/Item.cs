using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string name;
    public List<float> infectionResistance;
    public float price;
    public Item(string name, List<float> infectionResistance, float price)
    {
        this.name = name;
        this.infectionResistance = infectionResistance;
        this.price = price;
    }
    
}

