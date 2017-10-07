using Player.SyncedData;
using UnityEngine;

namespace Player.Tracking{
	public class TeamTracker {

		private static TeamTracker instance;

		public delegate void TeamChanged(int numAlphas, int numBetas);
		public event TeamChanged OnTeamChanged;

		private int alphas = 0;
		private int betas = 0;

		private TeamTracker(){
			foreach (GameObject player in PlayerTracker.GetInstance().GetPlayers()){
				AddTeamTrackingToPlayer(player);
			}
			PlayerTracker.GetInstance().OnPlayerAdded += AddTeamTrackingToPlayer;
			PlayerTracker.GetInstance().OnPlayerRemoved += (GameObject player) => { CountTeams(); };
		}

		public static TeamTracker GetInstance(){
			if (instance == null){
				instance = new TeamTracker();
			}
			return instance;
		}

		public void ForceRecount(){
			CountTeams();
		}

		private void AddTeamTrackingToPlayer(GameObject player){
			player.GetComponent<PlayerDataForClients>().OnTeamUpdated += (GameObject localPlayer, int teamId) => { CountTeams(); };
		}

		private void CountTeams(){
			alphas = betas = 0;
			foreach (GameObject player in PlayerTracker.GetInstance().GetPlayers()){
				if (player.GetComponent<PlayerDataForClients>().GetTeam() == PlayerDataForClients.PLAYER_ALPHA) {
					alphas ++;
				} else {
					betas ++;
				}
			}
			if (OnTeamChanged != null){
				OnTeamChanged (alphas, betas);
			}
		}

		public int[] GetTeams(){
			return new int[] { alphas, betas };
		}
	}
}
