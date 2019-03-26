using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//core app entrypoint class
public class SymApp : MonoBehaviour
{
    //references to all central app scripts
    public SymStatus status;
    public SymManager manager;
    public SymScene scene;

    public static SymApp instance;

    public static SymApp Instance
    {
        get { return instance ?? (instance = new GameObject("SymApp").AddComponent<SymApp>()); }
    }

    private void Start()
    {
        status = GameObject.FindObjectOfType<SymStatus>();
        manager = GameObject.FindObjectOfType<SymManager>();
        scene = GameObject.FindObjectOfType<SymScene>();
    }
}
