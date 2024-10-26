using Engine;
using UnityEngine;


    public class CellSpawner : SingletonBehaviour<CellSpawner>
    {
        [SerializeField] private Cell dummy;

        internal Cell SpawnNew()
        {
            return Instantiate(dummy);
        }
    }
