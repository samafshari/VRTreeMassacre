using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class Player : MonoBehaviour
{
    public static bool IsPlaying;

    public AudioClip DeathSound;
    public AudioClip MenuMusic;
    public AudioClip GameMusic;

    const float moveRange = 1;
    float moveSpeed = 0.1f;
    bool isAlive = true;
    AudioSource audioSource;
    MeshRenderer mshBlammo, mshDeathlight, mshTime;
    TextMesh txtTime;
    float time;
    int seconds;
    int high;
    LineRenderer[] lineRenderers;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderers = FindObjectsOfType<LineRenderer>();
        if (IsPlaying)
        {
            foreach (var item in lineRenderers)
                item.gameObject.SetActive(false);
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

        var thumbstick = GetThumbstickX(XRNode.LeftHand) + GetThumbstickX(XRNode.RightHand);

        if (Input.GetKey(KeyCode.LeftArrow))
            transform.position += Vector3.left * moveSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.RightArrow))
            transform.position += Vector3.right * moveSpeed * Time.deltaTime;
        
        if (Mathf.Abs(thumbstick) > 0.1f)
        {
            transform.position += Vector3.right * thumbstick * moveSpeed * Time.deltaTime;
            txtTime.text = $"{thumbstick}, {thumbstick * moveSpeed * Time.deltaTime}, {thumbstick * moveSpeed}";
        }
    }

    float GetThumbstickX(XRNode node)
    {
        var hands = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(node, hands);
        foreach (var item in hands)
        {
            if (!item.isValid) continue;
            if (item.TryGetFeatureValue(CommonUsages.primary2DAxis, out var v))
                return v.x;
        }
        return 0;
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
        yield return new WaitForSeconds(1);

        Restart();
    }

    void Vibrate()
    {
        var nodes = new[] { XRNode.RightHand, XRNode.LeftHand, XRNode.Head };
        foreach (var node in nodes)
        {
            InputDevice device = InputDevices.GetDeviceAtXRNode(node);
            HapticCapabilities capabilities;
            if (device.TryGetHapticCapabilities(out capabilities))
            {
                if (capabilities.supportsImpulse)
                {
                    uint channel = 0;
                    float amplitude = 0.5f;
                    float duration = 1.0f;
                    device.SendHapticImpulse(channel, amplitude, duration);
                }
            }
        }
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
