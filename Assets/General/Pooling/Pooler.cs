using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Pooler
{
    private static Dictionary<string, Pool> pools = new Dictionary<string, Pool>();
    private static GameObject poolsContainer;

    public static GameObject Spawn(GameObject go, Vector3 pos, Quaternion rot)
    {
        if (poolsContainer == null)
        {
            // Create the "Pools" GameObject if it doesn't exist
            poolsContainer = new GameObject("Pools");
        }

        GameObject obj;
        string key = go.name.Replace("(Clone)", "");

        if (pools.ContainsKey(key))
        {
            if (pools[key].inactive.Count == 0)
            {
                obj = Object.Instantiate(go, pos, rot, pools[key].parent.transform);
            }
            else
            {
                obj = pools[key].inactive.Pop();
                // if (go.name == "Toxic Splash")
                // {
                //     Debug.Log(pools[key].inactive.Count);
                // }

                obj.transform.position = pos;
                obj.transform.rotation = rot;
                obj.SetActive(true);
            }
        }
        else
        {
            GameObject newParent = new GameObject($"{key}_Pool");
            newParent.transform.SetParent(poolsContainer.transform);

            obj = Object.Instantiate(go, pos, rot, newParent.transform);
            Pool newPool = new Pool(newParent);
            pools.Add(key, newPool);
        }

        return obj;
    }

    public static void Despawn(GameObject go)
    {
        string key = go.name.Replace("(Clone)", "");
        
        if (pools.ContainsKey(key))
        {
            pools[key].inactive.Push(go);
            int count = pools[key].inactive.Count;
            go.transform.position = Vector3.zero;
            go.SetActive(false);

            // Check if the current parent is a pool, and if not, move the object back to the pool
            if (go.transform.parent != null)
            {
                if (!go.transform.parent.gameObject.name.Contains("_Pool"))
                {
                    go.transform.SetParent(pools[key].parent.transform);
                }
            }
        }
        else
        {
            GameObject newParent = new GameObject($"{key}_Pool");
            Pool newPool = new Pool(newParent);

            go.transform.SetParent(newParent.transform);

            pools.Add(key, newPool);
            pools[key].inactive.Push(go);
            go.SetActive(false);
        }
    }

    public static void ReturnToPool(GameObject go)
    {
        string key = go.name.Replace("(Clone)", "");

        if (go.transform.parent != null)
        {
            if (!go.transform.parent.gameObject.name.Contains("_Pool"))
            {
                go.transform.SetParent(pools[key].parent.transform);
            }
        }
    }

    public static void ClearPools()
    {
        pools.Clear();
    }
}