using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.InputSystem;

public class UserInterfaceManager : BaseSingleton<UserInterfaceManager>
{
    #region Components

        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI debugText;

        public UIHoverDetails HoverDetails;
        public UIPinnedHoverDetails PinnedHoverDetails;
    #endregion Components
    
    #region Life Cycle

    public void FixedUpdate()
    {
        var keyboard = Keyboard.current;
        var mouse = Mouse.current;
        
    }

    #endregion Life Cycle
        public void UpdateDebugText(string text)
        {
            debugText.text = text;
        }

        public void UpdateScore(int score)
        {
            scoreText.text = $"Score: {score}";
        }

    #region User Inputs

        public void PinActiveHover(InputAction.CallbackContext context)
        {
            PinnedHoverDetails.UserInputPinActiveHover(context);
        }
        
        public void RotateDraggingTile(InputAction.CallbackContext context)
        {
            //get the tile that is being dragged
            List<Tile> tiles = GameManager.Instance.tiles.Where(t => t.isDragging).ToList();
            if(tiles.Count == 0) return;
            Tile t = tiles.First();
            if (t == null) return;
            t.UserInputRotate(context);
        }

        public void DebugSolution(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            GameManager.Instance.DebugSolution();
        }
        
        public void DebugGrid(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            GameManager.Instance.DebugGrid();
        }
    #endregion User Inputs
         
}
