using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "Data/MapData")]
public class MapData : ScriptableObject
{
    public GameObject[] mFixedStartMap;
    public GameObject[] mFixedEndMap;
    public GameObject[] mMapPrefabs = null;
    [Header("地圖大小")]
    public float mMapX = 0;
    public float mMapY = 0;

    private int mFixedStartMaxCount = 0;
    private int mFixedEndMaxCount = 0;
    private int mFixedStartCurCount = 0;
    private int mFixedEndCurCount = 0;


    #region 開頭與結尾固定Map
    public void ResetData()
    {
        mFixedStartMaxCount= (mFixedStartMap != null) ? mFixedStartMap.Length : 0;
        mFixedEndMaxCount = (mFixedEndMap != null) ? mFixedEndMap.Length : 0;
        mFixedStartCurCount = 0;
        mFixedEndCurCount = 0;
    }
    public bool CheckStartCount()
    {
        return mFixedStartCurCount < mFixedStartMaxCount;
    }
    public bool CheckEndCount()
    {
        return mFixedEndCurCount < mFixedEndMaxCount;
    }
    public GameObject GetStartMapObj()
    {
        mFixedStartCurCount++;
        return mFixedStartMap[mFixedStartCurCount - 1];
    }
    public GameObject GetEndMapObj()
    {
        mFixedEndCurCount++;
        return mFixedEndMap[mFixedEndCurCount - 1];
    }
    #endregion
}