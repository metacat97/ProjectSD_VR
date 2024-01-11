using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_1 : UnitBase
{
    private void OnDestroy()
    {
        KHJUIManager.Instance.PopUpMsg("유닛1 파괴됨");
    }
}
