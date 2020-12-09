using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RC : MonoBehaviour
{
    [SerializeField] private GameObject fovInstance;
    private GameObject fovIns;
    private FOV fov;
    void Awake()
    {
        fovIns = Instantiate(fovInstance, new Vector2(0f,0f), Quaternion.identity);
        fov = fovIns.GetComponent<FOV>();
    }

    void LateUpdate()
    {
        fov.SetPosition(transform.position);
    }

    public void DestroyRC()
    {
        Destroy(fovIns);
        Destroy(gameObject);
    }
}
