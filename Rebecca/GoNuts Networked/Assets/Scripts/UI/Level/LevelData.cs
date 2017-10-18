namespace UI.Level {
	public class LevelData {

		private static LevelData instance;

		public int levelTime = 0;

		private LevelData() {}

		public static LevelData GetInstance() {
			if (instance == null){
				instance = new LevelData();
			}
			return instance;
		}
	}
}
