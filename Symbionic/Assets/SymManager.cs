using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SymManager : MonoBehaviour    
{
    public PlayerManager playerManager;
    public RotationManager rotationManager;
    public UIManager ui;
    public TubeManager tubes;

    // Start is called before the first frame update
    void Start()
    {
        playerManager = GameObject.FindObjectOfType<PlayerManager>();
        rotationManager = GameObject.FindObjectOfType<RotationManager>();
        ui = GameObject.FindObjectOfType<UIManager>();
        tubes = GameObject.FindObjectOfType<TubeManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
