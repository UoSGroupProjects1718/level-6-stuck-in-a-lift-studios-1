/**
 * This is based on the tutorial series found here: https://ditt.to/game-dev/multiplayer-1
 **/
using UnityEngine;
using UnityEngine.Networking;

namespace Player.SyncedData {
	public class PlayerDataForClients : NetworkBehaviour {

		public const int PLAYER_A = 1;
		public const int PLAYER_B = 2;
		public const int PLAYER_C = 3;
		public const int PLAYER_D = 4;
		public const int PLAYER_E = 5;

		public delegate void NameUpdated (GameObject player, string playerName);
		public event NameUpdated OnNameUpdated;

		public delegate void TeamUpdated (GameObject player, int team);
		public event TeamUpdated OnTeamUpdated;

		public delegate void ScoreUpdated (GameObject player, int score);
		public event ScoreUpdated OnScoreUpdated;

		public delegate void IsReadyFlagUpdated (GameObject player, bool isReady);
		public event IsReadyFlagUpdated OnIsReadyFlagUpdated;

		public delegate void IsServerFlagUpdated (GameObject player, bool isServer);
		public event IsServerFlagUpdated OnIsServerFlagUpdated;

		public delegate void HasNutFlagUpdated (GameObject player, bool hasNut);
		public event HasNutFlagUpdated OnHasNutFlagUpdated;

		public delegate void WeightUpdated (GameObject player, float weight);
		public event WeightUpdated OnWeightUpdated;

		public delegate void RankUpdated (GameObject player, int rank);
		public event RankUpdated OnRankUpdated;

		public delegate void PositionUpdated (GameObject player, float position);
		public event PositionUpdated OnPositionUpdated;

		public delegate void EagleTargetUpdated (GameObject player, int eagleTarget);
		public event EagleTargetUpdated OnEagleTargetUpdated;

		public delegate void CanMoveFlagUpdated (GameObject player, bool canMove);
		public event CanMoveFlagUpdated OnCanMoveFlagUpdated;

		public delegate void NutTimeUpdated (GameObject player, int time);
		public event NutTimeUpdated OnNutTimeUpdated;

		public delegate void TotalNutTimeUpdated (GameObject player, int time);
		public event TotalNutTimeUpdated OnTotalNutTimeUpdated;

		[SyncVar(hook = "UpdateName")]
		private string playerName;
		[SyncVar(hook = "UpdateTeam")]
		private int team;
		[SyncVar (hook = "UpdateScore")]
		private int score;
		[SyncVar (hook = "UpdateIsReadyFlag")]
		private bool isReadyFlag;
		[SyncVar(hook = "UpdateIsServerFlag")]
		private bool isServerFlag;
		[SyncVar (hook = "UpdateHasNutFlag")]
		private bool hasNutFlag;
		[SyncVar (hook = "UpdateWeight")]
		private float weight;
		[SyncVar (hook = "UpdateRank")]
		private int rank;
		[SyncVar (hook = "UpdatePosition")]
		private float position;
		[SyncVar (hook = "UpdateEagleTarget")]
		private int eagleTarget;
		[SyncVar (hook = "UpdateCanMoveFlag")]
		private bool canMoveFlag;
		[SyncVar (hook = "UpdateNutTime")]
		private int nutTime;
		[SyncVar (hook = "UpdateTotalNutTime")]
		private int totalNutTime;

		public override void OnStartClient(){
			if (!isLocalPlayer && !isServer){
				UpdateName(playerName);
				UpdateTeam(team);
				UpdateScore(score);
				UpdateIsReadyFlag(isReadyFlag);
				UpdateIsServerFlag(isServerFlag);
				UpdateHasNutFlag(hasNutFlag);
				UpdateWeight(weight);
				UpdateRank(rank);
				UpdatePosition(position);
				UpdateEagleTarget(eagleTarget);
				UpdateCanMoveFlag(canMoveFlag);
				UpdateNutTime(nutTime);
				UpdateTotalNutTime(totalNutTime);
			}
		}

		//Player name
		public string GetName(){
			return playerName;
		}
		[Client]
		public void SetName (string newName){
			CmdSetName(newName);
		}
		[Command]
		public void CmdSetName(string newName){
			playerName = newName;
		}
		[Client]
		public void UpdateName(string newName){
			playerName = newName;
			if (this.OnNameUpdated != null){
				this.OnNameUpdated(gameObject, newName);
			}
		}

		//Player team
		public int GetTeam(){
			return team;
		}
		[Client]
		public void SetTeam (int newTeam){
			CmdSetTeam(newTeam);
		}
		[Command]
		public void CmdSetTeam(int newTeam){
			team = newTeam;
		}
		[Client]
		public void UpdateTeam(int newTeam){
			team = newTeam;
			if (this.OnTeamUpdated != null){
				this.OnTeamUpdated(gameObject, newTeam);
			}
		}

		//Player score
		public int GetScore(){
			return score;
		}
		[Server]
		public void ResetScore (){
			score = 0;
		}
		[Command]
		public void CmdIncrementScore(){
			score++;
		}

