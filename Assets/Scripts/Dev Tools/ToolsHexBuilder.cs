using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using  UnityEditor;
using UnityEngine.UI;
using TMPro;
using  System.IO;
using UnityEditor.Animations;

public class ToolsHexBuilder : BaseMonoBehaviour
{
    #region Componets

        [BoxGroup("Components")]
        public TMP_Dropdown GamePartsDropDown;
        
        [BoxGroup("Components/Mode")] public Button ModeButtonRGO;
        [BoxGroup("Components/Mode")] public Button ModeButtonBuilding;
        [BoxGroup("Components/QR")] public Button QRButton00;
        [BoxGroup("Components/QR")] public Button QRButton01;
        [BoxGroup("Components/QR")] public Button QRButton10;
        [BoxGroup("Components/QR")] public Button QRButton11;
        [BoxGroup("Components/RGO Level")] public Button RGOFloatingButton;
        [BoxGroup("Components/RGO Level")] public Button RGOTrayButton;
        [BoxGroup("Components/RGO Level")] public Button RGOLevelButton1;
        [BoxGroup("Components/RGO Level")] public Button RGOLevelButton2;
        [BoxGroup("Components/RGO Level")] public Button RGOLevelButton3;
        
        
        [BoxGroup("Components/Auto Save")] public RectTransform AutoSaveIndicator;
        [BoxGroup("Components/Auto Save")] public Toggle AutoSaveToggle;
    #endregion Componets
    #region Menu Items

        
        public void BuildAllRGOs()
        {
            List<ResourceGatheringOperation> rgos = Resources.LoadAll<ResourceGatheringOperation>("Libraries").ToList();

            List<(bool, bool)> qr = new List<(bool, bool)>()
            {
                (true, true),
                (true, false),
                (false, true),
                (false, false)
            };
            
            foreach (ResourceGatheringOperation rgo in rgos)
            {
                currentRGO = rgo;
/*
                for (int i = 0; i < 4; i++)
                {
                    
                    if (!rgo.hasPrefabFor(i))
                    {
                        Q = qr[i].Item1;
                        R = qr[i].Item2;
                        
                        CheckRGODirs();
                        GameObject obj = CreateBlankRGO(rgo, RGOAnimatorTemplate);
                            
                        SaveRGO(obj);

                        rgo.prefabs[i] = AssetDatabase.LoadAssetAtPath<GameObject>(_pathRGOPrefab);
                        
                    }    
                }*/
                
            }
        }
    #endregion
    #region Data
        #region Terrain Data

        [BoxGroup("Data")] [BoxGroup("Data/QR")]
        public static bool Q;

        [BoxGroup("Data/QR")] public static bool R;
        
        

        #endregion Terrain Data

        [BoxGroup("Data/Mode")] public bool isBuildingMode;
        #region Buildings Data
            
            [BoxGroup("Data/Mode")] public Building currentBuilding;

            private List<Building> _buildings;

        #endregion Buildings Data

        #region RGO Data

            [BoxGroup("Data/Mode")] 
            public ResourceGatheringOperation currentRGO;

            private List<ResourceGatheringOperation> _rgos;

        #endregion RGO Data

        #region Workspace Data
        
            [BoxGroup("Data")]
            [BoxGroup("Data/Workspace")] public TileHex CurrentWorkSpace;
            [SerializeField,HideInInspector]
            private TerrainHex _terrainHex;

            [BoxGroup("Data/Workspace"),ShowInInspector]
            public TerrainHex terrainHex
            {
                get
                {
                    return _terrainHex;
                }
                set
                {
                    _terrainHex = value;
                    if (value != null)
                    {
                        Q = value.q % 2 == 0; //Might be backwards
                        R = value.r % 2 == 0;
                    }
                    else
                    {
                        Q = true;
                        R = true;
                    }
                }
            }
            
        #endregion Workspace Data

        #region Templates

            [BoxGroup("Templates"),ShowInInspector] public AnimatorController RGOAnimatorTemplate;
            [BoxGroup("Templates"),ShowInInspector] public List<AnimationClip> RGOAnimationClipTemplates;
            
            [BoxGroup("Templates"),ShowInInspector] public AnimatorController BuildingAnimatorTemplate;
            [BoxGroup("Templates"),ShowInInspector] public List<AnimationClip> BuildingAnimationClipTemplates;

        #endregion Templates
    #endregion Data

    #region Life Cycle
        private void Start()
        {
            InitializeWorkSpace();
            InitializeQRButtons();
            InitializeModeButtons();
            InitializeGamePartsDropdown();
            InitializeRGOLevelButtons();
            ChangeTORGOMode();
            
        }

        private void Update()
        {
           
        }

    #endregion Life Cycle
    
    #region Mode Managment

        private void InitializeModeButtons()
        {
            ModeButtonBuilding.onClick.AddListener(ChangeToBuildingMode);
            ModeButtonRGO.onClick.AddListener(ChangeTORGOMode);
            
            ModeButtonBuilding.interactable = false;
            ModeButtonRGO.interactable = true;
            
            isBuildingMode = true;
        }

        private void ChangeToBuildingMode()
        {
            ModeButtonBuilding.interactable = false;
            ModeButtonRGO.interactable = true;
            
            isBuildingMode = true;
            InitializeGamePartsDropdown();
        }

