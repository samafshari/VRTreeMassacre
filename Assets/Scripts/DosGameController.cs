using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DosGameController : GameController
{
    public Sprite LeftSprite, StraightSprite;

    Image imgPlayer, imgTreePrefab, imgGameBackground;
    Text lblBlammo;
    List<Image> imgTrees = new List<Image>();
    int direction;
    const float playerSpeed = 100;
    const float treeSpeed = 80;
    const float xMin = 109;
    const float xMax = 212;
    const int maxTrees = 7;
    bool isPlaying = false;

    // Start is called before the first frame update
    void Start()
    {
        imgPlayer = transform.Find("Player").GetComponent<Image>();
        imgTreePrefab = transform.Find("TreePrefab").GetComponent<Image>();
        imgGameBackground = transform.Find("Game Background").GetComponent<Image>();
        lblBlammo = transform.Find("lblBlammo").GetComponent<Text>();
        Reset();
    }

    private void Reset()
    {
        isPlaying = true;
        lblBlammo.enabled = false;
        direction = 0;
        imgPlayer.transform.localPosition = new Vector3(xMin + 0.5f * (xMax - xMin), imgPlayer.rectTransform.localPosition.y, 0);
        for (int i = 0; i < imgTrees.Count; i++)
            Destroy(imgTrees[i].gameObject);
        imgTrees.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPlaying) return;
        ReadInput();
        MovePlayer();
        UpdateDirection();
        MoveTrees();
        GenerateTrees();
        CheckGameOver();
    }

    void ReadInput()
    {
        var thumbstick = GetThumbstickX();
        if (thumbstick < -0.2) direction = -1;
        else if (thumbstick > 0.2) direction = 1;
        else direction = 0;
    }

    void MovePlayer()
    {
        imgPlayer.transform.localPosition += Vector3.right * direction * playerSpeed * Time.deltaTime;
        if (imgPlayer.transform.localPosition.x < xMin)
            imgPlayer.transform.localPosition = new Vector3(xMin, imgPlayer.transform.localPosition.y);
        else if (imgPlayer.transform.localPosition.x > xMax)
            imgPlayer.transform.localPosition = new Vector3(xMax, imgPlayer.transform.localPosition.y);
    }

    void MoveTrees()
    {
        for (int i = 0; i < imgTrees.Count; i++)
        {
            var imgTree = imgTrees[i];
            if (imgTree.transform.localPosition.y < -300)
            {
                Destroy(imgTree.gameObject);
                imgTrees.RemoveAt(i--);
                continue;
            }
            if (i == 0) Debug.Log(imgTree.transform.localPosition.y);
            imgTree.transform.localPosition += Vector3.down * Time.deltaTime * treeSpeed;
        }
    }

    void GenerateTrees()
    {
        while (imgTrees.Count < maxTrees)
        {
            var tree = Instantiate(imgTreePrefab.gameObject);
            tree.transform.SetParent(transform, false);
            var imgTree = tree.GetComponent<Image>();
            imgTree.enabled = true;
            imgTrees.Add(imgTree);
            imgTree.transform.localPosition = new Vector3(
                Random.Range(xMin, xMax),
                Random.Range(0, 300));
        }
    }

    void CheckGameOver()
    {
        foreach (var imgTree in imgTrees)
        {
            if ((imgTree.transform.localPosition - imgPlayer.transform.localPosition).magnitude < 10)
            {
                StartCoroutine(DeathProcedure());
                return;
            }
        }
    }

    IEnumerator DeathProcedure()
    {
        isPlaying = false;
        lblBlammo.enabled = true;
        //Vibrate();
        yield return new WaitForSeconds(2);
        Reset();
    }

    void UpdateDirection()
    {
        if (direction == 0)
        {
            imgPlayer.rectTransform.localScale = Vector3.one;
            imgPlayer.sprite = StraightSprite;
        }
        else
        {
            imgPlayer.sprite = LeftSprite;
            if (direction < 0)
                imgPlayer.rectTransform.localScale = Vector3.one;
            else
                imgPlayer.rectTransform.localScale = new Vector3(-1, 1, 1);
        }
    }
}