		public void UpdateScore(int newScore){
			score = newScore;
			if (this.OnScoreUpdated != null){
				this.OnScoreUpdated(gameObject, newScore);
			}
		}

		//Is player ready
		public bool GetIsReadyFlag(){
			return isReadyFlag;
		}
		[Client]
		public void SetIsReadyFlag (bool newIsReady){
			CmdSetIsReadyFlag(newIsReady);
		}
		[Command]
		public void CmdSetIsReadyFlag(bool newIsReady){
			isReadyFlag = newIsReady;
		}
		[Client]
		public void UpdateIsReadyFlag(bool newIsReady){
			isReadyFlag = newIsReady;
			if (this.OnIsReadyFlagUpdated != null){
				this.OnIsReadyFlagUpdated(gameObject, newIsReady);
			}
		}

		//Is player also the server
		public bool GetIsServerFlag (){
			return isServerFlag;
		}
		[Client]
		public void SetIsServerFlag (bool newIsServer){
			CmdSetIsServerFlag(newIsServer);
		}
		[Command]
		public void CmdSetIsServerFlag (bool newIsServer){
			isServerFlag = newIsServer;
		}
		[Client]
		public void UpdateIsServerFlag (bool newIsServer){
			isServerFlag = newIsServer;

			if (this.OnIsServerFlagUpdated != null) {
				this.OnIsServerFlagUpdated(gameObject, newIsServer);
			}
		}

		//Has the player picked up a nut
		public bool GetHasNutFlag (){
			return hasNutFlag;
		}
		[Command]
		public void CmdSetHasNutFlag(bool newHasNut){
			hasNutFlag = newHasNut;
		}
		[Client]
		public void UpdateHasNutFlag (bool newHasNut){
			hasNutFlag = newHasNut;
			if (this.OnHasNutFlagUpdated != null){
				this.OnHasNutFlagUpdated(gameObject, newHasNut);
			}
		}

		//Player weighting (ie how close are they from winning)
		public float GetWeight(){
			return weight;
		}
		[Server]
		public void ResetWeight (){
			weight = 0;
		}
		[Command]
		public void CmdSetWeight(float newWeight){
			weight = newWeight;
		}

		public void UpdateWeight(float newWeight){
			weight = newWeight;
			if (this.OnWeightUpdated != null){
				this.OnWeightUpdated(gameObject, newWeight);
			}
		}

		//Player Rank
		public int GetRank(){
			return rank;
		}
		[Server]
		public void ResetRank (){
			rank = 0;
		}
		[Server]
		public void SetRank(int newRank){
			rank = newRank;
		}

		public void UpdateRank(int newRank){
			rank = newRank;
			if (this.OnRankUpdated != null){
				this.OnRankUpdated(gameObject, newRank);
			}
		}

		//Player position on UI
		public float GetPosition(){
			return position;
		}
		[Server]
		public void ResetPosition (){
			weight = 0;
		}
		[Command]
		public void CmdSetPosition(float newPosition){
			position = newPosition;
		}

		public void UpdatePosition(float newPosition){
			position = newPosition;
			if (this.OnPositionUpdated != null){
				this.OnPositionUpdated(gameObject, newPosition);
			}
		}

		//Player's Eagle Targetting Weight
		public int GetEagleTarget(){
			return eagleTarget;
		}
		[Server]
		public void ResetEagleTarget (){
			eagleTarget = 0;
		}
		[Server]
		public void SetEagleTarget(int newEagleTarget){
			eagleTarget = newEagleTarget;
		}

		public void UpdateEagleTarget(int newEagleTarget){
			eagleTarget = newEagleTarget;
			if (this.OnEagleTargetUpdated != null){
				this.OnEagleTargetUpdated(gameObject, newEagleTarget);
			}
		}
		
		//Can the player move
		public bool GetCanMoveFlag(){
			return canMoveFlag;
		}
		[Server]
		public void SetCanMoveFlag(bool newCanMove){
			canMoveFlag = newCanMove;
		}

		public void UpdateCanMoveFlag(bool newCanMove){
			canMoveFlag = newCanMove;
			if (this.OnCanMoveFlagUpdated != null){
				this.OnCanMoveFlagUpdated(gameObject, newCanMove);
			}
		}

		//Player Nut Time
		public int GetNutTime (){
			return nutTime;
		}
		[Command]
		public void CmdSetNutTime(int newNutTime){
			nutTime = newNutTime;
		}
		[Client]
		public void UpdateNutTime (int newNutTime){
			nutTime = newNutTime;
			if (this.OnNutTimeUpdated != null){
				this.OnNutTimeUpdated(gameObject, newNutTime);
			}
		}

		//Player Total Nut Time
		public int GetTotalNutTime (){
			return totalNutTime;
		}
		[Command]
		public void CmdSetTotalNutTime(int newNutTime){
			totalNutTime = newNutTime;
		}
		[Client]
		public void UpdateTotalNutTime (int newNutTime){
			totalNutTime = newNutTime;
			if (this.OnTotalNutTimeUpdated != null){
				this.OnTotalNutTimeUpdated(gameObject, newNutTime);
			}
		}
	}
}
