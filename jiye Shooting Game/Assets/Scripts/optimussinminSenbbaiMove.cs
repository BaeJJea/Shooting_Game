/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCPlayerController : MonoBehaviour
{
	public Player Player;
	public CharacterController controller;
	PhotonView aView;
	public GameObject gunPos;
	public GameObject RHandPos;
	public GameObject humanModel;

	private float currentCameraRotationX = 0;
	public float cameraRotationLimit;
	public float Speed;
	public float walkSpeed = 5f;
	public float runSpeed = 8f;
	public bool run;
	public bool fastrun = false;
	private Vector3 Dir = Vector3.zero;
	private Vector3 gravity = new Vector3(0, -20, 0);

	public float JumpSpeed = 5f;
	private float ySpeed;
	private float originalStepOffset;

	bool isMoving = false;

	void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		aView = PhotonView.Get(this.transform.root);

		if (aView.isMine && Player != null)
		{
			RHandPos = Player.animator.GetBoneTransform(HumanBodyBones.RightHand).gameObject;
			gunPos = Player.GunObjectList[0].transform.parent.gameObject;

			gunPos.transform.parent = RHandPos.transform;
			gunPos.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
			controller = GetComponent<CharacterController>();

			SendTurnOnAnimator();
		}
		else
			enabled = false;
		originalStepOffset = controller.stepOffset;
	}

	void Update()
	{
		if (Input.GetKey(KeyCode.LeftShift))
		{
			run = true;
		}
		else
		{
			run = false;
		}
		if (Input.GetKeyDown(KeyCode.L))
		{
			fastrun = !fastrun;
		}
		if (Input.GetKeyDown(KeyCode.R))
		{
			Reload();
		}



	}

	private void FixedUpdate()
	{
		PCKeyboardInputTest();
		//	PCKeyboardInput();
		PCCharacterRotation();
		PCCameraRotation();
		AnimationPlay();
	}

	void Reload()
	{
		if (Player != null)
		{
			Player.SendReload(0);
		}
	}

	public void PCCharacterRotation()
	{
		float _yRotation = Input.GetAxisRaw("Mouse X");
		Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f);
		transform.Rotate(_characterRotationY);
	}

	public void PCCameraRotation()
	{
		GameObject PChead = Player.HeadObj.transform.Find("PCHead").gameObject;
		float _xRotation = Input.GetAxisRaw("Mouse Y");//마우스는 3차원이 아님 x와y밖에 없음
		float _cameraRotationX = _xRotation;//천천히 움직이게
		currentCameraRotationX -= _cameraRotationX;//마우스와 시야가 반전 되었기 때문에 마이너스를 해줌
		currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -45, 45);//무작정 더하지 못하고 limit 걸리게 하기 위함
		PChead.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);//currentCameraRotationX를 theCamera에 적용 시키기
	}

	public void PCKeyboardInput()
	{
		if (controller == null)
			return;
		if (fastrun)
		{
			Speed = 0.7f;
		}
		else
		{
			Speed = (run) ? runSpeed : walkSpeed;
		}

		isMoving = false;
		if (Input.GetKey(KeyCode.S))
		{
			Dir = new Vector3(humanModel.transform.forward.x, 0, humanModel.transform.forward.z);
			Debug.Log(humanModel.transform.forward);
			Debug.Log(humanModel.transform.forward.x);
			Debug.Log(humanModel.transform.forward.z);
			Debug.Log(Dir);

			Moving(Dir);
		}
		if (Input.GetKey(KeyCode.W))
		{
			Dir = -new Vector3(humanModel.transform.forward.x, 0, humanModel.transform.forward.z);
			Moving(Dir);
		}
		if (Input.GetKey(KeyCode.A))
		{
			Dir = new Vector3(humanModel.transform.right.x, 0, humanModel.transform.right.z);
			Moving(Dir);
		}
		if (Input.GetKey(KeyCode.D))
		{
			Dir = -new Vector3(humanModel.transform.right.x, 0, humanModel.transform.right.z);
			Moving(Dir);
		}
	}
	//--------------------------------------------------------------------------------------------
	//Test
	public void PCKeyboardInputTest()
	{
		Debug.Log("들어옴");
		if (controller == null)
			return;
		if (fastrun)
		{
			Speed = 15f;
		}
		else
		{
			Speed = (run) ? runSpeed : walkSpeed;
		}
		Debug.Log("들어옴 스피드 받음");

		isMoving = false;

		float horizontalInput = Input.GetAxisRaw("Horizontal");
		float verticalInput = Input.GetAxisRaw("Vertical");

		Vector3 _moveHorizontal = transform.right * horizontalInput;
		Vector3 _moveVertical = transform.forward * verticalInput;

		ySpeed += Physics.gravity.y * Time.deltaTime;

		if (controller.isGrounded)
		{
			controller.stepOffset = originalStepOffset;
			ySpeed = -0.5f;
			if (Input.GetKeyDown(KeyCode.Space))
			{
				ySpeed = JumpSpeed;
			}
		}
		else
		{
			controller.stepOffset = 0;
		}



		Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * Speed;
		_velocity.y = ySpeed;


		controller.Move(_velocity * Time.deltaTime);
		Debug.Log("움직여랏");
	}
	//-----------------------------------------------------------------------------------------------------------
	void Moving(Vector3 Dir)
	{
		if (controller.isGrounded == true)
		{
			controller.Move(-Dir * Speed + gravity);
		}
		else
		{
			controller.Move(gravity);
		}
		isMoving = true;
	}

	public void AnimationPlay()
	{
		if (Player.animator == null)
		{
			return;
		}
		Player.animator.SetBool("bRun", run && isMoving);
		Player.animator.SetBool("bWalk", !run && isMoving);
	}

	void SendTurnOnAnimator()
	{
		aView.RPC("TurnOnAnimatorRPC", PhotonTargets.All);
	}

	[PunRPC]
	void TurnOnAnimatorRPC()
	{
		TurnOnAnimator();
	}

	void TurnOnAnimator()
	{
		Player.animator.enabled = true;
	}

}

*/