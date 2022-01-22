using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Header("����")]
    public Transform mPlayerCenterPos = null;
    [Header("�a�Ϥ����I")]
    public Vector2 mMapCenter = Vector2.zero;

    private Transform mMapParent = null;
    private MapData mTopMapData = null, mDownMapData = null;
    private Queue<GameObject> mCacheTopMapObj = new Queue<GameObject>(), mCacheDownMapObj = new Queue<GameObject>();
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

    #region �ͦa��
    /// <summary>�̾� mCacheXIndex �ͦ��a��</summary>
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
        }
        else
        {
            mCacheDownMapObj.Enqueue(iObj);
        }
    }
    private void DestoryPool()
    {
        Destroy(mCacheTopMapObj.Dequeue());
        Destroy(mCacheDownMapObj.Dequeue());
    }
    private void ClearAllPool()
    {
        int aMaxCount = mCacheTopMapObj.Count;
        for (int i = 0; i < aMaxCount; i++)
        {
            DestoryPool();
        }
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