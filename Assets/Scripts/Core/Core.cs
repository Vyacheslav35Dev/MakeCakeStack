using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Core : MonoBehaviour
{
	public Text scoreText;
	public Color32[] gameColors = new Color32[4];
	public Material stackMat;
	public GameObject endPanel;

	public GameObject MainCamera;
	
	public List<GameObject> ListObjStack = new List<GameObject>();

	private const float BOUNDS_SIZE = 3.5F;
	private const float STACK_MOCING_SPEED = 5.0F;
	private const float ERROR_MARGIN = 0.25F;
	private const float STACK_BOUNDS_GAIN = 0.25F;
	private const int   COMBO_STACK_GAIN = 2;

	//private GameObject[] Stack;
	private Vector2 stackBounds = new Vector2 (BOUNDS_SIZE, BOUNDS_SIZE);

	private int scoreCount = 0;
	private int stackIndex;
	private int combo = 0;

	private float tileTransition = .0f;
	private float tileSpeed = 2.5f;
	private float secondaryPosition;

	private bool isMovingOnX = true;
	private bool gameOver = false;

	private Vector3 desirePosition;
	private Vector3 lastTilePosition;

	// Use this for initialization
	void Start ()
	{
		for (int i = 0; i < ListObjStack.Count; i++)
		{
			GameObject obj = Instantiate(ListObjStack[i], this.transform);
			ListObjStack[i] = obj;
		}

		int counterY = 0;
		foreach (var obj in ListObjStack)
		{
			counterY++;
			float height = obj.transform.localScale.y * 2;
			obj.transform.localPosition = new Vector3(0, -(counterY * height), 0);
		}
		stackIndex = ListObjStack.Count - 1;
	}
	
	// Update is called once per frame
    private void Update () 
    {
		if (Input.GetMouseButtonDown (0)) {
			if (PlaceTile ()) {
				SpawnTile ();
				scoreCount++;
				scoreText.text = scoreCount.ToString();
			} else {
				EndGame ();
			}
		}
		
		MoveTile ();
		
		MainCamera.transform.position = Vector3.Lerp (MainCamera.transform.position, new Vector3(MainCamera.transform.position.x, 20 + desirePosition.y * -1, MainCamera.transform.position.z), STACK_MOCING_SPEED);
	}

	private void MoveTile()
	{
		Transform t = ListObjStack [stackIndex].transform;
		
		t.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);
		
		if (gameOver) {
			return;
		}

		tileTransition += Time.deltaTime * tileSpeed;
		
		if(isMovingOnX)
			ListObjStack [stackIndex].transform.localPosition = new Vector3 (Mathf.Sin (tileTransition)* BOUNDS_SIZE, scoreCount, secondaryPosition);
		else
			ListObjStack [stackIndex].transform.localPosition = new Vector3 (secondaryPosition, scoreCount, Mathf.Sin (tileTransition)* BOUNDS_SIZE);
	}

	private void SpawnTile()
	{
		Transform t = ListObjStack [stackIndex].transform;
		
		lastTilePosition = ListObjStack [stackIndex].transform.localPosition;
		
		stackIndex--;
		
		if (stackIndex < 0)
		{
			stackIndex = transform.childCount - 1;
		}
		
		desirePosition = Vector3.down * scoreCount;
		
		t.localPosition = new Vector3 (0, scoreCount, 0);
		
		t.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);
	}
	
	private void CreateRubble(Vector3 pos,Vector3 scale)
	{
		GameObject go = GameObject.CreatePrimitive (PrimitiveType.Cube);
		go.transform.localPosition = new Vector3(pos.x, pos.y*2, pos.z);
		go.transform.localScale = scale;
		go.AddComponent<Rigidbody> ();
		go.GetComponent<MeshRenderer> ().material = stackMat; 
	}


	private bool PlaceTile()
	{
		Transform t = ListObjStack [stackIndex].transform;

		if (isMovingOnX) 
		{
			float deltaX = lastTilePosition.x - t.position.x;
			
			if (Mathf.Abs (deltaX) > ERROR_MARGIN)
			{
				
				combo = 0;
				stackBounds.x -= Mathf.Abs (deltaX);
				
				if (stackBounds.x <= 0)
					return false;
				
				float middle = lastTilePosition.x + t.localPosition.x / 2; 
				t.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);
				CreateRubble (
					new Vector3 ((t.position.x>0)
						? t.position.x + (t.localScale.x / 2)
						: t.position.x - (t.localScale.x/2)
						, t.position.y
						, t.position.z),
					new Vector3 (Mathf.Abs (deltaX), 1, t.localScale.z)
				);
				t.localPosition = new Vector3 (middle - (lastTilePosition.x / 2), scoreCount, lastTilePosition.z);

			} else {
				if (combo > COMBO_STACK_GAIN) {
					if (stackBounds.x > BOUNDS_SIZE)
						stackBounds.x = BOUNDS_SIZE;
					
					//stackBounds.x += STACK_BOUNDS_GAIN;
					float middle = lastTilePosition.x + t.localPosition.x / 2; 
					t.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);
					CreateRubble (
						new Vector3 ((t.position.x>0)
							? t.position.x + (t.localScale.x / 2)
							: t.position.x - (t.localScale.x/2)
							, t.position.y
							, t.position.z),
						new Vector3 (Mathf.Abs (deltaX), 1, t.localScale.z)
					);
					t.localPosition = new Vector3 (middle - (lastTilePosition.x / 2), scoreCount, lastTilePosition.z);

				}
				combo++;
				t.localPosition = new Vector3 (lastTilePosition.x, scoreCount, lastTilePosition.z);
			}
		}
		 else {
			float deltaZ = lastTilePosition.z - t.position.z;
			if (Mathf.Abs (deltaZ) > ERROR_MARGIN) {
				//cut the tile
				combo = 0;
				stackBounds.y -= Mathf.Abs (deltaZ);
				if (stackBounds.y <= 0) 
					return false;
				
				float middle = lastTilePosition.z + t.localPosition.z / 2; 
				t.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);
				CreateRubble (
					new Vector3 (
						t.position.x
						, t.position.y
						, (t.position.z>0)
						? t.position.z+ (t.localScale.z/ 2)
						: t.position.z - (t.localScale.z/2)),
					new Vector3 (t.localScale.x, 1, Mathf.Abs (deltaZ))
				);
				t.localPosition = new Vector3 (lastTilePosition.x, scoreCount,middle - (lastTilePosition.z / 2));

			}else {
				if (combo > COMBO_STACK_GAIN) {
					if (stackBounds.y > BOUNDS_SIZE)
						stackBounds.y = BOUNDS_SIZE;
					
					//stackBounds.y += STACK_BOUNDS_GAIN;
					float middle = lastTilePosition.z + t.localPosition.z / 2; 
					t.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);
					CreateRubble (
						new Vector3 (
							t.position.x
							, t.position.y
							, (t.position.z>0)
							? t.position.z+ (t.localScale.z/ 2)
							: t.position.z - (t.localScale.z/2)),
						new Vector3 (t.localScale.x, 1, Mathf.Abs (deltaZ))
					);
					t.localPosition = new Vector3 (lastTilePosition.x, scoreCount,middle - (lastTilePosition.z / 2));

				}
				combo++;
				t.localPosition = new Vector3 (lastTilePosition.x, scoreCount, lastTilePosition.z);
			}
		}

		Debug.Log (combo);

		secondaryPosition = (isMovingOnX)
			? t.localPosition.x
			: t.localPosition.z;
		
		isMovingOnX = !isMovingOnX;
		return true;
	}

    //渐变颜色
	private Color32 Lerp4(Color32 a, Color32 b, Color32 c,Color32 d,float t){
		if (t < 0.33f) {
			return Color.Lerp (a, b, t / 0.33f);
		} else if (t < 0.66f) {
			return Color.Lerp (b, c, (t - 0.33f) / 0.33f);
		} else {
			return Color.Lerp (c, d, (t - 0.66f) / 0.66f);
		}
	}

	private void EndGame(){

		if (PlayerPrefs.GetInt ("score") < scoreCount) {
			PlayerPrefs.SetInt ("score", scoreCount);
		
		}
		Debug.Log ("lose");
		gameOver = true;
		endPanel.SetActive(true);
		ListObjStack [stackIndex].AddComponent<Rigidbody> ();
	}

	public void OnButtonClick(string sceneName){
		//SceneManager.LoadScene (sceneName);
	}

	public void toMenu(){
		//SceneManager.LoadScene ("Menu");
	}

	public void Retry(){
		//SceneManager.LoadScene ("Stack");
	}
}
