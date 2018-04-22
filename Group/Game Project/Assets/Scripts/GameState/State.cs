using System.Collections.Generic;
using UnityEngine;

namespace GameState {
	public struct SubscriberOptions {
		public StateOption option;
		public State.Subscriber subscriber;
	}

	public class State {
		public const string NETWORK_CLIENT = "network_client";
		public const string NETWORK_SERVER = "network_server";

		public const string GAME_OFFLINE = "game_offline";
		public const string GAME_CONNECTING = "game_connecting";
		public const string GAME_ONLINE = "game_online";
		public const string GAME_DISCONNECTING = "game_disconnecting";

		public const string LEVEL_IN_LOBBY = "level_in_lobby";
		public const string LEVEL_NOT_READY = "level_not_ready";
		public const string LEVEL_READY = "level_ready";
		public const string LEVEL_PLAYING = "level_playing";
		public const string LEVEL_COMPLETE = "level_complete";

		public delegate void Subscriber();

		private static State instance;

		private string previousNetworkState;
		private string networkState;
		private string previousGameState;
		private string gameState;
		private string levelState;
		private string previousLevelState;

		private int playerCount;

		private bool isNetworkDirty = false;
		private bool isGameDirty = false;
		private bool isLevelDirty = false;

		private List<SubscriberOptions> subscribers = new List<SubscriberOptions>();

		private State() {}

		public static State GetInstance(){
			if (instance == null){
				instance = new State();
			}
			return instance;
		}

		public State Network (string newNetworkState){
			if (newNetworkState != networkState){
				previousNetworkState = networkState;
				networkState = newNetworkState;
				isNetworkDirty = true;
			}
			return this;
		}

		public string Network(){
			return networkState;
		}

		public State Game (string newGameState){
			if (newGameState != gameState){
				previousGameState = gameState;
				gameState = newGameState;
				isGameDirty = true;
			}
			return this;
		}

		public string Game(){
			return gameState;
		}

		public State Level(string newLevelState){
			if (newLevelState != levelState){
				previousLevelState = levelState;
				levelState = newLevelState;
				isLevelDirty = true;
				Debug.Log("Saving Level State: " + newLevelState);
			}
			return this;
		}

		public string Level(){
			return levelState;
		}

		public void Subscribe (StateOption options, Subscriber callback) {
			SubscriberOptions subscriberOption = new SubscriberOptions();
			subscriberOption.option = options;
			subscriberOption.subscriber = callback;

			if (!subscribers.Contains(subscriberOption)){
				subscribers.Add(subscriberOption);
			}
			PublishIfMatches(subscriberOption, true);
		}

		public void Publish(){
			foreach (SubscriberOptions subscriberOption in subscribers){
				PublishIfMatches(subscriberOption);
			}
			isNetworkDirty = false;
			isGameDirty = false;
			isLevelDirty = false;
		}

		private void PublishIfMatches (SubscriberOptions subscriberOption, bool forceDirtyBit = false){
			if (subscriberOption.option.Matches(
				previousNetworkState,
				previousGameState,
				previousLevelState,
				networkState,
				gameState,
				levelState,
				forceDirtyBit ? true : isNetworkDirty,
				forceDirtyBit ? true : isGameDirty,
				forceDirtyBit ? true : isLevelDirty
			)){
				subscriberOption.subscriber();
			}
		}

		public void SetPlayerCount (int numberOfPlayers){
			this.playerCount = numberOfPlayers;
		}
		public int GetPlayerCount (){
			return this.playerCount;
		}
	}
}
