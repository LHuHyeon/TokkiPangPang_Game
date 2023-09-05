using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawning : MonoBehaviour
{
    public GameObject[] _spawnBlock;

    [SerializeField]
    Vector3 _spawnPos;

    [SerializeField]
    int _blockCount = 0;

    [SerializeField]
    int _keepBlockCount = 0;

    [SerializeField]
    int _minBlockCount = 0;

    // 소환 중인지?
    bool isSpawn = false;

    // 블럭 개수 +/-
    public void AddMonsterCount(int value) { _blockCount += value; }

    void Start()
    {
        Managers.Game.OnSpawnEvent -= AddMonsterCount;
        Managers.Game.OnSpawnEvent += AddMonsterCount;
    }

    void Update()
    {
        Debug.Log(_blockCount);
        BlockSpawn();
    }

    void BlockSpawn()
    {
        // 스폰 중이 아니라면 블럭 생성
        if (isSpawn == false)
        {
            isSpawn = true;
            
            int random = Random.Range(_minBlockCount, _keepBlockCount);
            int ranObj = Random.Range(0, _spawnBlock.Length);
            for(int i=0; i<random; i++)
            {
                GameObject block = Managers.Game.Spawn(_spawnBlock[ranObj], transform);

                // y 1칸씩 간격 주기
                block.transform.position = Managers.Game.player.transform.position + _spawnPos + (Vector3.up * i);
            }

            _blockCount = random;
        }

        // 블럭이 없으면
        if (_blockCount == 0)
            isSpawn = false;
    }
}
