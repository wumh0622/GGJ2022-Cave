using System.Collections.Generic;
using UnityEngine;

public class MapSafePoint : MonoBehaviour
{
    public List<Transform> mSafePoint = new List<Transform>();

    public Vector3 GetNearSafePoint(Vector3 iPlayerPos)
    {
        if (mSafePoint != null && mSafePoint.Count > 0)
        {
            Vector3 iPlayerXPos = OnlyGetXPos(iPlayerPos);
            Vector3 aReturnPos = mSafePoint[0].position;
            float aDis = Vector3.SqrMagnitude(iPlayerXPos - OnlyGetXPos(aReturnPos));
            float aAntoherDis = aDis + 1;
            for (int i = 1; i < mSafePoint.Count; i++)
            {
                aAntoherDis = Vector3.SqrMagnitude(iPlayerPos - OnlyGetXPos(mSafePoint[1].position));
                if (aAntoherDis < aDis)
                {
                    aReturnPos = mSafePoint[1].position;
                    aDis = aAntoherDis;
                }
            }
            return aReturnPos;
        }
        Debug.LogError("SafePoint Array is Null");
        return iPlayerPos;
    }

    private Vector3 OnlyGetXPos(Vector3 iPos)
    {
        iPos.y = 0;
        iPos.z = 0;
        return iPos;
    }

    [Header("初始X位置")]
    public float mFirstX = 0;
    [Header("生成數量")]
    public byte mCreatCount = 0;
    [Header("生成間隔")]
    public float mCreatGap = 0;
    [ContextMenu("CreatSafePoint")]
    private void CreatSafePoint()
    {
        mSafePoint.Clear();

        Vector3 aCachePos = Vector3.zero;
        GameObject aSafePoolParent = new GameObject("SafePointPool");
        aSafePoolParent.SetActive(false);
        aSafePoolParent.transform.SetParent(this.transform);
        aSafePoolParent.transform.localPosition = Vector3.zero;
        aSafePoolParent.transform.localScale = Vector3.one;

        for (int i = 0; i < mCreatCount; i++)
        {
            GameObject aSafePoint = new GameObject($"SafePoint_{i}");
            aSafePoint.transform.SetParent(aSafePoolParent.transform);
            aCachePos.x = mFirstX + (mCreatGap * i);
            aSafePoint.transform.localPosition = aCachePos;
            mSafePoint.Add(aSafePoint.transform);
        }
    }
    [ContextMenu("RemoveNull")]
    private void RemoveNull()
    {
        for (int i = 0; i < mSafePoint.Count; i++)
        {
            if (mSafePoint[i] == null)
            {
                mSafePoint.RemoveAt(i);
                i--;
            }
        }
    }
    private void OnDrawGizmos()
    {
        if (mSafePoint != null && mSafePoint.Count > 0)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < mSafePoint.Count; i++)
            {
                if (mSafePoint[i] == null)
                {
                    continue;
                }
                Gizmos.DrawWireSphere(mSafePoint[i].position, 0.5f);
            }
        }
    }
}