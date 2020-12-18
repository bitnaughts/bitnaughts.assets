using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
public class Utilities : MonoBehaviour
{

    static Dictionary<string, UnityEngine.Object> prefab_cache = new Dictionary<string, UnityEngine.Object>();
    public static T Get<T>(string path) where T : UnityEngine.Object
    {
        if (!prefab_cache.ContainsKey(path))
        {
            prefab_cache[path] = Resources.Load<T>(path);
        }
        return (T)prefab_cache[path];
    }
    // public GameObject AssignMesh(string type, Transform parent)
    // {
        // return Instantiate(
        //     Get<GameObject>(type),
        //     parent
        // ) as GameObject;
        // if (prefab_pool[type].Count == 0) 
        // {
        //     prefab_pool[type].Add(
        //         Add(
        //             type,
        //             Vector3.zero,
        //             0
        //         )
        //     );
        // }
        // GameObject pooled_prefab = prefab_pool[type][0];

        // prefab_pool[type][0]
    // } 

    // string CUBE = "Cube", CUBE_INACTIVE = "Cube!";
    // public static GameObject GetCube() 
    // {
    //     if (prefab_pool[CUBE_INACTIVE].Count == 0)
    //     {
    //         for (int i = 0; i < 100; i++) /* Batch instantiations in groups of 10-1000, unclear what would be ideal for performance */
    //         {
    //             prefab_pool[CUBE_INACTIVE].Add(
    //                 GetNew(CUBE, Vector3.zero, 0)
    //             );    
    //         }
    //     }
    //     prefab_pool[CUBE].Add(prefab_pool[CUBE_INACTIVE][prefab_pool[CUBE_INACTIVE].Count - 1]);
    //     prefab_pool[CUBE_INACTIVE].RemoveAt(prefab_pool[CUBE_INACTIVE].Count - 1);
    //     return prefab_pool[CUBE][prefab_pool[CUBE].Count - 1];

    // }
    // public static void FreeCube(GameObject cube)
    // {
    //     prefab_pool[CUBE].Remove(cube);
    //     prefab_pool[CUBE_INACTIVE].Add(cube);

    // }

    public static GameObject Add(string prefab_name, Vector3 position, Vector3 scale, Quaternion rotation, Transform parent)
    {
        return Add(
            Get<GameObject>(prefab_name),
            position,
            scale,
            rotation, // Quaternion.Euler(0, 0, rotation),
            parent
        );
    }

    public static GameObject Add (GameObject prefab, Vector3 position, Vector3 scale, Quaternion rotation, Transform parent)
    {
        GameObject obj = Instantiate(
            prefab,
            Vector3.zero,
            rotation,
            parent
        ) as GameObject;
        obj.transform.localPosition = position;
        // obj.name = name;
        return obj;
    }
}

//     public IEnumerable<TValue> RandomValues<TKey, TValue>(IDictionary<TKey, TValue> dict)
//     {
//         List<TValue> values = Enumerable.ToList(dict.Values);
//         int size = dict.Count;
//         while (true)
//         {
//             yield return values[Randoms.NewInt(0, size)];
//         }
//     }

//     public static float Bound (float low, float high, float value) 
//     {
//         return Math.Min(high, Math.Max(low, value));
//     }

//     // Dictionary<string, object> dict = GetDictionary();
//     // foreach (object value in RandomValues(dict).Take(10))
//     // {
//     //     Console.WriteLine(value);
//     // }

// }