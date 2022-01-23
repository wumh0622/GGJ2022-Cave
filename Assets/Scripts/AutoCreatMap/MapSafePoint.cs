using System.Collections.Generic;
using UnityEngine;

public class MapSafePoint : MonoBehaviour
{
    public List<Transform> mSafePoint = new List<Transform>();

    public Vector3 GetNearSafePoint(Vector3 iPlayerPos)
    {
        if (mSafePoint != null && mSafePoint.Count > 0)
        {
            float iPlayerXPos = iPlayerPos.x;
            Vector3 aReturnPos = mSafePoint[0].position;
            float aDis = Mathf.Abs(iPlayerXPos - aReturnPos.x);
            //Debug.Log("第一個距離" + aDis);
            float aAntoherDis = aDis + 1;
            for (int i = 1; i < mSafePoint.Count; i++)
            {
                aAntoherDis =Mathf.Abs(iPlayerXPos - mSafePoint[i].position.x);
                //Debug.Log($"第{i}個距離" + aAntoherDis);
                if (aAntoherDis < aDis)
                {
                    aReturnPos = mSafePoint[i].position;
                    aDis = aAntoherDis;
                }
            }
            //Debug.Log("ReturnPos"+aReturnPos);
            return aReturnPos;
        }
        Debug.LogError("SafePoint Array is Null");
        return iPlayerPos;
    }


    //public Transform Player;
    //[ContextMenu("Test")]
    //public void Test()
    //{
    //    GetNearSafePoint(Player.position);
    //}

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