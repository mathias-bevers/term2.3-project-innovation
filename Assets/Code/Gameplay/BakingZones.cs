using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class BakingZones : MonoBehaviour
{

    [SerializeField] List<BakingZone> zones = new List<BakingZone>();

   public BakingData GetBonusValue(MarshMallowMovement movement)
    {
        float currentSize = 0;
        for (int i = 0; i < zones.Count; i++)
        {
            BakingZone zone = zones[i];
            currentSize += zone.size;
            if (Vector3.Distance(transform.position, movement.transform.position) < currentSize)
            {
                return new BakingData(zone.deadlyZone, zone.damageMultiplier);
            }
        }

        return new BakingData(true, 0);
    }

#if DEBUG

    private void OnDrawGizmosSelected()
    {
        float currentSize = 0;
        
        for(int i = 0; i < zones.Count; i++)
        {
            Gizmos.color = zones[i].debugColor;
            currentSize+= zones[i].size;
            Gizmos.DrawWireSphere(transform.position, currentSize);
        }
    }
#endif

}

public struct BakingData
{
    public bool isDamageType;
    public float bakingMultiplier;

    public BakingData(bool isDamageType, float bakingMultiplier)
    {
        this.isDamageType = isDamageType;
        this.bakingMultiplier = bakingMultiplier;
    }
}

[System.Serializable]
public struct BakingZone
{
    public float size;
    public bool deadlyZone;
    public float damageMultiplier;
#if DEBUG
    public Color debugColor;
#endif
}
