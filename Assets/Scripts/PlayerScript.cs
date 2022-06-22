using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class PlayerScript : MonoBehaviour 
{

	public float speed = 2.0f;
	
	public DetectionBarScript detection;
	
	public AudioClip getKeySound;
	public AudioClip musicNormal;
	public AudioClip musicPanic;
	public AudioClip laserOff;
	public AudioClip laserOn;
	public AudioClip gameOver;
	public GameObject player;
	public GameObject foot;
	public GameObject openDoor;
	public GameObject exitDoor;
	public GameObject laser1;
	public GameObject laser2;
	public GameObject laser3;

	private enum AudioEnum
	{
		Normal,
		Panic
	}
	
	private AudioEnum _currentAudio = AudioEnum.Normal;

	private enum WalkSpeed
	{
		Stay,
		Normal,
		Run
	}
	
	private WalkSpeed _currentWalkSpeed = WalkSpeed.Normal;
	
	[SerializeField]
	private bool takeKey = false;
	
	private bool _runPlayer = false;
	private bool _isMove = false;
	private bool _takePaper = false;
	private bool _laserOn1 = true;
	private bool _laserOn2 = true;
	private bool _laserOn3 = true;
	private bool _takeTool = false;
	private AudioSource _audio;
	
	private void Start()
	{
		if (detection == null)
			detection = GetComponent<DetectionBarScript>();
		_audio = GetComponent<AudioSource>();
		StartCoroutine (nameof(WalkSound));
	}
	
	private void RunningDetection()
	{
		detection.ScrollInvisibleBar();
		if (_runPlayer && _isMove) detection.AlarmDetection(0.5f);
		else if (!_runPlayer) detection.DisableAlarmDetection(0.5f);
	}

	private void RunningKey()
	{
		if (Input.GetKey(KeyCode.LeftShift))
		{
			speed = 10;
			_runPlayer = true;
			_currentWalkSpeed = WalkSpeed.Run;
		}
		else
		{
			speed = 5;
			_runPlayer = false;
			_currentWalkSpeed = WalkSpeed.Normal;
		}
	}

	private void MoveKey()
	{
		if (Input.GetKey(KeyCode.W))
		{
			_isMove = true;
			transform.Translate(Vector3.forward * (speed * Time.deltaTime));
		}
		if (Input.GetKey(KeyCode.S))
		{
			_isMove = true;
			transform.Translate(Vector3.back * (speed * Time.deltaTime));
		}
		if (Input.GetKey(KeyCode.A))
		{
			_isMove = true;
			transform.Translate(Vector3.left * (speed * Time.deltaTime));
		}
		if (Input.GetKey(KeyCode.D))
		{
			_isMove = true;
			transform.Translate(Vector3.right * (speed * Time.deltaTime));
		}
		if (!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
		{
			_isMove = false;
			_currentWalkSpeed = WalkSpeed.Stay;
		}
	}
	
	private void MusicDetection()
	{
		if (detection.invisible >= 75 && _currentAudio != AudioEnum.Panic)
		{
			_audio.clip = musicPanic;
			_audio.Play();
			_currentAudio = AudioEnum.Panic;
		}
		else if (detection.invisible < 75 && _currentAudio != AudioEnum.Normal)
		{
			_audio.clip = musicNormal;
			_audio.Play();
			_currentAudio = AudioEnum.Normal;
		}
		
		if (detection.invisible < 10)
			GameManager.gm.ActiveAlarm(false);
	}

	private void OutOfGame()
	{
		if (detection.invisible >= 100)
		{
			GameManager.gm.SetMsg ("Game Over!");
			AudioSource audio = player.GetComponent<AudioSource>();
			audio.clip = gameOver;
			audio.Play();
			audio.SetScheduledStartTime(6.0f);
			StartCoroutine(nameof(RestartGame));
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Exit"))
		{
			if (_takePaper)
			{
				exitDoor.GetComponent<Animator>().SetTrigger("isEnter");
				GameManager.gm.SetMsg("Thanks to play");
				StartCoroutine(nameof(RestartGame));
			}
		}

	}

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.CompareTag("Key"))
			TakeKey(other);
		else if (other.gameObject.CompareTag("Tool"))
			Tool(other);
		else if (other.gameObject.CompareTag("Laser1"))
			_laserOn1 = Laser(laser1, _laserOn1);
		else if (other.gameObject.CompareTag("Laser2"))
			_laserOn2 = Laser(laser2, _laserOn2);
		else if (other.gameObject.CompareTag("Laser3"))
			_laserOn3 = Laser(laser3, _laserOn3);
		else if (other.gameObject.CompareTag("Papers") && !_takePaper)
			Papers();
		else if (other.gameObject.CompareTag("Terminal"))
			Terminal();
		else if (other.gameObject.CompareTag("light"))
			detection.AlarmDetection(.5f);

	}

	private void Tool(Component other)
	{
		if (!_takeTool)
		{
			GameManager.gm.SetMsg("Press E to take hack tool");
			if (Input.GetKey(KeyCode.E))
			{
				StartCoroutine(DestroyObject(other.gameObject));
				_takeTool = true;
				GameManager.gm.SetMsg("Use hack tool for lasers");
			}
		}
	}
	
	private bool Laser(GameObject laser, bool laserOn)
	{
		if (laserOn && !_takeTool)
			GameManager.gm.SetMsg("Find hack tool");
		else if (laserOn && _takeTool)
		{
			GameManager.gm.SetMsg("Press E to turn off the laser");
			if (Input.GetKey(KeyCode.E))
			{
				laserOn = false;
				laser.GetComponent<Animator>().SetTrigger("LaserOpen");
				AudioSource audio = laser.GetComponent<AudioSource>();
				audio.loop = false;
				audio.clip = laserOff;
				audio.Play();
				GameManager.gm.SetMsg("Laser turn off");
			}
		}
		return laserOn;
	}
	
	private void Papers()
	{
		GameManager.gm.SetMsg("Press E to take paper !");
		if (Input.GetKey(KeyCode.E))
		{
			_takePaper = true;
			GameManager.gm.SetMsg("Well DONE Agent, Go out KNOW");
		}
	}
	
	private void Terminal()
	{
		if (takeKey)
		{
			GameManager.gm.SetMsg("Press E to open Door !");
			if (Input.GetKey(KeyCode.E))
			{
				openDoor.GetComponent<Animator>().SetTrigger("openDoor");
				openDoor.GetComponent<AudioSource>().Play();
			}

		}
		else
			GameManager.gm.SetMsg("Need to Badge to open door");
	}
	
	private void TakeKey(Component other)
	{
		if (!takeKey)
		{
			GameManager.gm.SetMsg("Press E to take KeyCard");
			if (Input.GetKey(KeyCode.E))
			{
				AudioSource audio = other.GetComponent<AudioSource>();
				audio.Play();
				StartCoroutine(DestroyObject(other.gameObject));
				takeKey = true;
				GameManager.gm.SetMsg("You Can Open The Door ! Find the Docs !");
			}
		}
	}
	
	private void OnTriggerExit(Collider other)
	{
		detection.catchPlayer = false;
	}
	
	private void Update()
	{
		RunningDetection();
		RunningKey();
		MoveKey();
		MusicDetection();
		OutOfGame();
	}

	private IEnumerator WalkSound()
	{
		while (true)
		{
			float wait = 0;
			switch (_currentWalkSpeed)
			{
				case WalkSpeed.Normal:
					foot.GetComponent<AudioSource>().Play();
					wait = 0.5f;
					break;
				case WalkSpeed.Run:
					foot.GetComponent<AudioSource>().Play();
					wait = 0.2f;
					break;
				default:
					foot.GetComponent<AudioSource>().Stop();
					break;
			}
			yield return new WaitForSeconds (wait);
		}
	}

	private IEnumerator RestartGame()
	{
		yield return new WaitForSeconds (4);
		SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex);
	}

	private IEnumerator DestroyObject(GameObject key)
	{
		yield return new WaitForSeconds (.5f);
		Destroy (key);
	}
	
}
