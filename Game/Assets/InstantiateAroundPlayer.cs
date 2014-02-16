using UnityEngine;
using System.Collections;

public class InstantiateAroundPlayer : MonoBehaviour {
	
	//number of elements to be initiated
	public int NoOfElements = 5;
	//the transform object to be initiated
	public Transform Diamond;
	
	
	//the controllable player
	public GameObject Player;
	//how far away from player
	private float DistFromPlayer = 20.0f;
	//how many elements have been created
	private int generatedElementsCount = 0;
	
	
	void Start()
	{
		//assign the Player gameobject
		Player = GameObject.Find("Player");
		
	}
	
	
	//creates elements that surround the player
	void CreateElements()
	{
		
		for(int i = 0; i < NoOfElements; i++)
		{
			//update total elements created
			generatedElementsCount++;
			//name of the instantiated object
			string objectName = "Element_" + generatedElementsCount;
			//angle of instantiated object
			float angelIteration = 360 / NoOfElements;
			//rotation of instantiated object
			float currentRotation = angelIteration * i;
			
			//create the object as a transform
			Transform elem;
			elem = Instantiate(Diamond, Player.transform.position, Player.transform.rotation) as Transform;
			
			elem.name = objectName;
			
			//update the position and rotation of the object
			elem.transform.Rotate(new Vector3(0, currentRotation, 0));
			elem.transform.Translate(new Vector3(DistFromPlayer, 5, 0));
			
		}
	}
	
	
	
	//GUI Button
	void OnGUI()
	{
		if(GUI.Button(new Rect(20, Screen.height * 0.66f, 200, 30), "Generate Elements"))
		{
			CreateElements();
		}
		
		
	}

}

