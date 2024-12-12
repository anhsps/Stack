using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject newCube, oldCube;
    public TextMeshProUGUI scoreText, bestText;
    private int level;
    private bool done;
    private Vector3 newPos => newCube.transform.position;
    private Vector3 oldPos => oldCube.transform.position;
    private Vector3 newScale => newCube.transform.localScale;
    private Vector3 oldScale => oldCube.transform.localScale;

    // Start is called before the first frame update
    void Start()
    {
        NewBlock();
    }

    // Update is called once per frame
    void Update()
    {
        if (done) return;

        // Time.realtimeSinceStartup % 2f : t thực tế từ khi app start chạy vs chu kỳ 2f => (t: 0->2)
        // -1f => (t: -1->1). Abs => (t: 1->0->1)
        var t = Mathf.Abs(Time.realtimeSinceStartup % 2f - 1f);

        var pos = oldPos + Vector3.up * oldScale.y;
        var pos1 = pos + (level % 2 == 0 ? Vector3.right : Vector3.forward) * 60;// .forward: phía trc
        var pos2 = pos + (level % 2 == 0 ? Vector3.left : Vector3.back) * 60;// .back: phía sau
        newCube.transform.position = Vector3.Lerp(pos1, pos2, t);// t 1->0->1 => nội suy tuyến tính từ end(pos2)->start(pos1) trước

        if (Input.GetMouseButtonDown(0))
            NewBlock();

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
                NewBlock();
        }
    }

    private void NewBlock()
    {
        if (oldCube != null)
        {
            newCube.transform.position = new Vector3(Mathf.Round(newPos.x), newPos.y, Mathf.Round(newPos.z));
            newCube.transform.localScale = new Vector3(oldScale.x - Mathf.Abs(newPos.x - oldPos.x),
                oldScale.y, oldScale.z - Mathf.Abs(newPos.z - oldPos.z));
            newCube.transform.position = Vector3.Lerp(newPos, oldPos, 0.5f) + Vector3.up * oldScale.y * 0.5f;

            if (newScale.x <= 0f || newScale.z <= 0f)
            {
                done = true;
                Invoke("Restart", 3f);
                return;
            }
        }

        level++;
        scoreText.text = level - 1 + "";
        UpdateBestScore();
        oldCube = newCube;
        newCube = Instantiate(oldCube);
        newCube.name = level.ToString();
        newCube.GetComponent<MeshRenderer>().material.color = Color.HSVToRGB((level / 50f) % 1f, 1f, 1f);
        Camera.main.transform.position = newCube.transform.position + new Vector3(100, 100, 100);
        Camera.main.transform.LookAt(newCube.transform.position);
    }

    private void Restart() => SceneManager.LoadScene(0);

    private void UpdateBestScore()
    {
        if (level - 1 > PlayerPrefs.GetInt("Best", 0))
            PlayerPrefs.SetInt("Best", level - 1);
        bestText.text = PlayerPrefs.GetInt("Best").ToString();
    }
}
