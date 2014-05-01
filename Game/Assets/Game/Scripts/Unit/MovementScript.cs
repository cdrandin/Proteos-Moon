using UnityEngine;
using System.Collections;

public class MovementScript : Photon.MonoBehaviour {
	public int speed = 10;
	public int gravity = 5;
	private CharacterController cc;
	//private GameObject other_player;
	private PhotonView my_photon_view;
	// Use this for initialization
	void Start () {
		cc = GetComponent<CharacterController>();
		my_photon_view = PhotonView.Get(this);
	}

	void OnPhotonSerializeView(PhotonStream stream,
	                           PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			//We own this player: send the others our data
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
		}
		else
		{
			//Network player, receive data
			//other_player.position = (Vector3)stream.ReceiveNext();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (my_photon_view.isMine){
			cc.Move(new Vector3(Input.GetAxis("Horizontal") * speed * Time.deltaTime, -gravity * Time.deltaTime, Input.GetAxis("Vertical") * speed * Time.deltaTime));
		}
		else{
			enabled = true;
		}
	}
}