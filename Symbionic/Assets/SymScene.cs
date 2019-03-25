using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SymScene : MonoBehaviour
{
    public PlayerScene player;
    public UIScene ui;
    public PauseScene p;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindObjectOfType<PlayerScene>();
        ui = GameObject.FindObjectOfType<UIScene>();
        p = GameObject.FindObjectOfType<PauseScene>();
    }   

    // Update is called once per frame
    void Update()
    {
        
    }
}
