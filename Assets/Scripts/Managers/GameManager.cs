using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor.Rendering;

public class GameManager : BaseSingleton<GameManager>
{
    #region Data

        public string randomSeed;

        public IslandGenerationSettings islandSettings;
    
    #endregion
    #region Game Parts

        private Island _island;
        [BoxGroup("Game Parts"),ShowInInspector]
        public Island island
        {
            get
            {
                if (_island == null) _island = FindObjectOfType<Island>();
                return _island;
            }
        }

        [BoxGroup("Game Parts"),ShowInInspector]
        public Tray tray {
            get
            {
                return Tray.Instance;
            }
        }
        
        private List<Tile> _tiles;
        [BoxGroup("Game Parts"),ShowInInspector]
        public Tile[] tiles
        {
            get
            {
                if (_tiles == null) _tiles = new List<Tile>();
                return _tiles.ToArray();
            }
        }
        
    #endregion Game Parts

    public void DebugSolution()
    {
        GameManager.Instance.island.ForEachHex(
            (int q, int r, TerrainHex h) =>
            {
                if (h == null)
                {
                    return;
                }

                HexLoc key = new HexLoc(q, r);
                if(solution.ContainsKey(key.ToString())) h.DebugText.text = $"{solution[key.ToString()]}";
                else h.DebugText.text = $"BAD";
            }
        );
        foreach (Tile tile in tiles)
        {
            tile.ForEachHex((q, r, h) =>
            {
                if (h == null) return;
                h.DebugText.text = $"{tile.solution}";
            });
        }
    }

    public void DebugGrid()
    {
        //have each island hex display its grid location
        GameManager.Instance.island.ForEachHex(
            (int q, int r, TerrainHex h) =>
            {
                if (h == null)
                {
                    return;
                }

                h.DebugText.text = $"{q},{r}";
            }
        );
        //have each tile display its grid location
        foreach (Tile tile in tiles)
        {
            tile.ForEachHex((q, r, h) =>
            {
                if (h == null) return;
                h.DebugText.text = $"{q},{r}";
            });
        }
    }

    #region island Genreation
        private List<int> visibleSolutions;
        private Dictionary<string, int> solution;
        public int GenerateNewIsland()
        {
            island.Clear();
            island.width = islandSettings.width+2;
            island.height = islandSettings.height+2;
            island.Generate();
            
            //Debug.Log("GenerateSolution - Mark A");
            visibleSolutions = new List<int>();
            int currentID = 1;
            solution = new Dictionary<string, int>();
            _tiles = new List<Tile>();
            //build solutions
            island.ForEachRandomOrderHex((int q, int r, TerrainHex h) => {
                if(h == null ) {
                    return;
                } 
                HexLoc key = new HexLoc(q,r);
                if(!solution.ContainsKey(key.ToString())) {
                    //if the hex is water indicate not part of solution
                    if(h.TerrainType == TerrainType.Water) {
                        solution[key.ToString()] = -1;
                    }
                    else {
                        //if the hex is not water, then it is part of the solution
                        solution[key.ToString()] = 0;
                    }
                }
                //if this is a water hex, then we don't need to do anything
                if(h.TerrainType == TerrainType.Water) {
                    return;
                }
                //if currentID of the piece is less than the number of pieces we want to generate
                if(currentID-1 < islandSettings.tileSizes.Count) {
                    int puzzleID =  solution[key.ToString()];
                    //if the puzzleID is 0, then we need to generate a new piece
                    if(puzzleID == 0) {

                        //Debug.Log("GenerateSolution - no solution hex detected - " + currentID);
                        tileHexLocs = new List<HexLoc>();
                        tileHexLocs.Add(key);
                        solution[key.ToString()] = currentID;
                        tileHexLocs = pieceBuilder(currentID,h,islandSettings.blockyness ,islandSettings.tileSizes[currentID-1]-1);
                        if(tileHexLocs.Count < islandSettings.minTileSize) {
                            h.TerrainType = TerrainType.Water;
                            solution[key.ToString()] = -1;
                        } else {
                            //Debug.Log("GenerateSolution - About to build a piece - " + currentID);
                            Tile t = BuildTile(tileHexLocs);
                            t.gameObject.name = "Tile - " + currentID;
                            t.solution = currentID;
                            t.UpdateBorders();
                            //t.transform.localPosition = -island.findCenter(Mathf.FloorToInt(island.width/2),Mathf.FloorToInt(island.height/2)) + (Vector2)h.transform.localPosition;
                            currentID++;
                            AddTile(t);
                            //Debug.Log("GenerateSolution - Piece built - " + (currentID-1));
                        }
                    }
                }
            });
            //replace unused hexes from the island with water
            island.ForEachHex((int q, int r, TerrainHex h) => {
                if(h == null) return;
                HexLoc key = new HexLoc(q,r);
                if(solution[key.ToString()] == 0) {
                    h.TerrainType = TerrainType.Water;
                }
            });
            Invoke("EntrySize", 0.1f);
            //Debug.Log("GenerateSolution - Mark Complete");
            return currentID;
        }
        public void EntrySize() {
            
            island.CenterGrid();
            island.AnimateEntry();
            foreach (Tile t in tiles)
            {
                t.ResizeGridToFitSpace(new Rect());
            }
        }

