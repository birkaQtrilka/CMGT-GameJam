using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class PrintAwake : MonoBehaviour
{
    public Tilemap tilemap;
    public GridBrushBase brush;
    void Awake()
    {
        Debug.Log("Editor causes this Awake");
    }

    void Update()
    {
        
        Debug.Log("Editor causes this Update");
        if (Input.GetKeyDown(KeyCode.DownArrow))
            brush.ChangeZPosition(-1);
    }
}
