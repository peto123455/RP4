using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gadgets : MonoBehaviour
{
    [SerializeField] private GameObject rccarPrefab;
    public Gadget equippedGadget = null;
    public class Gadget
    {
        string name;
        public bool hasGadget = false;
        public bool isSpawned = false;
        public GameObject prefab;
        
        public Gadget(string name)
        {
            this.name = name;
        }
    }

    public Gadget rcCar = new Gadget("RC Car");
    void Awake()
    {
        rcCar.prefab = rccarPrefab;
    }

    public void EquipGadget(Gadget gadget)
    {
        if(gadget.hasGadget) equippedGadget = gadget;
    }

    public void SetGadget(Gadget gadget, bool has)
    {
        gadget.hasGadget = has;
    }

}
