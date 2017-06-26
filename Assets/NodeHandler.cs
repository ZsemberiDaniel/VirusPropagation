using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(LineRenderer))]
public class NodeHandler : MonoBehaviour {

    [SerializeField]
    private Color normalColor;
    [SerializeField]
    private Color selectedColor;
    [SerializeField]
    private Color connectStartColor;

    private bool selected = false;
    public bool Selected {
        set { selected = value; }
        get { return selected; }
    }

    private bool selectedStart = false;
    public bool SelectedStart {
        set { selectedStart = value; }
        get { return selectedStart; }
    }

    private SpriteRenderer spriteRenderer;
    private LineRenderer lineRenderer;
    
	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        lineRenderer = GetComponent<LineRenderer>();

        name = RandomWords.getRandomWords(2);
    }
	
	void Update () {
        if (SelectedStart) spriteRenderer.color = connectStartColor;
        else if (Selected) spriteRenderer.color = selectedColor;
        else spriteRenderer.color = normalColor;
	}

    public bool Contains(Vector2 pos) {
        return spriteRenderer.bounds.Contains(pos);
    }
}
