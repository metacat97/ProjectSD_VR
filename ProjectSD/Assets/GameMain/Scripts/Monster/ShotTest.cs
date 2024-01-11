using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotTest : MonoBehaviour, IHitObject
{
    private WaitForSeconds uniyTime = new WaitForSeconds(30f);
    public GameObject bullet;
    public void Hit(float damage)
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // ESC키 누르면 재시작
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Golem.G_insance.restart = true;

            // TODO : 게임매니저 게임 재시작시 넣어야할 재시작 메소드 각각 괴수, 랜덤약점 활성화
            Golem.G_insance.Initilize();
            LuckyPointController.instance.Initialize();
        }

        // 엔터키 입력시 괴수 행동시작
        if(Input.GetKeyDown(KeyCode.Return))
        {
            Golem.G_insance.minionRestart = true;
            Golem.G_insance.restart = false;
            Golem.G_insance.GolemStart();
        }

        // Q 입력시 약점유닛 활성화
        if(Input.GetKeyDown(KeyCode.Q))
        {
            LuckyPointController.instance.LuuckyUint(uniyTime);
        }

        // 마우스 좌클릭시 슈팅
        if(Input.GetMouseButtonDown(0))
        {
            Instantiate(bullet, transform.position, Quaternion.identity);
        }
    }
}
