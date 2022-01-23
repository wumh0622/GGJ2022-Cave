using UnityEngine;
using DG.Tweening;

public class CameraControl : MonoBehaviour
{
    public LockCameraY mLockCameraY = null;    
    public PlayerController mPlayerCon = null;

    [Header("真實攝影機高度")]
    public float mTrueTopY = 0;
    public float mTrueDownY = 0;

    public Ease mPlayerEase;
    public Ease mCameraEase;
    public float mPlayerMoveTime = 0, mCameraMoveTime = 0;
    public float aReturnDelayGap = 0.25f;

    public MapManager testMap = null;
    public bool testTop = false;
    [ContextMenu("Test")]
    void test()
    {
        if (testTop)
        {
            SwitchToTop(testMap.GetSafePoint(testTop));
        }
        else
        {
            SwitchToDown(testMap.GetSafePoint(testTop));
        }
    }
    public void SwitchToTop(Vector3 iMovePoint)
    {
        MoveTo(iMovePoint, mTrueTopY);
    }
    public void SwitchToDown(Vector3 iMovePoint)
    {
        MoveTo(iMovePoint, mTrueDownY);
    }

    private void MoveTo(Vector3 iMovePoint, float iCameraY)
    {
        mPlayerCon.enabled = false;
        Sequence aSequence = DOTween.Sequence();
        aSequence.Append(mPlayerCon.transform.DOMove(iMovePoint, mPlayerMoveTime).SetEase(mPlayerEase));
        aSequence.Join(DOTween.To(() => mLockCameraY.m_YPosition, x => mLockCameraY.m_YPosition = x, iCameraY, mCameraMoveTime).SetEase(mCameraEase));
        aSequence.AppendInterval(aReturnDelayGap);
        aSequence.AppendCallback(() => mPlayerCon.enabled = true);
    }
}