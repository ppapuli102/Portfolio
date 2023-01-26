using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierArrows : MonoBehaviour
{
    #region Public Fields
    [Tooltip("Arrow Head Prefab")]
    public GameObject ArrowHeadPrefab;
    [Tooltip("Arrow Node Prefab")]
    public GameObject ArrowNodePrefab;
    [Tooltip("Number of Arrow Bodies")]
    public int arrowNodeNum;
    [Tooltip("Scale Multiplier for Arrow Nodes")]
    public float scaleFactor = 1f;
    #endregion

    #region Private Fields
    /// <summary>
    /// The position of P0 (the arrows emitter point)
    /// </summary>
    private RectTransform origin;

    /// <summary>
    /// The list of arrow nodes' transform
    /// </summary>
    private List<RectTransform> arrowNodes = new List<RectTransform>();

    /// <summary>
    /// The list of control points
    /// </summary>
    private List<Vector2> controlPoints = new List<Vector2>();

    /// <summary>
    /// The factors to determine the position of contorl points P1, P2
    /// </summary>
    private readonly List<Vector2> controlPointFactors = new List<Vector2> {new Vector2(-0.3f, 0.8f), new Vector2(0.1f, 1.4f)};
    #endregion

    [SerializeField] private Vector2 arrowHeadOffset = new Vector2(-45, -7); 
    public bool isActive = false;

    // On Awake, create all of the arrow nodes / head gameobjects and set them Inactive
    private void Awake() {
        // Instantiate the arrow nodes and arrow head
        for (int i = 0; i < this.arrowNodeNum; i ++) {
            this.arrowNodes.Add(Instantiate(this.ArrowNodePrefab, this.transform).GetComponent<RectTransform>());
        }

        this.arrowNodes.Add(Instantiate(this.ArrowHeadPrefab, this.transform).GetComponent<RectTransform>());

        for (int i = 0; i < this.arrowNodeNum + 1; i ++) {
            this.arrowNodes[i].gameObject.SetActive(false);
        }
    }


    public void BeginTargeting() {
        isActive = true;
        // Gets position of the arrows emitter point
        this.origin = this.GetComponent<RectTransform>();

        for (int i = 0; i < this.arrowNodeNum + 1; i ++) {
            this.arrowNodes[i].gameObject.SetActive(true);
        }

        // Hides the arrow nodes
        //this.arrowNodes.ForEach(a => a.GetComponent<RectTransform>().position = new Vector2(-1000, -1000));

        // Initializes the contorl points list
        for (int i = 0; i < 4; i ++) {
            this.controlPoints.Add(Vector2.zero);
        }
    }

    public void EndTargeting() {
        isActive = false;
        // Destroy all Arrow GameObjects
        for (int i = 0; i < this.arrowNodeNum + 1; i ++) {
            this.arrowNodes[i].gameObject.SetActive(false);
        }
    }

    private void Update() {
        if (isActive) {
            // P0 is at the arrow emitter point
            this.controlPoints[0] = new Vector2(this.origin.position.x, this.origin.position.y);

            // P3 is at the mouse position
            this.controlPoints[3] = new Vector2(Input.mousePosition.x + arrowHeadOffset.x, Input.mousePosition.y + arrowHeadOffset.y);

            // P1, P2 are determined by P0 and P3
            // P1 = P0 + (P3 - P0) * Vector2(0.3f, 0.8f, 0)
            // P2 = P0 + (P3 - P0) * Vector2(0.1f, 1.4f, 0)
            this.controlPoints[1] = this.controlPoints[0] + (this.controlPoints[3] - this.controlPoints[0]) * this.controlPointFactors[0];
            this.controlPoints[2] = this.controlPoints[0] + (this.controlPoints[3] - this.controlPoints[0]) * this.controlPointFactors[1];

            for (int i = 0; i < this.arrowNodes.Count; i ++) {
                // Calculate t
                var t = Mathf.Log(1f * i / (this.arrowNodes.Count - 1) + 1f, 2f);

                // Cubic Bezier curve
                // B(t) = (1-t)^3 * P0 + 3 * (1-t)^2 * t * P1 + 3 * (1-t) * t^2 * P2 + t^3 * P3
                this.arrowNodes[i].position =
                    Mathf.Pow(1 - t, 3) * this.controlPoints[0] +
                    3 * Mathf.Pow(1 - t, 2) * t * this.controlPoints[1] +
                    3 * (1 - t) * Mathf.Pow(t, 2) * this.controlPoints[2] +
                    Mathf.Pow(t, 3) * this.controlPoints[3];

                // Calculates rotations for each arrow node
                if (i > 0) {
                    var euler = new Vector3(0,0, Vector2.SignedAngle(Vector2.up, this.arrowNodes[i].position - this.arrowNodes[i-1].position));
                    this.arrowNodes[i].rotation = Quaternion.Euler(euler);
                }

                // Calculates scales for each arrow node
                var scale = this.scaleFactor * (1f - 0.03f * (this.arrowNodes.Count - 1 - i));
                this.arrowNodes[i].localScale = new Vector3(scale, scale, 1f);
            }

            // The first arrow node's rotation
            this.arrowNodes[0].transform.rotation = this.arrowNodes[1].transform.rotation;
        }
            }
        


}
