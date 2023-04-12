using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotemCell : MonoBehaviour
{
    TotemInstance totemInstance;

    public TotemInstance SpawnTotem(GameObject totemPrefab, Character player)
    {
        var totem = Instantiate(totemPrefab, transform);
        totemInstance = totem.GetComponent<TotemInstance>();
        TotemManager.Instance.AddSpawnedTotemToList(totemInstance);
        totemInstance.clone = totem;
        totemInstance.caster = player;

        return totemInstance;
    }

    public bool HasTotem()
    {
        if (totemInstance != null)
            return true;
        else
            return false;
    }
}
