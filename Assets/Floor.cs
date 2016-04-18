using UnityEngine;
using System.Collections;
using System;

public class Floor : MonoBehaviour
{
    [SerializeField]
    GameObject @default = null;
    [SerializeField]
    GameObject square = null;
    [SerializeField]
    GameObject circle = null;

    [SerializeField]
    private Collider col = null;

    [SerializeField]
    private MeshRenderer marker = null;

    private Maze.Free cell;
    private CollisionDetection detector;
    private PlayerController player ;

    private MeshFilter[] meshes;

    private GameObject activeMesh;
    private float fallTime;

    public enum CellProperty
    {
        None,
        Start,
        End
    }
    
    public CellProperty Property { private set; get; }
    public Color Color { private set; get; }


    void Awake()
    {
        this.meshes = this.GetComponentsInChildren<MeshFilter>();
        detector = this.GetComponentInChildren<CollisionDetection>();
        detector.TriggerEnter += Detector_TriggerEnter;
        detector.TriggerExit += Detector_TriggerExit;
        this.marker.gameObject.SetActive(false);
    }

    private void Detector_TriggerEnter(Collider obj)
    {
        player = obj.GetComponentInParent<PlayerController>();
        this.Destroy();
    }

    private void Destroy()
    {
        if (this.Property != CellProperty.Start && this.Property != CellProperty.End && this.fallTime >= 0)
        {
            this.StartCoroutine(this.DestroyCoroutine());
        }
    }

    private IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSeconds(this.fallTime);
        this.GetComponent<Rigidbody>().isKinematic = false;
    }

    private void Detector_TriggerExit(Collider obj)
    {
        player = null;
    }

    void Update()
    {

        if (player != null && 
            (
                (this.cell.Type == Maze.Free.Trap.Square && player.IsSquare()) ||
                (this.cell.Type == Maze.Free.Trap.Circle && player.IsCircle()) ||
                (this.cell.Color == Maze.Free.MatColor.Red && player.IsRed()) ||
                (this.cell.Color == Maze.Free.MatColor.Green && player.IsGreen())
            ))
        {
            this.col.enabled = false;
        }
        else
        {
            this.col.enabled = true;
        }

        if(player != null && this.Property == CellProperty.End)
        {
            GameManager.Instance.Win();
            player = null;
        }

    }

    internal void SetFallTime(float fallTime)
    {
        this.fallTime = fallTime;
    }

    private void ApplyColor()
    {
        this.activeMesh.GetComponent<MeshRenderer>().material.color = this.Color;

    }

    public void SetMeshAndColor(Maze.Cell cell)
    {
        this.cell  = (Maze.Free)cell; 

        foreach(MeshFilter mesh in this.meshes)
        {
            mesh.gameObject.SetActive(false);
        }


        if (this.cell.Type == Maze.Free.Trap.Square)
        {
            this.activeMesh = this.square;
        }
        else if (this.cell.Type == Maze.Free.Trap.Circle)
        {
            this.activeMesh = this.circle;
        }
        else
        {
            this.activeMesh = this.@default;
        }
        this.activeMesh.SetActive(true);
                
        switch (this.cell.Color)
        {
            case Maze.Free.MatColor.Red:
                this.Color = Color.red;
                break;
            case Maze.Free.MatColor.Green:
                this.Color = Color.green;
                break;
            case Maze.Free.MatColor.White:
                this.Color = Color.white;
                break;
        }
        this.ApplyColor();

    }

    public void SetStart()
    {
        this.Property = CellProperty.Start;
        this.marker.gameObject.SetActive(true);
        this.marker.material.color = Color.blue;
    }

    public void SetEnd()
    {
        this.Property = CellProperty.End;

        this.marker.gameObject.SetActive(true);
        this.marker.material.color = Color.yellow;
    }
}
