using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerControl : MonoBehaviour
{

	public float xSensitivity = 2f;
	public float ySensitivity = 2f;
	public bool clampVerticalRotation = true;
	public float minimumX = -90F;
	public float maximumX = 90F;
	public bool smooth;
	public float smoothTime = 5f;
	public bool lockCursor = true;

	private Quaternion _mCharacterTargetRot;
	private Quaternion _mCameraTargetRot;
	private bool _mCursorIsLocked = true;
	
	private void Start()
	{
		Init(transform, Camera.main.transform);
	}

	private void Update()
	{
		LookRotation(transform, Camera.main.transform);
	}

	public void Init (Transform character, Transform camera)
	{
		_mCharacterTargetRot = character.localRotation;
		_mCameraTargetRot = camera.localRotation;
	}

	public void LookRotation (Transform character, Transform camera)
	{
		float yRot = Input.GetAxis ("Mouse X") * xSensitivity;
		float xRot = Input.GetAxis ("Mouse Y") * ySensitivity;

		_mCharacterTargetRot *= Quaternion.Euler (0f, yRot, 0f);
		_mCameraTargetRot *= Quaternion.Euler (-xRot, 0f, 0f);

		if (clampVerticalRotation)
			_mCameraTargetRot = ClampRotationAroundXAxis (_mCameraTargetRot);

		if (smooth)
		{
			character.localRotation = Quaternion.Slerp (character.localRotation, _mCharacterTargetRot,
				smoothTime * Time.deltaTime);
			camera.localRotation = Quaternion.Slerp (camera.localRotation, _mCameraTargetRot,
				smoothTime * Time.deltaTime);
		}
		else
		{
			character.localRotation = _mCharacterTargetRot;
			camera.localRotation = _mCameraTargetRot;
		}

		UpdateCursorLock ();
	}

	public void SetCursorLock (bool value)
	{
		lockCursor = value;
		if (!lockCursor)
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}

	public void UpdateCursorLock()
	{
		if (lockCursor)
			InternalLockUpdate();
	}

	private void InternalLockUpdate()
	{
		if (Input.GetKeyUp (KeyCode.Escape))
			_mCursorIsLocked = false;
		else if (Input.GetMouseButtonUp (0))
			_mCursorIsLocked = true;

		if (_mCursorIsLocked)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		else if (!_mCursorIsLocked)
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}

	Quaternion ClampRotationAroundXAxis (Quaternion q)
	{
		q.x /= q.w;
		q.y /= q.w;
		q.z /= q.w;
		q.w = 1.0f;

		float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (q.x);

		angleX = Mathf.Clamp (angleX, minimumX, maximumX);
		q.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);

		return q;
	}
}
