using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CameraControls : BaseSingleton<CameraControls>
{
    #region Components

        private Camera _camera;

        public Camera camera
        {
            get
            {
                if(_camera == null) _camera = Camera.main;
                return _camera;
            }
        }
        
    #endregion Components

    #region life cycle
        
        void Start()
        {
            
        }

        void Update()
        {
            ZoomUpdate();
            PanUpdate();
        }
    

    #endregion  life cycle
    
    
    #region Zoom
        private void ZoomUpdate()
        {
            float w = Input.GetAxis("Mouse ScrollWheel");
            if (w == 0) return;
            // finally, do the actual Zoom
            
            Island island = GameManager.Instance.island;
            
            float s = GameManager.Instance.island.transform.localScale.x;
            Vector3 pos = GameManager.Instance.island.transform.position;
            float diff = s - GameManager.Instance.island.transform.localScale.x;

            s += w *  UserControlSettings.Instance.zoomSpeed * Time.deltaTime;
            Vector3 v =  new Vector3(s, s, 1);
            island.transform.localScale = v;
            Vector3 d = new Vector3(diff / 2f, diff / 2f, 0);
            transform.position -= d;
            

        }
    #endregion Zoom

    #region Pan

        public void CenterCameraOnIsland()
        {
            return;
            Vector2 center = GameManager.Instance.island.CenterMass;
            transform.position = new Vector3(
                center.x,
                center.y,
                transform.position.z
            );
        }
        private void PanUpdate()
        {
            Vector3 _currentPan =  Vector3.zero;
            if (Input.GetKey("w"))
            {
                _currentPan.y = 1;
            } else if (Input.GetKey("s"))
            {
                _currentPan.y = -1;
            }
            if (Input.GetKey("a"))
            {
                _currentPan.x = -1;
            } else if (Input.GetKey("d"))
            {
                _currentPan.x = 1;
            }

            _currentPan *= UserControlSettings.Instance.panSpeed * Time.deltaTime;
            transform.localPosition += _currentPan;
        }

    #endregion Pan
}
