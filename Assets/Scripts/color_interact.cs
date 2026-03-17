using UnityEngine;

public class color_interact : MonoBehaviour
{
    public Renderer handRenderer;     
    private Color collisionColor = Color.red;

    private Color originalColor;

    void Start()
    {
        
        originalColor = handRenderer.material.color;
        collisionColor.a = 0.5f;
    }

    void OnTriggerEnter(Collider other)
    {
       
        handRenderer.material.color = collisionColor;
    }

    void OnTriggerExit(Collider other)
    {
        
        handRenderer.material.color = originalColor;
    }
}