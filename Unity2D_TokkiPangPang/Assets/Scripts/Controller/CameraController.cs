using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Vector3 _delta;
    
    // 카메라 위치 이동을 마지막 업데이트에 실행함으로 써 떨림현상 완화
    void LateUpdate()
    {
        if (Managers.Game.player == null)
            return;

        transform.position = Managers.Game.player.transform.position + _delta;
    }

    // 카메라 위치 메소드
    public void SetQuaterView(Vector3 delta)
    {
        _delta = delta;
    }
}
