using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayManager : MonoBehaviour
{
    [SerializeField] Pig pig;
    [SerializeField] List<Terrain> terrainList;
    [SerializeField] int initialGrassCount = 5;
    [SerializeField] int horizontalSize;
    [SerializeField] int backViewDistance = -4;
    [SerializeField] int forwardViewDistance = 15;
    [SerializeField, Range(min: 0, max: 1)] float treeProbability;

    Dictionary<int, Terrain> activeTerrainDict = new Dictionary<int, Terrain>(capacity: 20);
    [SerializeField] private int travelDistance;

    public UnityEvent<int, int> OnUpdateTerrainLimit;

    private void Start()
    {
        // create initial Grass pos -4 --- 4
        for (int zPos = backViewDistance; zPos < initialGrassCount; zPos++)
        {
            var terrain = Instantiate(original: terrainList[index: 0]);

            terrain.transform.position = new Vector3(x: 0,y: 0,z: zPos);
            
            if(terrain is Grass grass)
                grass.SetTreePercentage(newProbability: zPos< -1 ? 1 : 0);
            
            terrain.Generate(size: horizontalSize);
            
            activeTerrainDict[key: zPos] = terrain;
        }

        // 4 --- 15
        for (int zPos = initialGrassCount; zPos < forwardViewDistance; zPos++)
        {
            SpawnRandomTerrain(zPos: zPos);
        }

        OnUpdateTerrainLimit.Invoke(arg0: horizontalSize,arg1: travelDistance + backViewDistance);
    }

    private Terrain SpawnRandomTerrain(int zPos)
    {
        Terrain comparatorTerrain = null;
        int randomIndex;
        for (int z = -1; z >= -3; z--)
        {
            var checkPos = zPos + z;
          
            if (comparatorTerrain == null)
            {
                comparatorTerrain = activeTerrainDict[key: checkPos];
                continue;
            }
            else if (comparatorTerrain.GetType() != activeTerrainDict[key: checkPos].GetType())
            {
                randomIndex = Random.Range(minInclusive: 0, maxExclusive: terrainList.Count);
                return SpawnTerrain(terrain: terrainList[index: randomIndex], zPos: zPos);
            }
            else
            {
                continue;
            }
        }

        var candidateTerrain = new List<Terrain>(collection: terrainList);

        for (int i = 0; i < candidateTerrain.Count; i++)
        {
            if (comparatorTerrain.GetType() == candidateTerrain[index: i].GetType())
            {
                candidateTerrain.Remove(item: candidateTerrain[index: i]);
                break;
            }
        }
        
        randomIndex = Random.Range(minInclusive: 0, maxExclusive: candidateTerrain.Count);
        return SpawnTerrain(terrain: candidateTerrain[index: randomIndex], zPos: zPos);
    }

    public Terrain SpawnTerrain(Terrain terrain, int zPos)
    {
        terrain = Instantiate(original: terrain);
        terrain.transform.position = new Vector3(x: 0,y: 0,z: zPos);
        terrain.Generate(size: horizontalSize);
        activeTerrainDict[key: zPos] = terrain;
        return terrain;
    }
    
    public void UpdateTravelDistance(Vector3 targetPosition)
    {
        if (targetPosition.z > travelDistance)
        {
            travelDistance = Mathf.CeilToInt(f: targetPosition.z);
            UpdateTerrain();
        }
    }

    public void UpdateTerrain()
    {
        var destroyPos = travelDistance - 1 + backViewDistance;
        Destroy(obj: activeTerrainDict[key: destroyPos].gameObject);
        activeTerrainDict.Remove(key: destroyPos);

        var spawnPosition = travelDistance - 1 + forwardViewDistance;
        SpawnRandomTerrain(zPos: spawnPosition);

        OnUpdateTerrainLimit.Invoke(arg0: horizontalSize, arg1: travelDistance + backViewDistance);
    }
}
