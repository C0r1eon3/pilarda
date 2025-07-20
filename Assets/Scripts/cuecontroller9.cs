using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CueController : MonoBehaviour
{
    public Transform whiteBall;
    public GameObject cueVisual;
    public float power = 10f;
    public float maxPower = 15f;
    public Transform pottedBallArea;

    private Vector2 shootDirection;
    private bool isAiming = true;
    public List<Rigidbody2D> allBalls = new List<Rigidbody2D>();

    void Start()
    {
        cueVisual.transform.localPosition = new Vector3(-0.75f, 0f, 0f);
        FillAllBallsList();

        if (AllBallsStoppedIncludingWhite())
        {
            cueVisual.SetActive(true);
            isAiming = true;
        }
        else
        {
            cueVisual.SetActive(false);
            isAiming = false;
        }
    }

    void Update()
    {
        if (!isAiming)
        {
            if (AllBallsStoppedIncludingWhite())
            {
                cueVisual.SetActive(true);
                isAiming = true;
            }
            return;
        }

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        shootDirection = (Vector2)whiteBall.position - mousePos;

        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        transform.position = whiteBall.position;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(Shoot());
        }
    }

    private IEnumerator Shoot()
    {
        isAiming = false;
        cueVisual.SetActive(false);

        Vector2 force = shootDirection.normalized * Mathf.Min(shootDirection.magnitude * power, maxPower);
        whiteBall.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);

        yield return null;
    }

    private bool AllBallsStoppedIncludingWhite()
    {
        for (int i = allBalls.Count - 1; i >= 0; i--)
        {
            if (allBalls[i] == null)
            {
                allBalls.RemoveAt(i);
                continue;
            }

            if (allBalls[i].linearVelocity.magnitude > 0.01f)
                return false;
        }

        Rigidbody2D whiteRb = whiteBall.GetComponent<Rigidbody2D>();
        if (whiteRb != null && whiteRb.linearVelocity.magnitude > 0.01f)
            return false;

        return true;
    }

    public void OnBallPotted(Rigidbody2D ballRb)
    {
        ballRb.linearVelocity = Vector2.zero;
        ballRb.angularVelocity = 0;
        ballRb.GetComponent<Collider2D>().enabled = false;
        ballRb.GetComponent<SpriteRenderer>().enabled = false;

        ballRb.transform.SetParent(pottedBallArea);
        int index = pottedBallArea.childCount - 1;
        ballRb.transform.localPosition = new Vector3(0.5f * index, 0, 0);
    }

    private void FillAllBallsList()
    {
        allBalls.Clear();
        GameObject[] ballObjects = GameObject.FindGameObjectsWithTag("Ball");
        foreach (GameObject ballObj in ballObjects)
        {
            Rigidbody2D rb = ballObj.GetComponent<Rigidbody2D>();
            if (rb != null && rb != whiteBall.GetComponent<Rigidbody2D>())
            {
                allBalls.Add(rb);
            }
        }
    }
}