    #endregion island Genreation

    #region Tile Generation
        
        public void AddTile(Tile t) {
            _tiles.Add(t);
            tray.DropTileOff(t);
        } 
        private void RandomRotation(Tile t) {
            if(!islandSettings.rotationOn) return;
            for(int i = 0; i < SeededRandom.GetInt(0,13); i++) {
                t.RotateCW();
            }
        }

        private Tile BuildEmptyTile()
        {
            GameObject go = GameObject.Instantiate(PrefabLibrary.Instance.EmptyTile);
            return go.GetComponent<Tile>();
        }
        private Tile CopyPiece(Tile reference) {
            Tile t = BuildEmptyTile();
            t.width = reference.width;
            t.height = reference.height;
            t.Generate();
            int size = 0;
            t.ForEachHex((int q, int r, TileHex h) => {
                if(reference.GetHex(q,r)!=null) {
                    size++;
                } else {
                    t.RemoveHex(q,r);
                }
            });
            
            if(size >= islandSettings.minSizeForCity) {
                t.BuildCityLoc(reference.cityHex.loc);
            }
            return t;
        }
        
        private List<Tile> SplitPiece(Tile refrence) {
            if(refrence.hexCount / 2.0f <= islandSettings.minTileSizeToSplit) return new List<Tile>(); //can't split a piece less than 4
            List<HexLoc> A = new List<HexLoc>();
            List<HexLoc> B = new List<HexLoc>();
            List<HexLoc> visited = new List<HexLoc>();

            Hex h = refrence.GrabRandomHex();
            if(h == null) return new List<Tile>(); //failed to slit an empty Tile
            A.Add(h.loc);
            visited.Add(h.loc);
            h = refrence.GrabRandomHex(visited);
            if(h == null) return new List<Tile>(); //failed to slit an empty Tile
            B.Add(h.loc);
            visited.Add(h.loc);
            int currentA = 0;
            int currentB = 0;
            while(visited.Count < refrence.hexCount && !((A.Count >= islandSettings.minTileSizeToSplit || B.Count >= islandSettings.minSizeForCity) && currentA ==  0 && currentB == 0)) {
                TileHex[] NA = refrence.GetHex(A[currentA]).neighbors.ToArray();
                TileHex[] NB = refrence.GetHex(B[currentB]).neighbors.ToArray();
                Hex HA = null;
                Hex HB = null;
                int ai = 0;
                int bi = 0;

                while(ai < NA.Length && (NA[ai] == null || visited.Contains(NA[ai].loc))) {
                    ai++;
                }
                while(bi < NB.Length && (NB[bi] == null || visited.Contains(NB[bi].loc))) {
                    bi++;
                } 

                if(ai >= NA.Length) {
                    currentA = Mathf.Max(0,currentA-1);
                } 
                if(bi >= NB.Length) {
                     currentB = Mathf.Max(0,currentB-1);
                }     
                if(ai < NA.Length) HA = refrence.GetHex(NA[ai].loc);
                if(bi < NB.Length) HB = refrence.GetHex(NB[bi].loc);
                if(HA == HB && HA != null) {
                    if( SeededRandom.GetFloat(1f)  <= 0.5f) {
                        A.Add(HA.loc);
                        visited.Add(HA.loc);
                        currentA++;
                    } else {
                        B.Add(HB.loc);
                        visited.Add(HB.loc);
                        currentB++;
                    }
                } else {
                    if(HA != null) {
                        A.Add(HA.loc);
                        visited.Add(HA.loc);
                        currentA++;
                    }
                    if(HB != null) {
                        B.Add(HB.loc);
                        visited.Add(HB.loc);
                        currentB++;
                    }
                }
            }
            if(A.Count <= islandSettings.minTileSize || B.Count <= islandSettings.minTileSize || visited.Count < refrence.hexCount) return new List<Tile>();
            return new List<Tile>() {BuildTile(A),BuildTile(B)};//p;
        }

