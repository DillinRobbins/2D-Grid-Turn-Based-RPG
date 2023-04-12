using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIEnemyCount : MonoBehaviour
{
    [SerializeField] private EnemyGrid grid;
    [SerializeField] private TextMeshProUGUI enemyCountText;
    private int enemyCount;

    private void OnEnable()
    {
        Character.OnDie += UpdateEnemyCount;
    }

    private void OnDisable()
    {
        Character.OnDie -= UpdateEnemyCount;
    }

    private void Start()
    {
        enemyCount = grid.GetTotalEnemies();
        enemyCountText.text = "Remaining Enemies: " + enemyCount;
    }

    private void UpdateEnemyCount(Character character)
    {
        enemyCount--;
        enemyCountText.text = "Remaining Enemies: " + enemyCount;
    }
}
