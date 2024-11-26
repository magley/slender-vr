using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    private AudioSource sndSource;
    [SerializeField] private float moveTimerMax = 500;
	private float moveTimer = 90;
    private LookToWalk player;
    private CapsuleCollider cld;
    [SerializeField] private int aggression = 1;

	void Start()
    {
        sndSource = GetComponent<AudioSource>();
		cld = GetComponent<CapsuleCollider>();
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<LookToWalk>();
	}

    void FixedUpdate()
    {
        Think();
    }

    private void Think()
    {
        int pagesLeft = GameObject.FindGameObjectsWithTag("Paper").Length;
        int pagesTaken = (6 - pagesLeft);
        aggression = pagesTaken;
		moveTimerMax = 460 - 60 * Mathf.Min(pagesTaken, 6);

        if (!IsInFrustum())
        {
            moveTimer -= 1;
            if (moveTimer < 0)
            {
                moveTimer = moveTimerMax;
                MoveToPlayer();
            }
		}
        
        if (IsVisibleAndNotOccluded())
        {
            player.OnNoticeEnemy(this);
        }
        else
        {
			player.OnNotNoticeEnemy(this);
			LookAtPlayer();
		}
	}

    private void MoveToPlayer()
    {    
        Vector3 newPos = transform.position;

        float prcBonus = 0.05f * aggression;
		float prc = Random.Range(0.25f + prcBonus, 0.55f + prcBonus);
        newPos += (player.transform.position - newPos) * prc;

        PlayBoom();

		transform.position = new Vector3(newPos.x, transform.position.y, newPos.z);
	}

    private void LookAtPlayer()
    {
		transform.LookAt(player.transform);
		Vector3 eulerAngles = transform.rotation.eulerAngles;
		eulerAngles.x = 0;
		eulerAngles.z = 0;
		transform.rotation = Quaternion.Euler(eulerAngles);
	}

    private void PlayBoom()
    {
        AudioManager.Instance.PlaySound(sndSource.clip);
    }

    private bool IsInFrustum()
    {
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        return (GeometryUtility.TestPlanesAABB(planes, cld.bounds));
	}

	private bool IsVisibleAndNotOccluded()
	{
        Camera camera = Camera.main;
		Vector3 viewportPoint = camera.WorldToViewportPoint(transform.position);

		bool isVisible = viewportPoint.x >= 0 && viewportPoint.x <= 1 &&
						 viewportPoint.y >= 0 && viewportPoint.y <= 1 &&
						 viewportPoint.z > 0;

		if (!isVisible)
		{
			return false;
		}

		Vector3 direction = transform.position - camera.transform.position;
		RaycastHit hit;

		if (Physics.Raycast(camera.transform.position, direction, out hit))
		{
			if (hit.transform == transform)
			{
				return true;
			}
		}

		return false;
	}


	public void MaybeTeleportBehindPlayer()
    {
        if (Random.Range(0, 12) <= aggression)
        {
            Vector3 newPos = player.transform.position;
            newPos -= Camera.main.transform.forward * (20 - aggression);
            transform.position = new Vector3(newPos.x, transform.position.y, newPos.z);
		}
    }
}