        private Tile BuildTile(List<HexLoc> pieceHexLocs)
        {
            //log the hexlocs
            Debug.Log("BuildTile  Start size = " + pieceHexLocs.Count);
            

            //create a new tile
            Tile t = BuildEmptyTile();

            
            pieceHexLocs = shiftShapeToOrigin(pieceHexLocs);
            
            HexLoc min = new HexLoc(pieceHexLocs[0].q, pieceHexLocs[0].r);
            HexLoc max = new HexLoc(pieceHexLocs[0].q, pieceHexLocs[0].r);
            //find the size of the piece
            foreach(HexLoc boardLoc in pieceHexLocs) {
                if(boardLoc.q < min.q) {
                    min.q = boardLoc.q;
                } else if (boardLoc.q > max.q) {
                    max.q = boardLoc.q;
                }
                if(boardLoc.r < min.r) {
                    min.r = boardLoc.r;
                } else if (boardLoc.r > max.r) {
                    max.r = boardLoc.r;
                }
            }

            //Debug.Log("Constructing piece geometry - matrix adjusted");     
            t.width = max.q +1;
            t.height = max.r+1;
            t.Generate();
            //Debug.Log("Constructing piece geometry - blank grid generated");     

            t.ForEachHex((int q, int r, TileHex h) => {
                if(pieceHexLocs.Contains(h.loc)) {
                } else {
                    t.RemoveHex(q,r);
                }
            });
            Debug.Log($"Done size = {t.hexCount}");
            //Debug.Log("Constructing piece geometry - grid made to match matrix");     
            if(pieceHexLocs.Count >= islandSettings.minSizeForCity)
            {
                TileHex h = t.GrabRandomHex();
                if(h != null) t.BuildCityLoc(t.GrabRandomHex().loc);
            }
            //p.DrawEdges();

            //Debug.Log("Constructing piece geometry - done");     
            return t;
        }
        [Button("Debug shiftShapeToOrigin")]
        private List<HexLoc> shiftShapeToOrigin(List<HexLoc> coordinates)
        {
            //Debug.Log("input: "+ string.Join(", ", coordinates.Select(x => $"({x.ToString()})").ToArray()));
            // Find the minimum q and r values in the list
            int minQ = int.MaxValue;
            int minR = int.MaxValue;
            foreach (var coord in coordinates)
            {
                minQ = Math.Min(minQ, coord.q);
                minR = Math.Min(minR, coord.r);
            }
            //round minR to even
            if(minR % 2 != 0) {
                minR--;
            }
            
            // Subtract the minimum values from all the q and r values in the list
            List<HexLoc> shiftedCoords = new List<HexLoc>();
            foreach (var coord in coordinates)
            {
                shiftedCoords.Add(new HexLoc(coord.q - minQ, coord.r - minR));
            }

            //Debug.Log("actual: "+ string.Join(", ", shiftedCoords.Select(x => $"({x.ToString()})").ToArray()));
    
            return shiftedCoords;
        }
        private List<HexLoc> tileHexLocs;
        
