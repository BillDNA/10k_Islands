using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
public class Island : HexGrid<TerrainHex> 
{

    
    #region Data


    #endregion Data

    #region Generation
    
        [Button("generate New island")]
        public void GenerateNewIsland()
        {
            RemoveAllChildrenTransforms();
            Generate();
            
            

            AnimateEntry();
        }

        [Button("Animate Entry")]
        public void AnimateEntry()
        {
            //now trigger build animations
            float delay = 0;
            ForEachRandomOrderHex((TerrainHex h) =>
            {
                if (h != null)
                {
                    h.MoveToLayer(LayerType.Terrain);
                    h.buildTrigger.StartCountDown(delay);
                    delay += 0.05f;
                }
            });
            CenterGrid();
            
        }
        public override GameObject GetHexPrefab()
        {
            return PrefabLibrary.Instance.terrainHex;
        }

        private void RepositionHex(TerrainHex h)
        {
            Vector2 loc = findCenter(h);
            h.gameObject.transform.localPosition =  loc;
            h.gameObject.name = $"{h.q},{h.r} - {h.TerrainType}";
        }
        public override TerrainHex GenerateHex(int q, int r)
        {
            GameObject go = Instantiate(PrefabLibrary.Instance.terrainHex, new Vector3(0,0,0) , Quaternion.identity);
            go.transform.localRotation = transform.localRotation;
            go.transform.parent = gameObject.transform;
            go.transform.localScale = new Vector2(1,1);
            TerrainHex h = go.GetComponent<TerrainHex>();
            h.q = q;
            h.r = r;
            RepositionHex(h);

            h.island = this;
            //Figure out Biome
            
            if(q == 0 || q == width-1 || r == 0 || r == height-1)
            {
                h.TerrainType = TerrainType.Water;
                return h;
            }
            List<TerrainHex> neighnors = h.neighbors;

            List<(TerrainType, int)> weights = new List<(TerrainType, int)>();
            int total = 0;
            
            foreach (TerrainType t in Enum.GetValues(typeof(TerrainType)))
            {
                int w = 0;
                BiomesData bd = TerrainSettings.Instance.TerrainChances[t];
                foreach (TerrainHex n in neighnors)
                {
                    w += bd.influence[n.TerrainType];
                }

                total += w;
                weights.Add((t,w));
            }
            //Sort the weights
            weights = weights.OrderBy(x => x.Item2).ToList();

            int pick = SeededRandom.GetInt(0, total);
            for (int i = 0; i < weights.Count; i++)
            {
                if (pick < weights[i].Item2)
                {
                    h.TerrainType = weights[i].Item1;
                    break;
                }
            }

            if (h.TerrainType == TerrainType.Unset)
            {
                h.TerrainType = TerrainType.Plains;
            }
            return h;
        }

        private bool[,] solution;
        
        

    #endregion Generation
    #region Life Cycle
        private void Start()
        {
            GenerateNewIsland();
        }
        
        public void Update()
        {
                
        }
    #endregion Life Cycle

    public void Clear()
    {
        RemoveAllChildrenTransforms();
        hexes = null;
    }

    #region Debug

        public void OnDrawGizmosSelected()
        { 
            DrawGizmoRect(Color.red, Boundries);
            DrawGizmoRect(Color.cyan,new Rect(Boundries.center,new Vector2(0.5f,0.5f)));
        }

    #endregion Debug
}
