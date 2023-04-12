using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using Mono.Cecil.Cil;
using Unity.VisualScripting;
using UnityEngine.TextCore.Text;

public class TurnManager : MonoBehaviour
{
    private float nextTurnDelay = .5f;
    public bool isFirstTurn = true;
    [SerializeField] private TurnCounter TurnCounter;

    public Character selectedPlayerCharacter;

    public event UnityAction<bool> OnBeginTurn;
    public event UnityAction OnEndTurn;

    public static TurnManager Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
            Instance = this;
    }

    private void Start()
    {
        BeginNextTurn();
    }

    public void InvokeBeginNextTurn()
    {
        Invoke(nameof(BeginNextTurn), nextTurnDelay);
    }

    public void BeginNextTurn()
    {
        if (isFirstTurn == true)
        {
            OnBeginTurn?.Invoke(true);
            isFirstTurn = false;
        }
        else
            OnBeginTurn?.Invoke(false);
    }

    public void EndTurn()
    {
        OnEndTurn?.Invoke();
    }
}
