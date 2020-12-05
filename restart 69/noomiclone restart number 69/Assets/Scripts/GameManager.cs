using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("MainScene");
        }
    }

    public void resetScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void tuckButton(bool tuck)
    {
        PlayerController.instance.tuckButton = tuck;
    }
    public void archButton(bool arch)
    {
        PlayerController.instance.archButton = arch;
    }
    public void letGoButton(bool letGo)
    {
        PlayerController.instance.letGoButton = letGo;
    }
}
