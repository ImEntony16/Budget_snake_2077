using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SmoothSnake : MonoBehaviour
{
    [Header("Звукові ефекти")]
    private AudioSource audioSource; 

    [Header("Налаштування смерті від хвоста")]
    public int safeSegments = 10;
    public float deathRadius = 0.5f;

    [Header("Налаштування руху")]
    public float speed = 5f;
    public int gap = 10;

    [Header("Тіло змії")]
    public GameObject bodyPrefab;
    public int initialBodyParts = 3;

    private Vector2 direction = Vector2.up;
    private List<Marker> marks = new List<Marker>();
    private List<Transform> bodyParts = new List<Transform>();

    private struct Marker
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        for (int i = 0; i < 200; i++)
        {
            marks.Add(new Marker { position = transform.position, rotation = transform.rotation });
        }

        for (int i = 0; i < initialBodyParts; i++)
        {
            GrowSnake();
        }
    }

    void Update()
    {
 
        if (Input.GetKeyDown(KeyCode.W) && direction != Vector2.down) direction = Vector2.up;
        if (Input.GetKeyDown(KeyCode.S) && direction != Vector2.up) direction = Vector2.down;
        if (Input.GetKeyDown(KeyCode.A) && direction != Vector2.right) direction = Vector2.left;
        if (Input.GetKeyDown(KeyCode.D) && direction != Vector2.left) direction = Vector2.right;

        transform.position += (Vector3)direction * speed * Time.deltaTime;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);

  
        marks.Insert(0, new Marker { position = transform.position, rotation = transform.rotation });


        for (int i = 0; i < bodyParts.Count; i++)
        {
            int markerIndex = (i + 1) * gap;
            if (markerIndex < marks.Count)
            {
                bodyParts[i].position = marks[markerIndex].position;
                bodyParts[i].rotation = marks[markerIndex].rotation;
            }
        }

        if (marks.Count > (bodyParts.Count + 1) * gap + 50)
        {
            marks.RemoveAt(marks.Count - 1);
        }

        for (int i = 4; i < bodyParts.Count; i++)
        {
            float distance = Vector3.Distance(transform.position, bodyParts[i].position);
            if (distance < 0.5f)
            {
                Debug.Log("Вкусив себе за хвіст!");
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }

    public void GrowSnake()
    {
        if (bodyPrefab != null)
        {
            GameObject body = Instantiate(bodyPrefab, transform.position, transform.rotation);
            bodyParts.Add(body.transform);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Food"))
        {
            if (audioSource != null)
            {
                audioSource.pitch = Random.Range(0.85f, 1.15f);
                audioSource.Play();
            }

            GrowSnake();

            FoodSpawner fs = other.GetComponent<FoodSpawner>();
            if (fs != null)
            {
                fs.MoveFood();
            }
            else
            {
                other.gameObject.SendMessage("MoveFood", SendMessageOptions.DontRequireReceiver);
            }

            return;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}