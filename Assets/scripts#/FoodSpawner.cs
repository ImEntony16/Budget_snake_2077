using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public float minX = -5f, maxX = 5f;
    public float minY = -3f, maxY = 3f;

    public void MoveFood()
    {
        float x = Random.Range(minX, maxX);
        float y = Random.Range(minY, maxY);
        transform.position = new Vector3(x, y, 0);

        Debug.Log("Їжа перемістилася в: " + transform.position);
    }
}