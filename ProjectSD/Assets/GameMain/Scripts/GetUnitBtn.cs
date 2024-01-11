using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GetUnitBtn : MonoBehaviour
{
    public GameObject[] unitBtnList = default;

    private float originWidth, originHeight;
    private RectTransform upperRectTP; // rectTransform 
    private GridLayoutGroup unitGridLayout;  

    //[SerializeField] private GameObject btnPrefab;

    //TODO 동적 셀 할당

    void Awake()
    {
        upperRectTP = gameObject.GetComponent<RectTransform>();
        unitGridLayout = gameObject.GetComponent<GridLayoutGroup>();

        originWidth = upperRectTP.rect.width;
        originHeight = upperRectTP.rect.height;

        unitBtnList = Resources.LoadAll<GameObject>("UnitBtnPrefabs/");

    }
    private void OnEnable()
    {
        //CreateUnitBtn();
    }
    private void OnDisable()
    {
        //CreateUnitBtn();
    }
    void Start()
    {
        //Debug.Log($"{unitBtnList.Length}");
        
        CreateUnitBtn();
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
   
    //생성해야 하는 수만큼 버튼 프리팹 생성
    //그런데 얼만큼 생성해야하는지 어떻게 알지?
    public void CreateUnitBtn()
    {
        for(int i = 0; i < unitBtnList.Length; i++ )
        {
            Instantiate(unitBtnList[i],this.transform);
        }
    }

    public void ChangeDynamicGrid(int count, int minCol, int maxCol)
    {
        int rows = Mathf.Clamp(Mathf.CeilToInt((float)count / minCol),1, maxCol + 1);
        int cols = Mathf.CeilToInt((float)count / rows);

        float spaceW = (unitGridLayout.padding.left + unitGridLayout.padding.right) + (unitGridLayout.spacing.x * (rows - 1));
        float spaceH = (unitGridLayout.padding.top + unitGridLayout.padding.bottom) + (unitGridLayout.spacing.y * (rows - 1));

        float maxWidth = originWidth - spaceW;
        float maxHeight = originHeight - spaceH;

        float width = Mathf.Min(upperRectTP.rect.width - (unitGridLayout.padding.left + unitGridLayout.padding.right) - (unitGridLayout.spacing.x * (cols - 1)), maxWidth);
        float height = Mathf.Min(upperRectTP.rect.height - (unitGridLayout.padding.top + unitGridLayout.padding.bottom) - (unitGridLayout.spacing.y * (rows - 1)), maxHeight);

        unitGridLayout.cellSize = new Vector2(width / cols, height / rows);
    }

}
