using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class Player : GameController
{
    public static bool IsPlaying;

    public AudioClip DeathSound;
    public AudioClip MenuMusic;
    public AudioClip GameMusic;

    const float moveRange = 15;
    float moveSpeed = 2;
    bool isAlive = true;
    AudioSource audioSource;
    MeshRenderer mshBlammo, mshDeathlight, mshTime;
    TextMesh txtTime;
    float time;
    int seconds;
    int high;
    XRInteractorLineVisual[] handLines;
    // Start is called before the first frame update
    void Start()
    {
        handLines = FindObjectsOfType<XRInteractorLineVisual>();
        if (IsPlaying)
        {
            foreach (var item in handLines)
                item.enabled = false;
        }
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
        var musicAudioSource = gameObject.AddComponent<AudioSource>();
        musicAudioSource.clip = IsPlaying ? GameMusic : MenuMusic;
        musicAudioSource.loop = true;
        musicAudioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.f12Key.wasPressedThisFrame)
            ScreenCapture.CaptureScreenshot(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), DateTime.Now.Ticks.ToString() + ".png"));

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
            txtTime.text = $"High: {high}\nYou: {seconds}";
        }

        var thumbstick = GetThumbstickX();

        if (Mathf.Abs(thumbstick) > 0.1f)
        {
            transform.position += Vector3.right * thumbstick * moveSpeed * Time.deltaTime;
            if (transform.localPosition.x < -moveRange)
                transform.localPosition = new Vector3(-moveRange, transform.localPosition.y, transform.localPosition.z);
            else if (transform.localPosition.x > moveRange)
                transform.localPosition = new Vector3(moveRange, transform.localPosition.y, transform.localPosition.z);
            //txtTime.text = $"{thumbstick}, {thumbstick * moveSpeed * Time.deltaTime}, {thumbstick * moveSpeed}";
        }
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
        IsPlaying = false;
        Vibrate();
        audioSource.PlayOneShot(DeathSound);
        mshDeathlight.enabled = true;
        mshBlammo.enabled = true;
        mshTime.enabled = false;
        yield return new WaitForSeconds(2);

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
