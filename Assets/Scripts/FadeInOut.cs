using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeInOut : MonoBehaviour {
	
    public static bool sceneEnd;
    public float fadeSpeed = 7f;
    public int nextLevel;
    private Image _image;
    private bool sceneStarting;

    void Awake ()
    {
        _image = GetComponent<Image>();
        _image.enabled = true;
        sceneStarting = true;
        sceneEnd = false;
        Cursor.visible = false;
    }

    void Update ()
    {
        if(sceneStarting) StartScene();
        if(sceneEnd) EndScene();
    }

    void StartScene ()
    {
        _image.color = Color.Lerp(_image.color, Color.clear, fadeSpeed * Time.deltaTime);

        if(_image.color.a <= 0.01f)
        {
            _image.color = Color.clear;
            _image.enabled = false;
            sceneStarting = false;
            Cursor.visible = true;
        }
    }

    void EndScene ()
    {
        _image.enabled = true;
        _image.color = Color.Lerp(_image.color, Color.white, fadeSpeed * Time.deltaTime);

        if(_image.color.a >= 0.95f)
        {
            Cursor.visible = false;
            _image.color = Color.black;
            Application.LoadLevel(nextLevel);
        }
    }
}
