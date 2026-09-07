using UnityEngine;

public class MenuParralax : MonoBehaviour
{
    private float startPosX;
    private float startPosY;
    private float length;
    private Vector2 screenPos = Vector2.zero;

    public float parralaxEffect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosX=transform.position.x;
        startPosY = transform.position.y;

    }

    // Update is called once per frame
    void LateUpdate()
    {
        
        //vv- 0=move with cam / 1=wont move / 0.5=half 
        float distX = screenPos.x * parralaxEffect;
        float distY = screenPos.y * parralaxEffect;
        float movement = screenPos.x * (1 - parralaxEffect);
        screenPos = Input.mousePosition; 

        transform.position = new Vector3(startPosX - distX, startPosY - distY,transform.position.z);

        // if (movement > startPosX + length)
        // {
        //     startPosX += length;
        // }
        // else if (movement < startPosX - length) 
        // {
        //     startPosX -= length;
        // }
    }
}
