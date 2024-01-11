using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Unit_2 : UnitBase
{
    private void OnDestroy()
    {
        KHJUIManager.Instance.PopUpMsg("유닛2 파괴됨");
    }

}
