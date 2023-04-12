using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.Rendering.DebugUI.Table;

public class PlayerCell : MonoBehaviour
{
    public enum Type
    {
        Top, Left, Right, Bottom
    }

    public Type CellType;
    [SerializeField] Character character;
    [SerializeField] CharacterUI characterUI;
    public TotemCell totemSlot;
    Outline selectedOutline;

    private void Awake()
    {
        selectedOutline = GetComponent<Outline>();
    }

    private void OnEnable()
    {
        CharacterManager.Instance.OnNewPlayerSelected += UpdateOutline;
    }

    private void OnDisable()
    {
        CharacterManager.Instance.OnNewPlayerSelected -= UpdateOutline;
    }

    private void Start()
    {
        character = GetComponentInChildren<Character>();
        characterUI.Initialize(character);

        if (CharacterManager.Instance.GetSelectedPlayer() == character)
            selectedOutline.enabled = true;
        else
            selectedOutline.enabled = false;
    }

    public Character GetCharacter()
    {
        return character;
    }

    void UpdateOutline(Character character)
    {
        if (this.character == character && this.character != null)
            selectedOutline.enabled = true;
        else
            selectedOutline.enabled = false;
    }
}
