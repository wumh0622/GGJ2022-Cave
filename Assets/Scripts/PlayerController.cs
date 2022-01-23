using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
	[Header("Status Info")]
	public bool IsInGround;
	public bool IsDown;
	public bool IsUnderGround;
	public bool IsJump;
	public Vector3 Velocity;

	[Header("Show Collider")]
	public bool ShowDigBox;
	public bool ShowFrontCheck;


	[Header("Base Param")]
	public float OriginMoveSpeed = 5f;
	public float JumpSpeed = 60f;
	public float OriginAttack = 1f;
	public float FrontCheckDistance = 0.6f;

	[Header("UnderGroundMove")]
	public float MinRange = 3f;

	[Header("DigBox")]
	public Vector2 DigOffset;
	public Vector2 DigSize;
	public LayerMask DigLayerMask;


	private float _moveSpeed;
	private float _jumpSpeed;
	private float _attack;

	private bool _isUnderGround = false;
	private bool _inGround = false;
	private bool _isDown = false;
	private bool _isJump = false;

	private Rigidbody2D _rb;
	private float _originGravityScale;

	public float _itemEffectSec;
	public string _itemType;

	public CameraControl cameraControl;
	public MapManager mapManager;

	public UnityEvent<string, float> onGetItem;

	void Start()
	{
		_rb = GetComponent<Rigidbody2D>();
		_originGravityScale = _rb.gravityScale;

		_moveSpeed = OriginMoveSpeed;
		_attack = OriginAttack;
		_jumpSpeed = JumpSpeed;
	}

	void Update()
	{
		if (_isUnderGround)
		{
			UnderGroundAction();
		}
		else
		{
			InGroundAction();
		}

		ChangePosStatus();

		Move();

		ItemTimer();

		Show();
	}

	private void ItemTimer()
	{
		if (_itemEffectSec > 0)
		{
			_itemEffectSec -= Time.fixedDeltaTime;
			if (_itemEffectSec <= 0)
			{
				OnItemTimeOut(_itemType);
			}
		}
	}

	#region Input
	private void ChangePosStatus()
	{
		if (Input.GetKeyDown(KeyCode.Q))
		{
			_isUnderGround = !_isUnderGround;

			print($"Click Q, new PosStatus {_isUnderGround}");

			if (_isUnderGround)
			{
				cameraControl.SwitchToDown(mapManager.GetSafePoint(false));
				_rb.gravityScale = 0;
				SetVelocity(_rb.velocity.x, 0, false);
			}
			else
			{
				cameraControl.SwitchToTop(mapManager.GetSafePoint(true));
				_rb.gravityScale = _originGravityScale;
			}
			
		}
	}

	private void InGroundAction()
	{
		if (_inGround)
		{
			if (Input.GetKeyDown(KeyCode.W) && !_isDown && !_isJump)
			{
				OnJump();
			}
			else if (Input.GetKeyDown(KeyCode.S))
			{
				OnDown(true);
			}
			else if (Input.GetKeyUp(KeyCode.S))
			{
				OnDown(false);
			}
		}
	}

	private void UnderGroundAction()
	{
		UnderGroundMove();

		if (Input.GetMouseButtonDown(0))
		{
			OnDig();
		}
	}
	#endregion Input

	#region Move
	private void Move()
	{
		var point = new Vector2(transform.position.x, transform.position.y);
		var hits = Physics2D.RaycastAll(point, Vector2.right, FrontCheckDistance);

		var speed = _moveSpeed;

		foreach (var hit in hits)
		{
			if (hit.collider != null && !hit.collider.isTrigger && hit.collider.tag != "Player")
			{
				speed = 0;
				break;
			}
		}

		SetVelocity(speed, _rb.velocity.y);
	}

	private void UnderGroundMove()
	{
		var screenPos = Camera.main.WorldToScreenPoint(transform.position);
		var moveY = Input.mousePosition.y - screenPos.y;

		var distance = Mathf.Abs(moveY);
		if (distance >= MinRange)
		{
			float newY = moveY < 0 ? -OriginMoveSpeed : OriginMoveSpeed;

			SetVelocity(_rb.velocity.x, newY);
		}
		else
		{
			SetVelocity(_rb.velocity.x, 0, false);
		}
	}

	private void SetVelocity(float newX, float newY, bool alwaysRefresh = true)
	{
		float x = _rb.velocity.x;
		float y = _rb.velocity.y;
		bool isSame = true;

		if (x != newX)
		{
			isSame = false;
			x = newX;
		}

		if (y != newY)
		{
			isSame = false;
			y = newY;
		}

		if (alwaysRefresh || !isSame)
		{
			_rb.velocity = new Vector2(x, y);
		}
	}
	#endregion Move

	#region Action
	private void OnJump()
	{
		_isJump = true;
		print($"Jump   Jump {_isJump}, Ground {_inGround}");
		_rb.AddForce(Vector2.up * _jumpSpeed * 10);
	}

	private void OnDown(bool isDown)
	{
		_isDown = isDown;

		if (isDown)
		{
			print("Down");
		}
		else
		{
			print("Up");
		}
	}

	private void OnDig()
	{
		var point = new Vector2(transform.position.x, transform.position.y) + DigOffset;
		var colliders = Physics2D.OverlapBoxAll(point, DigSize, 0, (int)DigLayerMask);

		if (colliders == null || colliders.Length == 0)
		{
			return;
		}

		print("Dig");

		foreach (var collider in colliders)
		{
			var block = collider.GetComponent<Block>();
			block.Mining(_attack);
		}
	}

	private void OnGetItem(ItemInfo itemInfo)
	{
		itemInfo.GetItemInfo(out var key, out var itemType, out var value, out var effectSec);

		if (value < 0)
		{
			Debug.LogError("Invalid Value " + value);
			return;
		}

		print($"Get New Item {itemType}, Value = {value}");

		switch (itemType)
		{
			case "Move":
				_moveSpeed = value;
				break;
			case "Attack":
				_attack = value;
				break;
			default:
				Debug.LogError("Invalid ItemType " + itemType);
				break;
		}

		_itemType = itemType;
		_itemEffectSec = effectSec;
		onGetItem.Invoke(key, effectSec);
	}

	private void OnItemTimeOut(string itemType)
	{
		print($"Item {itemType} Timeout");

		switch (itemType)
		{
			case "Move":
				_moveSpeed = OriginMoveSpeed;
				break;
			case "Attack":
				_attack = OriginAttack;
				break;
			default:
				break;
		}
	}
	#endregion Action

	private void Show()
	{
		//Show
		IsUnderGround = _isUnderGround;
		IsInGround = _inGround;
		IsDown = _isDown;
		Velocity = _rb.velocity;
		IsJump = _isJump;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{

			//print($"TriggerEnter Jump {_isJump}, Ground {_inGround}");

		if (other.tag == "Item")
		{
			var itemData = other.GetComponent<ItemInfo>();
			if(itemData)
            {
				OnGetItem(itemData);
				Destroy(other.gameObject);
			}
			
		}
	}

	private void OnTriggerStay2D(Collider2D other)
	{

			_inGround = true;
		_isJump = false;
		//print($"TriggerStay Jump {_isJump}, Ground {_inGround}");
	}

	private void OnTriggerExit2D(Collider2D other)
	{

			_inGround = false;
			//print($"TriggerExit Jump {_isJump}, Ground {_inGround}");

	}

	public void OnDrawGizmosSelected()
	{
		if (ShowDigBox)
		{
			Gizmos.color = Color.white;
			var center = transform.position + new Vector3(DigOffset.x, DigOffset.y, 0);
			var size = new Vector3(DigSize.x, DigSize.y, 0);
			Gizmos.DrawCube(center, size);
		}

		if (ShowFrontCheck)
		{
			Gizmos.color = Color.red;
			var direct = Vector3.right * FrontCheckDistance;
			Gizmos.DrawRay(transform.position, direct);
		}
	}
}
