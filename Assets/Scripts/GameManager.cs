using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

	public PlayerScript player;
	public static GameManager gm { get; private set; }
	public Text message;
	public GameObject megaPhone;
	private AudioSource _audioSource;
	private Animator _animator;
	
	private void Awake()
	{
		_audioSource = megaPhone.GetComponent<AudioSource>();
		_animator = message.GetComponent<Animator>();
	}

	private void Start()
	{
		if (gm == null)
			gm = this;
		_animator.SetTrigger("showText");

	}
	
	public void ActiveAlarm(bool isPlay)
	{
		if (isPlay)
		{
			if (!_audioSource.isPlaying)
				_audioSource.Play();
		}
		else
			_audioSource.Stop();
	}
	public void CameraSpotPlayer()
	{
		player.detection.AlarmDetection(20);
	}

	public void SetMsg(string msg)
	{
		message.text = msg;
		_animator.SetTrigger("showText");
	}
}
