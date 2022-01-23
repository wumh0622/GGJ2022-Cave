using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
	[Header("Show ColliderCheck")]
	public bool ShowDigBox;
	public bool ShowFrontCheck;

	[Header("Base Param")]
	public float OriginMoveSpeed = 5f;
	public float JumpSpeed = 60f;
	public float OriginAttack = 1f;
	public float FrontCheckDistance = 0.6f;
	public float ChangeMapCD = 2f;


	[Header("Status Info")]
	public Vector3 Velocity;
	[SerializeField]
	private bool _canChangeMap = true;
	[SerializeField]
	private bool _isUnderGround = false;
	[SerializeField]
	private bool _inGround = false;
	[SerializeField]
	private bool _isDown = false;
	[SerializeField]
	private bool _isJump = false;

	public string _itemType;
	public float _itemEffectSec;

	[Header("UnderGroundMove")]
	public float MinRange = 3f;

	[Header("DigBox")]
	public Vector2 DigOffset;
	public Vector2 DigSize;
	public LayerMask DigLayerMask;

	public CameraControl cameraControl;
	public MapManager mapManager;
	public Text ChangeMapCDText;

	public UnityEvent<string, float> onGetItem;

	private float _moveSpeed;
	private float _jumpSpeed;
	private float _attack;
	private float _curMapCD;

	private Rigidbody2D _rb;
	private float _originGravityScale;

	void Start()
	{
		_rb = GetComponent<Rigidbody2D>();
		_rb.constraints = RigidbodyConstraints2D.FreezeRotation;

		_originGravityScale = _rb.gravityScale;

		_moveSpeed = OriginMoveSpeed;
		_attack = OriginAttack;
		_jumpSpeed = JumpSpeed;
		_curMapCD = ChangeMapCD;
	}

	void Update()
	{
		ChangePosStatus();

		if (_isUnderGround)
		{
			UnderGroundAction();
		}
		else
		{
			InGroundAction();
		}

		Move();

		Timer();

		Show();
	}

	private void Timer()
	{
		var deltaTime = Time.deltaTime;

		if (_itemEffectSec > 0)
		{
			_itemEffectSec -= deltaTime;
			if (_itemEffectSec <= 0)
			{
				_itemEffectSec = 0;
				OnItemTimeOut(_itemType);
			}
		}

		if (_curMapCD > 0)
		{
			_curMapCD -= deltaTime;
			if (_curMapCD <= 0)
			{
				_curMapCD = 0;
				_canChangeMap = true;
			}
		}
	}

	#region Input
	private void ChangePosStatus()
	{
		if (!_canChangeMap)
		{
			return;
		}

		if (Input.GetKeyDown(KeyCode.Q))
		{
			_canChangeMap = false;
			_curMapCD = ChangeMapCD;

			_isUnderGround = !_isUnderGround;

			print($"Click Q, new PosStatus {_isUnderGround}");

			try
			{
				if (_isUnderGround)
				{
					cameraControl.SwitchToTop(mapManager.GetSafePoint(true));
				}
				else
				{
					cameraControl.SwitchToDown(mapManager.GetSafePoint(false));
				}
			}
			catch (System.Exception ex)
			{
				Debug.LogError(ex.Message);
			}

			_rb.gravityScale = _isUnderGround ? 0 : _originGravityScale;
		}
	}

	private void InGroundAction()
	{
		if (_inGround && !_isJump)
		{
			if (Input.GetKeyDown(KeyCode.W) && !_isDown)
			{
				OnJump();
			}
			if (Input.GetKeyDown(KeyCode.S))
			{
				OnDown(true);
			}
			if (Input.GetKeyUp(KeyCode.S))
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

		float newY = 0f;
		if (distance >= MinRange)
		{
			newY = moveY > 0 ? _moveSpeed : -_moveSpeed;
		}

		SetVelocity(_rb.velocity.x, newY);
	}

	private void SetVelocity(float newX, float newY)
	{
		float x = _rb.velocity.x;
		float y = _rb.velocity.y;

		if (x != newX)
		{
			x = newX;
		}

		if (y != newY)
		{
			y = newY;
		}

		_rb.velocity = new Vector2(x, y);
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
				return;
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
		Velocity = _rb.velocity;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		//print($"TriggerEnter Jump {_isJump}, Ground {_inGround}");

		if (other.tag == "Item")
		{
			var itemData = other.GetComponent<ItemInfo>();
			if (itemData)
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
