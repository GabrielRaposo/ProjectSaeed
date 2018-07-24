using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    [System.Serializable]
    public struct PoolInfo
    {
        public GameObject prefab;
        public int size;
        [HideInInspector] public List<GameObject> pool;
        [HideInInspector] public int index;
    }
    public PoolInfo[] poolInfos;

    Vector2 startingPosition = Vector2.down * 100;
    static public Spawner instance;

    private void Awake()
    {
        instance = this;
    }

    void Start ()
    {
		for(int i = 0; i < poolInfos.Length; i++)
        {
            poolInfos[i].pool = new List<GameObject>();
            for(int j = 0; j < poolInfos[i].size; j++)
            {
                GameObject _object = Instantiate(
                    poolInfos[i].prefab, startingPosition, Quaternion.identity, transform);
                _object.SetActive(false);
                poolInfos[i].pool.Add(_object);
            }
        }
	}

    public GameObject GetFromPool(int poolIndex, Vector2 position)
    {
        PoolInfo poolInfo = poolInfos[poolIndex];
        GameObject _object = poolInfo.pool[poolInfo.index];
        _object.SetActive(true);
        _object.transform.position = position;

        poolInfo.index = (poolInfo.index + 1) % poolInfo.size;
        poolInfos[poolIndex].index = poolInfo.index;

        return _object;
    }

    public void Return(GameObject _object)
    {
        //_object.SetActive(false);
        _object.transform.position = startingPosition;
    }

}
