using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class theStack : MonoBehaviour
{
	//public GoogleAdMobController AdsManager;
	
	public Text scoreText;
	
	public GameObject endPanel;

	public GameObject StackRoot;

	public GameObject StackRubbleRoot;
	
	public List<Material> ListStackMat = new List<Material>();

	public Keks AnimKeks;

	public AudioSource MainAudio;

	public AudioSource EndAudio;
	
	private Material stackMat;

	private const float BOUNDS_SIZE = 3.5F;
	private const float STACK_MOCING_SPEED = 5.0F;
	private const float ERROR_MARGIN = 0.25F;
	private const float STACK_BOUNDS_GAIN = 0.25F;
	private const int   COMBO_STACK_GAIN = 1;

	private List<GameObject> Stack;
	
	private List<GameObject> StackRubble = new List<GameObject>();
	
	private Vector2 stackBounds = new Vector2 (BOUNDS_SIZE, BOUNDS_SIZE);

	private int scoreCount = 0;
	private int stackIndex;
	private int combo = 0;

	private float tileTransition = .0f;
	private float tileSpeed = 2.5f;
	private float secondaryPosition;
	
	private float deltaX;
	private float deltaZ;
	private float middle;

	private bool isMovingOnX = true;
	private bool gameOver = false;

	private Vector3 desirePosition;
	private Vector3 lastTilePosition;

	private int _countStack = 10;

	// Use this for initialization
	
	void Start ()
	{
		PlayerPrefs.SetInt("counter", 0);
		//AdsManager.InitStart();
		
		InitStack();
		
		int state = PlayerPrefs.GetInt("audio");
		if (state == 1)
		{
			MainAudio.enabled = true;
			EndAudio.enabled = true;
		}
		else
		{
			MainAudio.enabled = false;
			EndAudio.enabled = false;
		}
	}
	
	private void InitStack()
	{
		Stack = new List<GameObject>();

		int count = PlayerPrefs.GetInt("counter");
		count = count + 1;
		PlayerPrefs.SetInt("counter", count);
		
		for (int i = 1; i < _countStack; i++)
		{
			var obj = Instantiate(StackRoot, this.transform);
			var material = obj.GetComponent<MeshRenderer>().material;
			
			material = ListStackMat[i];
			obj.GetComponent<MeshRenderer>().material = material;
			obj.transform.localPosition = new Vector3(0, -1 * i, 0);
			Stack.Add(obj);
		}
	
		stackIndex = Stack.Count - 1;
		
		stackMat = Stack[stackIndex].GetComponent<MeshRenderer>().material;
		
		EndAudio.Stop();
		
		MainAudio.Play();
	}
	
	private void ResetAndNewStack()
	{
		scoreCount = 0;
		tileTransition = 0;
		secondaryPosition = 0;
		desirePosition = new Vector3(0,0,0);
		lastTilePosition = new Vector3(0,0,0);
		stackBounds = new Vector2(3.5f,3.5f);
		combo = 0;
		deltaX = 0;
		deltaZ = 0;
		middle = 0;
		combo = 0;
		scoreText.text = "0";
		
		transform.localPosition = new Vector3(0,0,0);
		
		endPanel.SetActive(false);
		
		int count = transform.childCount;
		
		for (int i = 0; i < count; i++) 
		{
			Destroy(this.transform.GetChild(i).gameObject);
		}
		
		Stack.Clear();
		
		AnimKeks.Play("Walk");
		
		InitStack();
		
		gameOver = false;
	}
	
	// Update is called once per frame
    private void Update () 
    {
	    if (!gameOver)
	    {
		    if (Input.GetMouseButtonDown (0)) 
		    {
			    if (PlaceTile ()) 
			    {
				    SpawnTile ();
				    scoreCount++;
				    scoreText.text = scoreCount.ToString();
			    } 
			    else 
			    {
				    gameOver = true;
				    EndGame ();
			    }
		    }
		
		    MoveTile ();
		
		    transform.position = Vector3.Lerp (transform.position, desirePosition, STACK_MOCING_SPEED);
	    }
	    
		if (StackRubble.Count != 0)
		{
			foreach (var go in StackRubble)
			{
				float dist = Vector3.Distance(go.transform.position, StackRubbleRoot.transform.position);
				if (dist > 100)
				{
					StackRubble.Remove(go);
					Destroy(go);
					return;
				}
			}
		}
	}
    
    private void CreateRubble(Vector3 pos, Vector3 scale) // создает обрубок бруска
    {
	    GameObject obj = Instantiate(StackRoot, StackRubbleRoot.transform);
	    obj.transform.localPosition = pos;
	    obj.transform.localScale = scale;
	    obj.AddComponent<Rigidbody> ();
	    var material = obj.GetComponent<MeshRenderer>().material;
	    material = stackMat;
	    obj.GetComponent<MeshRenderer>().material = material;
	    StackRubble.Add(obj);
    }

	private void MoveTile()
	{
		Transform t = Stack [stackIndex].transform;
		
		stackMat = Stack[stackIndex].GetComponent<MeshRenderer>().material;
		
		t.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);
		
		if (gameOver) 
		{
			return;
		}

		tileTransition += Time.deltaTime * tileSpeed;
		//two way —— x or z moving
		if (isMovingOnX)
		{
			Stack[stackIndex].transform.localPosition =
				new Vector3(Mathf.Sin(tileTransition) * BOUNDS_SIZE, scoreCount, secondaryPosition);
		}
		else
		{
			Stack[stackIndex].transform.localPosition =
				new Vector3(secondaryPosition, scoreCount, Mathf.Sin(tileTransition) * BOUNDS_SIZE);
		}
	}

	private void SpawnTile()
	{
		Transform t = Stack [stackIndex].transform;
		
		lastTilePosition = Stack [stackIndex].transform.localPosition;
		
		stackIndex--;
		if (stackIndex < 0) 
		{
			stackIndex = transform.childCount - 1;
		}
		
		desirePosition = Vector3.down * scoreCount;
		t.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);
	}
	
	private bool PlaceTile()
	{
		Transform t = Stack [stackIndex].transform;

		if (isMovingOnX) 
		{
			deltaX = lastTilePosition.x - t.position.x;
			if (Mathf.Abs (deltaX) > ERROR_MARGIN) 
			{
				combo = 0;
				
				stackBounds.x -= Mathf.Abs (deltaX);
				
				if (stackBounds.x <= 0)
				{
					return false;
				}
				
				middle = lastTilePosition.x + t.localPosition.x / 2; 
				
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
				
				AnimKeks.Play("Angry");

			}
			else 
			{
				if (combo > COMBO_STACK_GAIN) 
				{
					if (stackBounds.x > BOUNDS_SIZE)
					{
						stackBounds.x = BOUNDS_SIZE;
					}
					
					float middle = lastTilePosition.x + t.localPosition.x / 2; 
					
					t.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);
					
					/*CreateRubble (
						new Vector3 ((t.position.x>0)
							? t.position.x + (t.localScale.x / 2)
							: t.position.x - (t.localScale.x/2)
							, t.position.y
							, t.position.z),
						new Vector3 (Mathf.Abs (deltaX), 1, t.localScale.z)
					);*/
					
					t.localPosition = new Vector3 (middle - (lastTilePosition.x / 2), scoreCount, lastTilePosition.z);

				}
				
				combo++;
				
				t.localPosition = new Vector3 (lastTilePosition.x, scoreCount, lastTilePosition.z);
				
				AnimKeks.Play("Smile");
			}
		}
		 else 
		{
			deltaZ = lastTilePosition.z - t.position.z;
			
			if (Mathf.Abs (deltaZ) > ERROR_MARGIN) 
			{
				combo = 0;
				
				stackBounds.y -= Mathf.Abs (deltaZ);

				if (stackBounds.y <= 0)
				{
					return false;
				}
				
				middle = lastTilePosition.z + t.localPosition.z / 2;
				
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
				
				AnimKeks.Play("Angry");

			}
			else
			{
				if (combo > COMBO_STACK_GAIN) 
				{
					if (stackBounds.y > BOUNDS_SIZE)
					{
						stackBounds.y = BOUNDS_SIZE;
					}
					
					middle = lastTilePosition.z + t.localPosition.z / 2;
					
					t.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);
					
					/*CreateRubble (
						new Vector3 (
							t.position.x
							, t.position.y
							, (t.position.z>0)
							? t.position.z+ (t.localScale.z/ 2)
							: t.position.z - (t.localScale.z/2)),
						new Vector3 (t.localScale.x, 1, Mathf.Abs (deltaZ))
					);*/
					
					t.localPosition = new Vector3 (lastTilePosition.x, scoreCount,middle - (lastTilePosition.z / 2));
				}
				
				combo++;
				
				t.localPosition = new Vector3 (lastTilePosition.x, scoreCount, lastTilePosition.z);
				
				AnimKeks.Play("Smile");
			}
		}

		Debug.Log (combo);

		secondaryPosition = (isMovingOnX)
			? t.localPosition.x
			: t.localPosition.z;
		
		isMovingOnX = !isMovingOnX;
		return true;
	}
    
	private void EndGame(){

		if (PlayerPrefs.GetInt ("score") < scoreCount) {
			PlayerPrefs.SetInt ("score", scoreCount);
		
		}
		
		endPanel.SetActive(true);
		
		EndAudio.Play();
		
		MainAudio.Stop();
		
		Stack [stackIndex].AddComponent<Rigidbody> ();
	}
	
	public void Retry()
	{
		int count = PlayerPrefs.GetInt("counter");
		
		if (count == 7)
		{
			PlayerPrefs.SetInt("counter", 0);
			//AdsManager.BeforeRequestAndLoadInterstitialAd();
		}
		ResetAndNewStack();
	}
}
