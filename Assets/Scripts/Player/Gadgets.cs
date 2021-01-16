using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gadgets : MonoBehaviour
{
    [SerializeField] private GameObject rccarPrefab;
    public List<Gadget> gadgets;
    public Gadget equippedGadget = null;

    void Awake()
    {
        gadgets = new List<Gadget>()
        {
            new Gadget(0, "RC Car", Gadget.Action.Spawn, rccarPrefab),
            new Gadget(1, "Laser", Gadget.Action.Laser)
        };
    }

    public void EquipGadget(Gadget gadget)
    {
        this.equippedGadget = gadget;
    }

    public void SetGadget(Gadget gadget, bool has)
    {
        gadget.SetGadget(has);
    }

}
