using UnityEngine;

public class ItemInfo : MonoBehaviour
{
	[SerializeField]
	private string Key;

	[SerializeField]
	private string ItemType;

	[SerializeField]
	private float Value;

	[SerializeField]
	private float EffectSeconds = 5f;

	public void GetItemInfo(out string key,out string itemType, out float value, out float effectSeconds)
	{
		key = this.Key;
		itemType = this.ItemType;
		value = this.Value;
		effectSeconds = this.EffectSeconds;
	}
}
