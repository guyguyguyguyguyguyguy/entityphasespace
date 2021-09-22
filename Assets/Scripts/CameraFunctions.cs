using UnityEngine;
using System.Collections;

public class CameraFunctions : MonoBehaviour {
    
    private Camera cam;
    private float targetZoom;
    private float maxOrtho;
    public float minOrtho = 40.0f;
    private float zoomFactor = 300f;
    [SerializeField] private float zoomLerpSpeed = 7;

    public float speed = 10f;

    void Start()
    {
        cam  = Camera.main;
        maxOrtho = targetZoom = cam.orthographicSize;
    }

    void Update()
    {
        Zoom();
        MoveWithMouse();
    }

    void Zoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        targetZoom -= scroll * zoomFactor;
        targetZoom = Mathf.Clamp(targetZoom, minOrtho, maxOrtho);

        if (Time.timeScale == 0)
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, 15 * zoomLerpSpeed);
        else 
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomLerpSpeed);
    }

    void MoveWithMouse() 
    {
        Vector3 move = new  Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
        transform.position += move * speed;
    }
}