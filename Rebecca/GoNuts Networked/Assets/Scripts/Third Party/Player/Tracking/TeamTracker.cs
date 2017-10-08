using Player.SyncedData;
using UnityEngine;

namespace Player.Tracking{
	public class TeamTracker {

		private static TeamTracker instance;

		public delegate void TeamChanged(int numA, int numB, int numC, int numD, int numE);
		public event TeamChanged OnTeamChanged;

		private int a = 0;
		private int b = 0;
		private int c = 0;
		private int d = 0;
		private int e = 0;

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
			a = b = c = d = e = 0;
			foreach (GameObject player in PlayerTracker.GetInstance().GetPlayers()){
				switch (player.GetComponent<PlayerDataForClients>().GetTeam()){
					case PlayerDataForClients.PLAYER_A:
						a++;
						break;
					case PlayerDataForClients.PLAYER_B:
						b++;
						break;
					case PlayerDataForClients.PLAYER_C:
						c++;
						break;
					case PlayerDataForClients.PLAYER_D:
						d++;
						break;
					case PlayerDataForClients.PLAYER_E:
						e++;
						break;
				}
			}
			if (OnTeamChanged != null){
				OnTeamChanged (a, b, c, d, e);
			}
		}

		public int[] GetTeams(){
			return new int[] { a, b, c, d, e };
		}
	}
}
