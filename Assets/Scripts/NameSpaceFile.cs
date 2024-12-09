using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameSpaceFile : MonoBehaviour
{
    
}

namespace NeededEnums
{
    [System.Serializable]
    public enum AbilityTiming { OnHit, WhenHit, RoundStart, RoundEnd, TurnStart, TurnEnd, EachAction }
}