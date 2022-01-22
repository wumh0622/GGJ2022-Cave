using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{


	[Header("Only Show")]
	public bool IsInGround;
	public bool IsDown;
	public bool IsUnderGround;
	public Vector3 Velocity;

	[Header("Setting Param")]
	[SerializeField]
	private float moveSpeed = 0;

	[SerializeField]
	private float jumpSpeed = 0;


	private bool _isUnderGround = false;
	private bool _inGround = false;
	private bool _isDown = false;

	private Rigidbody _rb;

	void Start()
	{
		_rb = GetComponent<Rigidbody>();
		_rb.useGravity = true;
	}

	void Update()
	{
		transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);

		if (_isUnderGround)
		{
			UnderGroundMove();
		}
		else
		{
			InGroundMove();
		}

		ChangePosStatus();

		Show();
	}

	private void Show()
	{
		//Show
		IsUnderGround = _isUnderGround;
		IsInGround = _inGround;
		IsDown = _isDown;
		Velocity = _rb.velocity;
	}

	private void ChangePosStatus()
	{
		if (Input.GetKeyDown(KeyCode.Q))
		{
			_isUnderGround = !_isUnderGround;

			print($"Click Q, new PosStatus {_isUnderGround}");

			if (_isUnderGround)
			{
				_rb.useGravity = false;
				_rb.velocity = Vector3.zero;
			}
			else
			{
				_rb.useGravity = true;
			}
		}
	}

	private void InGroundMove()
	{
		if (_inGround)
		{
			if (Input.GetKeyDown(KeyCode.W) && !_isDown)
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

	private void UnderGroundMove()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Dig();
		}
	}

	private void Jump()
	{
		print("Jump");

		_inGround = false;

		_rb.AddForce(Vector3.up * jumpSpeed * 10);
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
		print("Dig");
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Ground" && !_inGround)
		{
			print($"TriggerEnter {other.name}");
			_inGround = true;
		}
	}
}
