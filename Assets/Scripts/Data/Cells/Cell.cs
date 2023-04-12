using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour
{
    public Character cellCharacter;
    [SerializeField] private Image highlightBG;
    public bool isSelected;
    public bool isHighlighted;

    [SerializeField] private int xPos;
    [SerializeField] private int yPos;

    private void OnEnable()
    {
        EnemyGrid.DehighlightAllCells += DeHighlight;
        EnemyGrid.DehighlightAllCells += UnSelect;
    }

    private void OnDisable()
    {
        EnemyGrid.DehighlightAllCells -= DeHighlight;
        EnemyGrid.DehighlightAllCells -= UnSelect;
    }

    public void Initialize(int x, int y)
    {
        xPos = x;
        yPos = y;
    }

    public int GetX() { return xPos; }
    public int GetY() { return yPos; }

    public void SetCharacter(Character character)
    {
        cellCharacter = character;
    }

    public void Highlight()
    {
        highlightBG.enabled = true;
        isHighlighted= true;
    }

    public void DeHighlight()
    {
        highlightBG.enabled = false;
        isHighlighted= false;
    }

    public void Select()
    {
        isSelected= true;
    }

    public void UnSelect()
    {
        isSelected = false;
    }
}