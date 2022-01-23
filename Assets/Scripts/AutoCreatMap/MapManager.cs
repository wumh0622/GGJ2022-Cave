using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MapManager : MonoBehaviour
{
    [Header("角色")]
    public Transform mPlayerCenterPos = null;
    [Header("地圖中心點")]
    public Vector2 mMapCenter = Vector2.zero;

    public UnityEvent<int> onMapCreate;

    private Transform mMapParent = null;
    private MapData mTopMapData = null, mDownMapData = null;
    private Queue<GameObject> mCacheTopMapObj = new Queue<GameObject>(), mCacheDownMapObj = new Queue<GameObject>();
    private List<MapSafePoint> mTopSafePoints = new List<MapSafePoint>(), mDownSafePoints = new List<MapSafePoint>();
    private int mCacheXIndex = 0;
    private Vector3 mCacheCreatPos = Vector3.zero;
    private float mCacheCheckNextXPos = 0;
    private bool mIsUpdate = false;
    private bool mIsEndMap = false;

    #region Private
    private void Awake()
    {
        LoadData();
        mMapParent = new GameObject("MapObjects").transform;
        mMapParent.SetParent(this.transform);
        mMapParent.localPosition = Vector3.zero;

    }
    private void LoadData()
    {
        mTopMapData = (MapData)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Resources/Data/Map/TopMapData.asset", typeof(MapData));
        mDownMapData = (MapData)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Resources/Data/Map/DownMapData.asset", typeof(MapData));
        if (mTopMapData == null || mDownMapData == null)
        {
            Debug.LogError("Load Map Data Fail");
        }
        if (mTopMapData.mMapPrefabs == null || mDownMapData.mMapPrefabs == null || mTopMapData.mMapPrefabs.Length == 0 || mDownMapData.mMapPrefabs.Length == 0)
        {
            Debug.LogError("Map Prefab is null");
        }
    }
    private GameObject GetMapObj(MapData iMapData)
    {
        if (mIsEndMap && iMapData.CheckEndCount())
        {
            return iMapData.GetEndMapObj();
        }
        if (iMapData.CheckStartCount())
        {
            return iMapData.GetStartMapObj();
        }
        return iMapData.mMapPrefabs[Random.Range(0, iMapData.mMapPrefabs.Length)];
    }
    private IEnumerator CreatUpdate()
    {
        while (mIsUpdate)
        {
            if (mPlayerCenterPos.position.x >= mCacheCheckNextXPos)
            {
                CreatNextMap();
                DestoryPool();
            }
            yield return null;
        }
        yield break;
    }

    #region 生地圖
    /// <summary>依據 mCacheXIndex 生成地圖</summary>
    private void CreatNextMap(int CreatCount = 1)
    {
        for (int i = 0; i < CreatCount; i++)
        {
            mCacheCreatPos.x = mMapCenter.x + (mTopMapData.mMapX * mCacheXIndex);
            mCacheCreatPos.y = mMapCenter.y;
            EnqueuePool(Instantiate(GetMapObj(mTopMapData), mCacheCreatPos, Quaternion.identity, mMapParent), true);
            mCacheCreatPos.x = mMapCenter.x + (mDownMapData.mMapX * mCacheXIndex);
            mCacheCreatPos.y -= mDownMapData.mMapY;
            EnqueuePool(Instantiate(GetMapObj(mDownMapData), mCacheCreatPos, Quaternion.identity, mMapParent), false);
            mCacheXIndex++;
            onMapCreate.Invoke(mCacheXIndex);
            mCacheCheckNextXPos = mMapCenter.x + (mTopMapData.mMapX * (mCacheXIndex - 2));
        }
    }
    #endregion
    #endregion

    #region DestoryMap
    private void EnqueuePool(GameObject iObj, bool iTop)
    {
        if (iTop)
        {
            mCacheTopMapObj.Enqueue(iObj);
            mTopSafePoints.Add(iObj.GetComponent<MapSafePoint>());
        }
        else
        {
            mCacheDownMapObj.Enqueue(iObj);
            mDownSafePoints.Add(iObj.GetComponent<MapSafePoint>());
        }
    }
    private void DestoryPool()
    {
        Destroy(mCacheTopMapObj.Dequeue());
        Destroy(mCacheDownMapObj.Dequeue());
        mTopSafePoints.RemoveAt(0);
        mDownSafePoints.RemoveAt(0);
    }
    private void ClearAllPool()
    {
        mTopSafePoints.Clear();
        mDownSafePoints.Clear();

        int aMaxCount = mCacheTopMapObj.Count;
        for (int i = 0; i < aMaxCount; i++)
        {
            DestoryPool();
        }
    }
    #endregion

    #region 取得當前切換安全點
    public Vector3 GetSafePoint(bool iTop)
    {
        try
        {
            if (iTop)
            {

                return GetSafePointPos(mTopSafePoints);
            }
            else
            {
                return GetSafePointPos(mDownSafePoints);
            }
        }
        catch (System.Exception iMessage)
        {
            Debug.LogError($"MapManager Get SafePoints Fail GetIsTop => {iTop}   ErrorMessage= {iMessage}");
            throw;
        }
    }
    private Vector3 GetSafePointPos(List<MapSafePoint> iSafePintPool)
    {
        Vector3 aReturPos = iSafePintPool[0].GetNearSafePoint(mPlayerCenterPos.position);
        Vector3 aComparePos = Vector3.zero;
        float aCurDis = Mathf.Abs(aReturPos.x-mPlayerCenterPos.position.x);
        float aNextDis = aCurDis + 1;
        for (int i = 1; i < iSafePintPool.Count; i++)
        {
            aComparePos = iSafePintPool[i].GetNearSafePoint(mPlayerCenterPos.position);
            aNextDis = Mathf.Abs(aComparePos.x - mPlayerCenterPos.position.x);
            if (aNextDis < aCurDis)
            {
                aReturPos = aComparePos;
                aNextDis = aCurDis;
            }
        }
        return aReturPos;
    }
    #endregion

    public void CreatMap_Reset()
    {
        if (mCacheTopMapObj.Count != 0)
        {
            ClearAllPool();
        }
        mTopMapData.ResetData();
        mDownMapData.ResetData();
        mIsEndMap = false;
        mIsUpdate = false;
        mCacheXIndex = -1;
        CreatNextMap(3);
    }
    [ContextMenu("TestCreat")]
    public void CreatMap_Start()
    {
        CreatMap_Reset();
        mIsUpdate = true;
        StartCoroutine(CreatUpdate());
    }
    public void CreatMap_StartEnd()
    {
        mIsEndMap = true;
    }
    public void CreatMap_End()
    {
        mIsUpdate = false;
    }
}