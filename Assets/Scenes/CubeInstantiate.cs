using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeInstantiate : MonoBehaviour
{
    [SerializeField]
    Transform cubePrefab;
    void Awake()
    {
        for(int i = -5; i < 5; i += 2) {
            for(int j = 0; j < 10; j += 2) {
                var cube = Instantiate(cubePrefab);
                cube.transform.position = new Vector3(i, j, 5f);
                cube.parent = this.transform;
            }
        }
    }
}
