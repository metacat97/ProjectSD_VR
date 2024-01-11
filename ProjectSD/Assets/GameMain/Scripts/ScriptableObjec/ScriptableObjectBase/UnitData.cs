using UnityEngine;

[CreateAssetMenu(fileName = "Unit Data", menuName = "Scriptable Object/Unit Data")]
public class UnitData : ScriptableObject
{
    public string unitName = default;
    public int unitPrice = default;
    public int unitPower = default;
    public int unitLifeTime = default;
    public int bulletPointNum = default;

    
}
