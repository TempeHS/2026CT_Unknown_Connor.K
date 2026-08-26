using UnityEngine;

public class CheckPoint : MonoBehaviour, IInteractable
{
    public GameObject interactOutline;
    public string checkpointID{ get; private set;}
    [SerializeField] private PlayerMovement Player;
    [SerializeField] private ParticleSystem SaveParticle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        checkpointID ??= GlobalHelper.GenerateUniqueId(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool canInteract()
    {
        return true;
    }
    public void outline() 
    {
        interactOutline.SetActive(true);
    }
    public void unoutline()
    {
        interactOutline.SetActive(false);
    }

    public void interact()
    {
        if (!canInteract()) return;
        saveGame();
    }
    private void saveGame()
    {
        Debug.Log("saved at object" + checkpointID);
        Player.respawnPos = transform.position;
        SaveParticle.Play();
    }
}
