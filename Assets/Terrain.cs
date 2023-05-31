using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terrain : MonoBehaviour
{
    [SerializeField] GameObject tilePrefab;

    protected int horizontalSize;

    public virtual void Generate(int size)
    {
        horizontalSize = size;
        
        if (size == 0)
            return;

        if((float)size % 2 == 0)
            size -= 1;

        int limit = Mathf.FloorToInt(f: (float) size / 2);
            
        for (int i = -limit; i <= limit; i++)
        {
            SpawnTile(xPos: i);
        }

        var leftBoundaryTile = SpawnTile(xPos: -limit -1);
        var rightBoundaryTile = SpawnTile(xPos: limit +1);

        DarkenObject(go: leftBoundaryTile);
        DarkenObject(go: rightBoundaryTile);
    }

    private GameObject SpawnTile(int xPos)
    {
        var go = Instantiate(
            original: tilePrefab,
            parent: transform);

        go.transform.localPosition = new Vector3(x: xPos,y: 0,z: 0); 

        return go;
    }

    private void DarkenObject(GameObject go)
    {
        var renderers = go.GetComponentsInChildren<MeshRenderer>();
        
        foreach (var rend in renderers)
        {
            rend.material.color *= rend.material.color * Color.grey;
        }
    }
}
