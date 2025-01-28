using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.InputSystem.EnhancedTouch;

public class TouchContactDataClass
{ 
    // information des données stocker dans startTouch
    public Vector2 Position;
    public float Time;
}

[DefaultExecutionOrder(-1)] // definit que ce script s'executera en premier avant les autres = ce qui nous permet de voir les swipe vert et donc supprimer l'erreur.
public class TouchManager : MonoBehaviour
{
    [SerializeField] private InputActionReference m_onTouchContactInput;
    [SerializeField] private InputActionReference m_onTouchPositionInput;
    // singleton = classe statique accessible depuis n'importe quel script, on peut y acceder depuis n'importe où, n'existe quand une seule entité on ne peut pas en avoir plusieurs.
    
    
    private static TouchManager m_instance;
    public static TouchManager Instance { get => m_instance; set => m_instance = value; }

    // event 
    public delegate void StartTouch(TouchContactDataClass contactData); // delegate permet de creer un type de variable qui sera return par des événements
    public event StartTouch OnTouchStartEvent;// event renvoyant un type de donner uniquement grâce à un delegate ligne précédente
    public delegate void EndTouch(TouchContactDataClass contactData);
    public event EndTouch OnTouchEndEvent;

    private void Awake()

    {
        // Mettre un code dans Awake qui vérifie si il existe plusieurs copies de cette entité et si oui détruit celle en trop car sinon bug.
        TouchSimulation.Enable();

        if (m_instance != null)
            Destroy(gameObject);
        m_instance = this;
    }
    private void OnEnable()
    {
        m_onTouchContactInput.action.started += OnTouchStart;
        m_onTouchContactInput.action.canceled += OnTouchEnd;

    }

    private void OnDisable()
    {
        m_onTouchContactInput.action.started -= OnTouchStart;
        m_onTouchContactInput.action.canceled -= OnTouchEnd;
    }

    private void OnTouchStart(InputAction.CallbackContext context)
    {
        // appeler event au touch start
        // stockage dans un data au lieu d'un vecteur = englober dans une classe plus grande contenant + de donner si besoin (soucie organisation)
        TouchContactDataClass data = new TouchContactDataClass();
        data.Position = GetTouchWorldPosition();
        data.Time = (float)context.time;
        if (OnTouchStartEvent != null) OnTouchStartEvent(data);
        Debug.Log("OnTouchStart" + data.Time);
    }

    private void OnTouchEnd(InputAction.CallbackContext context)
    {  // appeler event au touch end
        TouchContactDataClass data = new TouchContactDataClass();
        data.Position = GetTouchWorldPosition();
        // ancin qui appelait une variable isolé Vector2 position = GetTouchWorldPosition();
        data.Time = (float)context.time;
        if (OnTouchEndEvent != null) OnTouchEndEvent(data);

        Debug.Log("OnTouchEnd" + data.Time);
    }

    Vector2 GetTouchWorldPosition()
    {
        return ScreenToWorld(Camera.main, m_onTouchPositionInput.action.ReadValue<Vector2>());
    }

    private Vector3 ScreenToWorld(Camera camera, Vector3 position)
    {
        position.z = -camera.transform.position.z;
        return camera.ScreenToWorldPoint(position);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

