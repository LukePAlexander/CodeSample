using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenuUIController : MonoBehaviour {

    public string LevelToLoad;
    public bool IsExit;
    public Sprite UnHighLightSprite;
    public Sprite HighLightSprite;
	// Use this for initialization
	void Start () {
	
	}

    void OnMouseDown()
    {
        //Debug.Log("Button Working");
        if (IsExit)
        {
            Application.Quit();
        }
        else
        {
            SceneManager.LoadScene(LevelToLoad);
        }
    }
	
	// Update is called once per frame
	void Update () {

	}

    void OnMouseOver()
    {
        SpriteRenderer spriteRenderer = (SpriteRenderer) GetComponent("SpriteRenderer");
        spriteRenderer.sprite = HighLightSprite;
    }

    void OnMouseExit()
    {
        SpriteRenderer spriteRenderer = (SpriteRenderer)GetComponent("SpriteRenderer");
        spriteRenderer.sprite = UnHighLightSprite;
    }
}
