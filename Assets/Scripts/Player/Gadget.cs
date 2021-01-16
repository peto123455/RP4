using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gadget
{
    public enum Action
    {
        None,
        Spawn,
        Laser
    };

    int id;
    string name;
    Action action;
    bool hasGadget = false;
    bool isSpawned = false;
    public GameObject prefab;
    
    public Gadget(int id, string name, Action action = Action.None, GameObject prefab = null)
    {
        this.id = id;
        this.name = name;
        this.action = action;
        this.prefab = prefab;
    }

    public bool IsSpawned()
    {
        return this.isSpawned;
    }

    public void SetSpawned(bool isSpawned)
    {
        this.isSpawned = isSpawned;
    }

    public void SetGadget(bool hasGadget)
    {
        this.hasGadget = hasGadget;
    }

    public bool HasGadget()
    {
        return this.hasGadget;
    }

    public Action ReturnAction()
    {
        return action;
    }
}