        private List<HexLoc> pieceBuilder(int pieceID, TerrainHex current, float addChance, int limit, List<HexLoc> lookedAtAlready = null) {
            //Debug.Log("Piece Builder - "+pieceID+" - " + "("+pieceHexLocs.Count+")");
            if(limit < tileHexLocs.Count) return tileHexLocs;
            if(lookedAtAlready == null) lookedAtAlready =  new List<HexLoc>();
            List<TerrainHex> hexesThatCanBeBuildOff = new List<TerrainHex>();
            foreach(TerrainHex h in shuffledNeighbors(current)) {
                bool add = SeededRandom.GetFloat() <= addChance;
                bool key = !solution.ContainsKey(h.loc.ToString());
                bool val = !key ? solution[h.loc.ToString()] == 0 : false;
                bool notAlreadyLookedAt = !lookedAtAlready.Contains(h.loc);
                lookedAtAlready.Add(h.loc);
                if(add && (key || val) && notAlreadyLookedAt) {
                    tileHexLocs.Add(h.loc);
                    solution[h.loc.ToString()] = pieceID;
                    //Debug.Log("Piece Builder - "+pieceID+" - Added Hex");
                    hexesThatCanBeBuildOff.Add(h);
                    if(limit < tileHexLocs.Count) return tileHexLocs;
                } 
            }
            foreach(TerrainHex h in hexesThatCanBeBuildOff) {
                tileHexLocs = pieceBuilder(pieceID,h,addChance,limit,lookedAtAlready);
                if(limit < tileHexLocs.Count) return tileHexLocs;
            }
            return tileHexLocs;
        }
        
        public  TerrainHex[] shuffledNeighbors(TerrainHex t,bool waterAllowed = false)
        {
            List<TerrainHex> hexes = t.neighbors.Where(n => n.TerrainType != TerrainType.Water).ToList();
                return Shuffle(hexes,2).ToArray();
        }
    #endregion Tile Generation

    #region Life Cycle

        public void Clear()
        {
            foreach (Tile t in tiles)
            {
                Destroy(t.gameObject);
            }
            _tiles = null;
            island.Clear();
        }

        public void Start()
        {
        }

        public void Update()
        {
            if (!hasInitialized)
            {
                Init();
                return;
            }
            UpdateScore();
        }
        
        bool hasInitialized = false;
        public void Init()
        {
            hasInitialized = true;
            GenerateNewRandomSeed();
            GenerateNewIsland();
        }
    #endregion Life Cycle
    
    #region Score

        public bool needsScoreRecalculation = true;
        public void UpdateScore()
        {
            if (!needsScoreRecalculation) return;
            int s = 0;
            foreach (Tile t in tiles)
            {
                if (t.isPlaced)
                {
                    t.ForceRecalcuateRGOAndBuildings();
                    s += t.score;
                }
            }
            UserInterfaceManager.Instance.UpdateScore(s);
            needsScoreRecalculation = false;
        }
    #endregion Score
    #region Debug Buttons

        [Button("Generate New Island")]
        public void DebugGenerateNewIsland()
        {
            GenerateNewRandomSeed();
            GenerateNewIsland();
        }
        
        [Button("Create Tile")]
        public void DebugCreateTile()
        {
            GameObject go = GameObject.Instantiate(PrefabLibrary.Instance.EmptyTile);
            Tile t = go.GetComponent<Tile>();
            
            t.GenerateFullTile(3,3);
            
            Tray.Instance.DropTileOff(t);
        }

        [Button("Regenerate Random Seed")]
        public void GenerateNewRandomSeed()
        {
            
            if(randomSeed.IsNullOrWhitespace()) SeededRandom.GenerateRandomSeed();
            else SeededRandom.GenerateNewSeed(randomSeed);
            randomSeed = SeededRandom.GetCurrentSeed();
        }
    #endregion Debug Buttons
}
