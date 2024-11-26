using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LookToWalk : MonoBehaviour
{
    enum MoveMode
    {
        Stand, Walk, Run
    }
    private MoveMode moveMode = MoveMode.Stand;
    private Camera mainCamera;
    [SerializeField] private float walkingSpeed = 2.5f;
    [SerializeField] private float minimumAngleTreshold = 35.0f;
    [SerializeField] private float maximumAngleTreshold = 90.0f;
	[SerializeField] private float minimumAngleRunTreshold = 70.0f;
	[SerializeField] private AudioClip[] sndWalkOnGrass;
	[SerializeField] private AudioClip[] sndRunOnGrass;
    [SerializeField] private AudioClip sndNoticeEnemy;

    [SerializeField] private Transform flashlight = null;
	[SerializeField] private float flashlightT = 0.0f;
	[SerializeField] private float flashlightS = 0.0f;

    [SerializeField] private AudioSource staticMedium;
    [SerializeField] private AudioSource staticHeavy;

    private bool canMove = true;

    [SerializeField] private Transform AmbienceSounds;
	[SerializeField] private Transform StaticSounds;

	[Range(0, 1)]
    public float SawSlender = 0.0f;
    private bool Startled = false;

	private Rigidbody rb;
	private AudioSource walkingAudioSource;

    private Vector3 targetVelocity = Vector3.zero;

    private Vector3 startPos;

    private HintController hintController;
    private bool LearnedHowToWalk = false;
    private Paper[] papers = null;
    private bool LearnedHowToCollectPapers = false;

    void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
        walkingAudioSource = GetComponent<AudioSource>();

        startPos = transform.position;

        hintController = GameObject.FindFirstObjectByType<HintController>();

        canMove = false;
		Invoke("ShowHowToMove", 1);

        GameData.PagesCollected = 0;
        GameData.TimeStart = DateTime.Now;
	}

    private void ShowHowToMove()
    {
		hintController.ShowText("Move by looking at the ground");
        canMove = true;
	}

	void Update()
    {
        if (canMove && (mainCamera.transform.eulerAngles.x >= minimumAngleTreshold 
                    && mainCamera.transform.eulerAngles.x <= maximumAngleTreshold))
        {
            moveMode = mainCamera.transform.eulerAngles.x >= minimumAngleRunTreshold ? MoveMode.Run : MoveMode.Walk;
        }
        else
        {
			moveMode = MoveMode.Stand;
        }

        CheckIfPlayerLearnedHowToWalk();
        CheckIfShouldShowHintForCollectingPlayers();
        HandleStaticSounds();


		Cursor.lockState = CursorLockMode.Locked;
	}

	private void CheckIfPlayerLearnedHowToWalk()
	{
		if (!LearnedHowToWalk && (startPos - transform.position).magnitude > 20)
		{
			hintController.HideText();
			LearnedHowToWalk = true;
		}
	}

    private void CheckIfShouldShowHintForCollectingPlayers()
    {
		// Can't do it in Start because PaperHolder hasn't decided which paper of the bunch to "spawn".
		if (papers == null)
		{
			papers = GameObject.FindObjectsOfType<Paper>(false);
		}

		if (!LearnedHowToCollectPapers)
        {
            if (papers.Where(p => (p.transform.position - transform.position).magnitude <= 20).Any()) {
				hintController.ShowText("Collect the paper by looking at it");
                LearnedHowToCollectPapers = true;
			}
        }
	}

	private void HandleStaticSounds()
    {
        float desiredMediumVolume = SawSlender > 0.15f ? (1.0f) : (0.0f);
		float desiredHeavyVolume = SawSlender > 0.55f ? (1.0f) : (0.0f);

		staticMedium.volume = Mathf.Lerp(staticMedium.volume, desiredMediumVolume, 0.75f * Time.deltaTime);
		staticHeavy.volume = Mathf.Lerp(staticHeavy.volume, desiredHeavyVolume, 0.95f * Time.deltaTime);
	}

	private void FixedUpdate()
    {
		if (moveMode == MoveMode.Walk)
        {
            MovePlayer(1);
			SwayFlashlight(0.25f, 7f);

			if (!walkingAudioSource.isPlaying)
            {
                int clipIndex = UnityEngine.Random.Range(0, sndWalkOnGrass.Length);
                walkingAudioSource.PlayOneShot(sndWalkOnGrass[clipIndex]);
            }
        }
        else if (moveMode == MoveMode.Run)
        {
            MovePlayer(1.85f);
			SwayFlashlight(0.35f, 13f);

			if (!walkingAudioSource.isPlaying)
            {
                int clipIndex = UnityEngine.Random.Range(0, sndRunOnGrass.Length);
                walkingAudioSource.PlayOneShot(sndRunOnGrass[clipIndex]);
            }
        }
        else
        {
            SwayFlashlightStop();
			targetVelocity = Vector3.zero;
        }

		rb.velocity = Vector3.Lerp(rb.velocity, targetVelocity, 0.25f);
	}

    private void MovePlayer(float multiplier)
    {
        Vector3 movementVector = new Vector3(mainCamera.transform.forward.x, 0, mainCamera.transform.forward.z);
		targetVelocity = walkingSpeed * multiplier * movementVector.normalized;
    }

    private void SwayFlashlight(float intensity, float speed)
    {
        if (flashlight == null)
        {
            return;
        }

        flashlightS = Mathf.Lerp(flashlightS, speed, 0.1f);
		flashlightT += Time.deltaTime * flashlightS;

		float newX = Mathf.Cos(flashlightT) * intensity;
		float newY = Mathf.Sin(flashlightT * 2) * intensity * 0.44f;
		float newZ = Mathf.Cos(flashlightT * 2) * intensity * 0.25f;

		flashlight.localPosition = new Vector3(newX, newY, newZ);
    }

    private void SwayFlashlightStop()
    {
		if (flashlight == null)
		{
			return;
		}

		flashlightS = Mathf.Lerp(flashlightS, 0f, 0.25f);
	}

    public void OnNotNoticeEnemy(Enemy enemy)
    {
		float distanceToEnemy = (transform.position - enemy.transform.position).magnitude;

        if (distanceToEnemy > 30)
        {
		    SawSlender -= 0.1f * Time.deltaTime;
            if (SawSlender <= float.Epsilon)
            {
                SawSlender = 0.0f;
                Startled = false;
		    }
        }
	}

    public void OnNoticeEnemy(Enemy enemy)
    {
		float distanceToEnemy = (transform.position - enemy.transform.position).magnitude;

		if (SawSlender <= float.Epsilon && !Startled)
        {
            if (distanceToEnemy < 40)
            {
                SawSlender += 0.015f;
			    AudioSource.PlayClipAtPoint(sndNoticeEnemy, transform.position, 1);
                Startled = true;
            }
        }
        else
        {   
            float targetSawValue = SawSlender;
            float sawSpeed = 0.05f;

            if (distanceToEnemy < 15)
            {
                targetSawValue = 1.0f;
                sawSpeed = 0.3f;

			}
            else if (distanceToEnemy < 40)
            {
                targetSawValue = 0.75f;
				sawSpeed = 0.25f;
			}
            else
            {
                targetSawValue = 0.5f;
            }

            SawSlender += sawSpeed * Time.deltaTime;

			if (SawSlender >= targetSawValue)
			{
                SawSlender = targetSawValue;
			}
			if (SawSlender >= 1.0f)
            {
                Die();
            }
        }
    }

    private void Die()
    {
		GameData.TimeEnd = DateTime.Now;
        SceneManager.LoadScene("death", LoadSceneMode.Single);
	}
}