        private void ChangeTORGOMode()
        {
            ModeButtonBuilding.interactable = true;
            ModeButtonRGO.interactable = false;
            
            isBuildingMode = false;
            InitializeGamePartsDropdown();
        }
        
        private void InitializeRGOLevelButtons()
        {
            RGOTrayButton.onClick.AddListener(() => {ChangeRGOLevel(-1);});
            RGOFloatingButton.onClick.AddListener(() => {ChangeRGOLevel(0);});
            RGOLevelButton1.onClick.AddListener(() => {ChangeRGOLevel(1);});
            RGOLevelButton2.onClick.AddListener(() => {ChangeRGOLevel(2);});
            RGOLevelButton3.onClick.AddListener(() => {ChangeRGOLevel(3);});
        }
        private void ChangeRGOLevel(int lvl)
        {
            RGOTrayButton.interactable = lvl != -1;
            RGOFloatingButton.interactable = lvl != 0;
            RGOLevelButton1.interactable = lvl != 1;
            RGOLevelButton2.interactable = lvl != 2;
            RGOLevelButton3.interactable = lvl != 3;
            
            CurrentWorkSpace.CheckActiveStructure();
            
        }
    
    #endregion Mode Managment

    #region QR Managment

        private void InitializeQRButtons()
        {
            QRButton00.onClick.AddListener(() => {ChangeQR(false,false);});
            QRButton01.onClick.AddListener(() => {ChangeQR(false,true);});
            QRButton10.onClick.AddListener(() => {ChangeQR(true,false);});
            QRButton11.onClick.AddListener(() => {ChangeQR(true,true);});  
            ChangeQR(Q,R); 
        }
        private void ChangeQR(bool q, bool r)
        {
            bool didChange = Q != q || R != r;
            
            Q = q;
            R = r;

            QRButton00.interactable = !(!Q && !R);
            QRButton01.interactable = !(!Q && R);
            QRButton10.interactable = !(Q && !R);
            QRButton11.interactable = !(Q && R);

            
            if(didChange)
            {
                terrainHex.q = Q ? 0 : 1;
                terrainHex.r = R ? 0 : 1;
                if(isBuildingMode) LoadBuilding(currentBuilding);
                else LoadRGO(currentRGO);
            }
        }
    
    #endregion QR Managment

    #region WorkSpace

        private void InitializeWorkSpace()
        {
            RemoveAllChildrenTransforms();
            
            GameObject go = Instantiate(PrefabLibrary.Instance.terrainHex, new Vector3(0,0,0) , Quaternion.identity);
            go.transform.parent = transform;
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;

            terrainHex = go.GetComponent<TerrainHex>();
            terrainHex.q = Q ? 0 : 1;
            terrainHex.r = R ? 0 : 1;
            
            go = Instantiate(PrefabLibrary.Instance.tileHex, new Vector3(0,0,0) , Quaternion.identity);
            go.transform.parent = transform;
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;

            CurrentWorkSpace = go.GetComponent<TileHex>();
            CurrentWorkSpace.terrainHex = terrainHex;
            CurrentWorkSpace.UserInteractionEnabled = false;
            //CurrentWorkSpace.CityIndicator.SetActive(false);
        }

    #endregion WorkSpace

    #region  Dropdown
        private void InitializeGamePartsDropdown()
        {
            GamePartsDropDown.onValueChanged.RemoveAllListeners();
                    
            if (isBuildingMode) InitializeBuildingDropdown();
            else InitializeRGODropdown();
        }
        #region RGO DropDown


            private void InitializeRGODropdown()
            {
                
                _rgos = Resources.LoadAll<ResourceGatheringOperation>("Libraries").ToList();
                GamePartsDropDown.options = new System.Collections.Generic.List<TMP_Dropdown.OptionData>();
                foreach (ResourceGatheringOperation rgo in _rgos) 
                {
                    GamePartsDropDown.options.Add(new TMP_Dropdown.OptionData(rgo.name));
                }

                GamePartsDropDown.onValueChanged.AddListener(OnRGOOptionChanged);
                GamePartsDropDown.value = 0;
                OnRGOOptionChanged(0);
            }
            
            private void OnRGOOptionChanged(int v)
            {
                LoadRGO(_rgos[v]);
            }

            private void LoadRGO(ResourceGatheringOperation rgo)
            {
                
                currentRGO = rgo;

                terrainHex.TerrainType = rgo.terrainUnder;

                CurrentWorkSpace.ForceBuildRGO(rgo);

            }

        #endregion RGO DropDown
        #region Building DropDown
            private void InitializeBuildingDropdown()
            {
                
                _buildings = Resources.LoadAll<Building>("Libraries").ToList();
                GamePartsDropDown.options = new System.Collections.Generic.List<TMP_Dropdown.OptionData>();
                foreach (Building b in _buildings) 
                {
                    GamePartsDropDown.options.Add(new TMP_Dropdown.OptionData(b.name));
                }
                GamePartsDropDown.onValueChanged.AddListener(OnBuildingOptionChanged);
                
                GamePartsDropDown.value = 0;
                OnBuildingOptionChanged(0);
                
            }
            public void OnBuildingOptionChanged(int v)
            {
                LoadBuilding(_buildings[v]);
            }

            private void LoadBuilding(Building b)
            {
                
                
                currentBuilding = b;

                terrainHex.TerrainType = b.terrainUnder;

                CurrentWorkSpace.ForceBuildBuilding(b);
            }
        #endregion Building Drop Down

    #endregion  Dropown


}
