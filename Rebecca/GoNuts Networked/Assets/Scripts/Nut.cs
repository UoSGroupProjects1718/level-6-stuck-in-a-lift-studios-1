using Player.SyncedData;
using UnityEngine;

public class Nut : MonoBehaviour {

	private int uniqueID = -1;
	private int ownerID = -1;

	void OnCollisionEnter(Collision col){
		if (col.gameObject.tag == "player"){
			if (ownerID != -1){
				
			} else {
				col.gameObject.GetComponent<PlayerDataForClients>().SetHasNutFlag(true);
				this.ownerID = col.gameObject.GetComponent<PlayerDataForClients>().GetTeam();
				this.gameObject.SetActive(false);
			}
		}
	}

	public void SetUniqueID (int id){
		uniqueID = id;
	}

	public int GetUniqueID (){
		return uniqueID;
	}

	public int GetOwnerID (){
		return ownerID;
	}
}
