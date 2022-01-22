using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	public float OriginMoveSpeed = 5;
	public float JumpSpeed = 60;
	public float FrontCheckDistance = 0.5f;
	public float OriginAttack = 1;

	[Header("UnderGroundMove")]
	public float MinRange = 3;

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
	private RaycastHit2D[] _hits;
	private float _originGravityScale;

	private Dictionary<string, float> _itemEffectTimer;

	void Start()
	{
		_itemEffectTimer = new Dictionary<string, float>();

		_rb = GetComponent<Rigidbody2D>();
		_originGravityScale = _rb.gravityScale;

		_jumpSpeed = JumpSpeed;
	}

	void FixedUpdate()
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
		var deltaTime = Time.fixedDeltaTime;

		foreach (var itemType in _itemEffectTimer.Keys)
		{
			_itemEffectTimer[itemType] -= deltaTime;
			if (_itemEffectTimer[itemType] <= 0)
			{
				_itemEffectTimer.Remove(itemType);
				OnItemTimeOut(itemType);
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
				_rb.gravityScale = 0;
				SetVelocity(_rb.velocity.x, 0);
			}
			else
			{
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
		_hits = Physics2D.RaycastAll(new Vector2(transform.position.x, transform.position.y), Vector2.right, FrontCheckDistance);

		var speed = _moveSpeed;
		foreach (var hit in _hits)
		{
			if (hit.collider != null && hit.collider.tag != "Player")
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
			SetVelocity(_rb.velocity.x, 0);
		}
	}

	private void SetVelocity(float newX, float newY)
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

		if (!isSame)
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
			if (!collider.TryGetComponent<Block>(out var block))
			{
				Debug.LogError($"Collider {collider.name} No Block.cs");
				continue;
			}

			block.Mining(_attack);
		}
	}

	private void OnGetItem(string itemType, float newValue)
	{
		if (newValue < 0)
		{
			Debug.LogError("Invalid Value " + newValue);
			return;
		}

		switch (itemType)
		{
			case "Move":
				_moveSpeed = newValue;
				break;
			case "Attack":
				_attack = newValue;
				break;
			default:
				Debug.LogError("Invalid ItemType " + itemType);
				break;
		}
	}

	private void OnItemTimeOut(string itemType)
	{
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
		switch (other.tag)
		{
			case "Ground":
				_isJump = false;
				print($"TriggerEnter Jump {_isJump}, Ground {_inGround}");
				break;
			case "Item":
				other.GetComponent<ItemInfo>().GetItemInfo(out string itemType, out float value, out float effectSec);
				_itemEffectTimer[itemType] = effectSec;
				OnGetItem(itemType, value);
				break;
			default:
				break;
		}
	}

	private void OnTriggerStay2D(Collider2D other)
	{
		if (other.tag == "Ground" && !_isJump)
		{
			_inGround = true;
			print($"TriggerStay Jump {_isJump}, Ground {_inGround}");
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.tag == "Ground")
		{
			_inGround = false;

			print($"TriggerExit Jump {_isJump}, Ground {_inGround}");
		}
	}

	public void OnDrawGizmosSelected()
	{
		if (ShowDigBox)
		{
			Gizmos.color = Color.white;
			Gizmos.DrawCube(transform.position + new Vector3(DigOffset.x, DigOffset.y, 0), new Vector3(DigSize.x, DigSize.y, 0));
		}

		if (ShowFrontCheck)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawRay(transform.position, Vector3.right * FrontCheckDistance);
		}
	}
}
