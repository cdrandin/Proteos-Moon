using UnityEngine;
using System.Collections;

public class ThirdPersonNetwork : Photon.MonoBehaviour
{
    private TextMesh tMesh;
    void Awake()
    {
        Debug.Log("SPAWN PLAYER");
        tMesh = gameObject.GetComponentInChildren<TextMesh>();
    }
    void Start ()   
    {		    	
    	transform.SendMessage ("IsLocalPlayer", photonView.isMine, SendMessageOptions.DontRequireReceiver);
        
		if (photonView.isMine)
        {
            tMesh.gameObject.active = false;
            Camera.main.SendMessage("SetLocalPlayerTransform", transform, SendMessageOptions.DontRequireReceiver);
		
        }
        else
        {
            tMesh.text = photonView.owner.name;
        }


    }


    void Update()
    {
        if (!photonView.isMine)
        {
            Vector3 pos = tMesh.transform.position - new Vector3(-10, 5, 10);// Camera.main.transform.position;
           // pos = tMesh.transform.position + pos;
            tMesh.transform.LookAt(pos);
        }
    }

    void OnPhotonInstantiate(PhotonMessageInfo info)
    {       
        GameManager.AddPlayer(transform);
    }
    void OnDestroy()
    {
      
        GameManager.RemovePlayer(transform);
    }
   

}