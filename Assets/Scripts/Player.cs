using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    float moveSpeed = 0.1f;
    bool isAlive = true;
    AudioSource audioSource;
    MeshRenderer mshBlammo, mshDeathlight, mshTime;
    TextMesh txtTime;
    public AudioClip DeathSound;
    float time;
    int seconds;
    int high;

    public static bool IsPlaying;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        mshBlammo = GameObject.Find("BLAMMO!").GetComponent<MeshRenderer>();
        mshDeathlight = GameObject.Find("DeathLight").GetComponent<MeshRenderer>();
        mshTime = GameObject.Find("Time").GetComponent<MeshRenderer>();
        txtTime = GameObject.Find("Time").GetComponent<TextMesh>();
        mshBlammo.enabled = false;
        mshDeathlight.enabled = false;
        txtTime.text = "";
        time = 0;
        seconds = 0;
        high = PlayerPrefs.GetInt("HighScore", 0);
        GameObject.Find("Main Menu Canvas").GetComponent<Canvas>().enabled = !IsPlaying;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsPlaying) return;
        time += Time.deltaTime;
        if ((int)time > seconds)
        {
            seconds = (int)time;
            if (seconds > high)
            {
                high = seconds;
                PlayerPrefs.SetInt("HighScore", high);
            }
            txtTime.text = $"High: {high}\nScore: {seconds}";
        }

        //if (Input.GetKey(KeyCode.LeftArrow))
        //    transform.position -= new Vector3(moveSpeed, 0, 0);
        //if (Input.GetKey(KeyCode.RightArrow))
        //    transform.position += new Vector3(moveSpeed, 0, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsPlaying) return;
        if (!isAlive) return;
        Debug.Log($"Collider entered: {other.gameObject.name}");
        isAlive = false;
        PlayerPrefs.Save();
        StartCoroutine(DeathProcedure());
    }

    IEnumerator DeathProcedure()
    {
        audioSource.PlayOneShot(DeathSound);
        mshDeathlight.enabled = true;
        mshBlammo.enabled = true;
        mshTime.enabled = false;
        yield return new WaitForSeconds(1);
        Restart();
    }

    public void StartGame()
    {
        IsPlaying = true;
        Restart();
    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
