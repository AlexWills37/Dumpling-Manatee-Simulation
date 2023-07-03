using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExitBehav : MonoBehaviour
{
    private GameObject physicalPlayer;
    [SerializeField]
    private GameObject levelController;
    // Start is called before the first frame update
    void Start()
    {
        physicalPlayer = GameObject.Find("PhysicalPlayer");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == physicalPlayer)
        {
            //If 
            endLevel();
        }
    }

    public void endLevel()
    {
        //whatever we do when switching levels
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
