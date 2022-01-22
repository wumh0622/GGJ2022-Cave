using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;

public class GenerateBlock : MonoBehaviour
{
    public GameObject defaultBlock;

    public Vector2 mapSize;
    public float gap;
    public Transform startTransform;
    public bool SETTINGMODE;

    private void OnValidate()
    {

    }

    [ContextMenu("TestCreat")]
    public void CreateBlock()
    {
        if (!startTransform)
        { return; }
        for (int i = 0; i < mapSize.x; i++)
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                GameObject newBlock = Instantiate(defaultBlock);
                newBlock.transform.position = startTransform.position + new Vector3(i * gap, -j * gap, 0);
                newBlock.transform.parent = this.transform;
            }
        }
    }

    private void Update()
    {
        if (!SETTINGMODE)
        {
            return;
        }
        Block[] blocks = GetComponentsInChildren<Block>();
        foreach (var item in blocks)
        {
            DestroyImmediate(item.gameObject);
        }
        CreateBlock();
    }
}
