using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour 
{
    [SerializeField]
    private float gravity = 10;

    [SerializeField]
    private GameObject morphable = null;
    [SerializeField]
    private GameObject hardEdgeCube = null;
    private SkinnedMeshRenderer smr;
    [SerializeField]
    private AnimationCurve morphCurve = null;
    

    private int currentShapeIndex = 0;
    private int currentColorIndex = 0;

    private Color[] colors =
    {
        Color.red,
        Color.green
    };

    private bool isDead = false;
    private Rigidbody rb;
    [SerializeField]
    private float defaultY;

    private Vector3 direction;
    private bool controlDisabled = false;

    private IEnumerator morphing = null;
    private IEnumerator coloring = null;
    private Material material;

    private GameCanvas gameCanvas;

    void Awake()
    {
        this.smr = this.morphable.GetComponent<SkinnedMeshRenderer>();
        this.rb = this.GetComponentInChildren<Rigidbody>();
        SetShape(0);
        this.material = this.smr.sharedMaterial;
        this.material.color = this.colors[0];

        this.StartCoroutine(this.GetDefaultZ());

    }
    void Start()
    {

        this.gameCanvas = FindObjectOfType<GameCanvas>();
    }

    private IEnumerator GetDefaultZ()
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        this.defaultY = rb.position.y;
    }

    public void SetShape(int index)
    {
        this.currentShapeIndex = index;
        if (morphing != null)
        {
            this.StopCoroutine(this.morphing);
        }
        this.morphing = Morph(index * 100);
        this.StartCoroutine(this.morphing);
    }


    private void SetColor(int index)
    {
        this.currentColorIndex = index;
        if (coloring != null)
        {
            this.StopCoroutine(this.coloring);
        }
        this.coloring = DoColor(this.colors[index]);
        this.StartCoroutine(this.coloring);
    }


    public void PlayWin()
    {
        if(this.morphable.activeSelf)
        {
            this.morphable.GetComponent<Animator>().SetTrigger("win");
        }
        else if (this.hardEdgeCube.activeSelf)
        {
            this.hardEdgeCube.GetComponent<Animator>().SetTrigger("win");
        }
    }
    

    private IEnumerator DoColor(Color end)
    {
        Color start = this.material.color;
        const float duration = 0.2f;
        float step = 1/duration;
        float p = 0;
        while (p < 1)
        {
            p = Math.Min(1, p + step * Time.deltaTime);
            this.material.color = Color.Lerp(start, end, p);
            yield return null;
        }

    }
    private IEnumerator Morph(int end)
    {
        this.hardEdgeCube.SetActive(false);
        this.morphable.SetActive(true);
        float start = this.smr.GetBlendShapeWeight(0);
        float distance = Mathf.Abs(end - start);
        const float speed = 500;
        float step = speed / distance;
        float p = 0;
        while (p < 1)
        {
            p = Math.Min(1, p + step * Time.deltaTime);
            float pp = morphCurve.Evaluate(p);
            this.smr.SetBlendShapeWeight(0, Mathf.Lerp(start, end, Mathf.Clamp01(pp) ));

            if (pp < 0)
            {
                if (start < end)
                {
                    this.smr.SetBlendShapeWeight(2, -pp * 100);
                    this.smr.SetBlendShapeWeight(1, 0);
                }
                else
                {
                    this.smr.SetBlendShapeWeight(1, -pp * 100);
                    this.smr.SetBlendShapeWeight(2, 0);
                }
            }
            else if (pp > 1)
            {
                if (start < end)
                {
                    this.smr.SetBlendShapeWeight(1, (pp - 1) * 100);
                    this.smr.SetBlendShapeWeight(2, 0);
                }
                else
                {
                    this.smr.SetBlendShapeWeight(2, (pp - 1) * 100);
                    this.smr.SetBlendShapeWeight(1, 0);
                }
            }
            else
            {
                this.smr.SetBlendShapeWeight(1, 0);
                this.smr.SetBlendShapeWeight(2, 0);
            }
            yield return null;
        }

        this.hardEdgeCube.SetActive(end == 100);
        this.morphable.SetActive(end != 100);

        this.morphing = null;

    }


    private void SnapToClosest()
    {
        Vector3 position = this.rb.transform.position;
        position.x = Mathf.Round(position.x);
        position.z = Mathf.Round(position.z);
        this.SnapTo(position);
    }

    public void SnapTo(Vector3 position)
    {
        this.StartCoroutine(this.SnapToCoroutine(position));
    }

    private IEnumerator SnapToCoroutine(Vector3 target)
    {
        this.rb.isKinematic = true;
        Vector3 start = this.rb.position;
        const float duration = 0.1f;
        float step = 1 / duration;

        float p = 0;
        while (p < 1)
        {
            p = Mathf.Min(1, p + step * Time.deltaTime);
            this.rb.position = Vector3.Lerp(start, target, p);
            yield return null;
        }
        this.rb.isKinematic = false;
        yield return null;



    }

    private void ToggleShape()
    {
        this.SetShape(1 - this.currentShapeIndex);
    }

    private void ToggleColor()
    {
        this.SetColor(1 - this.currentColorIndex);
    }


    public void DisableControls()
    {
        this.controlDisabled = true;
    }

    void Update()
    {
        this.direction = Vector3.zero;
        if (controlDisabled || this.gameCanvas.IsPause) return;
#if UNITY_ANDROID && !UNITY_EDITOR
        this.MobileInputs();
#else
        this.UnityEditorInputs();
#endif
    }

    private void UnityEditorInputs()
    {
        if (this.isDead) return;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            direction.x -= 1;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            direction.x += 1;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            direction.z += 1;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            direction.z -= 1;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.LeftShift))
        {
            this.ToggleShape(); 
        }
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            this.ToggleColor();
        }
    }


    private void MobileInputs()
    {
        this.direction = new Vector3(Input.acceleration.x, 0, Input.acceleration.y);
        foreach(Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                if (touch.position.x < Screen.width/2)
                {
                    this.ToggleShape();
                }
                else
                {
                    this.ToggleColor();
                }
            }
        }
        
    }


    void FixedUpdate()
    {
        if (!this.isDead)
        {
            if (this.rb.position.y < this.defaultY - 0.01f)
            {
                this.isDead = true;
                this.SnapToClosest();
                GameManager.Instance.Lose();
            }
            if (this.direction != Vector3.zero)
            {
                this.rb.AddForce(this.direction.normalized * this.gravity);
            }

        }
        
    }


    public bool IsCircle()
    {
        return this.currentShapeIndex == 0;
    }
    public bool IsSquare()
    {
        return this.currentShapeIndex == 1;
    }
    public bool IsRed()
    {
        return this.currentColorIndex == 0;
    }
    public bool IsGreen()
    {
        return this.currentColorIndex == 1;
    }
}
