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


	[Header("Setting Param")]
	[SerializeField]
	private float _moveSpeed = 5;
	[SerializeField]
	private float _jumpSpeed = 60;
	[SerializeField]
	private float frontCheckDistance = 0.5f;

	[Header("UnderGroundMove")]
	[SerializeField]
	private float _minDistance = 3;

	[Header("DigBox")]
	[SerializeField]
	private Vector2 _digOffset;
	[SerializeField]
	private Vector2 _digSize;
	[SerializeField]
	private LayerMask _digLayerMask;

	private bool _isUnderGround = false;
	private bool _inGround = false;
	private bool _isDown = false;
	private bool _isJump = false;

	private Rigidbody2D _rb;
	private RaycastHit2D[] _hits;
	private float _originGravityScale;

	void Start()
	{
		_rb = GetComponent<Rigidbody2D>();
		_originGravityScale = _rb.gravityScale;
	}

	void Update()
	{
		Move();

		if (_isUnderGround)
		{
			UnderGroundAction();
		}
		else
		{
			InGroundAction();
		}

		ChangePosStatus();

		Show();
	}

	private void ChangePosStatus()
	{
		if (Input.GetKeyDown(KeyCode.Q))
		{
			_isUnderGround = !_isUnderGround;

			print($"Click Q, new PosStatus {_isUnderGround}");

			if (_isUnderGround)
			{
				_rb.gravityScale = 0;
				_rb.velocity = new Vector2(_rb.velocity.x, 0);
			}
			else
			{
				_rb.gravityScale = _originGravityScale;
			}
		}
	}

	private void Move()
	{
		_hits = Physics2D.RaycastAll(new Vector2(transform.position.x, transform.position.y), Vector2.right, frontCheckDistance);

		var speed = _moveSpeed;
		foreach (var hit in _hits)
		{
			if (hit.collider != null && hit.collider.tag != "Player")
			{
				speed = 0;
				break;
			}
		}

		_rb.velocity = new Vector2(speed, _rb.velocity.y);
	}

	private void InGroundAction()
	{
		if (_inGround)
		{
			if (Input.GetKeyDown(KeyCode.W) && !_isDown && !_isJump)
			{
				Jump();
			}
			else if (Input.GetKeyDown(KeyCode.S))
			{
				Down(true);
			}
			else if (Input.GetKeyUp(KeyCode.S))
			{
				Down(false);
			}
		}
	}

	private void UnderGroundAction()
	{
		UnderGroundMove();

		if (Input.GetMouseButtonDown(0))
		{
			Dig();
		}
	}

	private void UnderGroundMove()
	{
		var screenPos = Camera.main.WorldToScreenPoint(transform.position);
		var moveY = Input.mousePosition.y - screenPos.y;

		var distance = Mathf.Abs(moveY);
		if (distance >= _minDistance)
		{
			bool isNegative = moveY < 0;

			_rb.velocity = new Vector2(_rb.velocity.x, isNegative ? -_moveSpeed : _moveSpeed);
		}
		else if (_rb.velocity.y != 0)
		{
			_rb.velocity = new Vector2(_rb.velocity.x, 0);
		}
	}

	private void Jump()
	{
		_isJump = true;
		print($"Jump   Jump {_isJump}, Ground {_inGround}");
		_rb.AddForce(Vector2.up * _jumpSpeed * 10);
	}

	private void Down(bool isDown)
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

	private void Dig()
	{
		var point = new Vector2(transform.position.x, transform.position.y) + _digOffset;
		var colliders = Physics2D.OverlapBoxAll(point, _digSize, 0, (int)_digLayerMask);

		if (colliders.Length > 0)
		{
			print("Dig");

			foreach (var collider in colliders)
			{
				if (!collider.TryGetComponent<Block>(out var block))
				{
					Debug.LogError($"Collider {collider.name} No Block.cs");
					continue;
				}

				block.Mining();
			}
		}
	}

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
		if (other.tag == "Ground")
		{
			_isJump = false;
			print($"TriggerEnter Jump {_isJump}, Ground {_inGround}");
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
			Gizmos.DrawCube(transform.position + new Vector3(_digOffset.x, _digOffset.y, 0), new Vector3(_digSize.x, _digSize.y, 0));
		}

		if (ShowFrontCheck)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawRay(transform.position, Vector3.right * frontCheckDistance);
		}
	}
}
