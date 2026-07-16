using UnityEngine;

namespace MeowTruck
{
	public static class Constants
	{
		/** Network Params **/
		public static readonly int MAX_PLAYERS = 4;

		/** Layers **/
		public static readonly LayerMask LAYER_PLAYER = 1 << 10;

		/** Scene Names **/
		public static readonly string SCENE_INIT = "0_InitScene";
		public static readonly string SCENE_MAIN = "1_MainScene";
		public static readonly string SCENE_LOBBY = "2_LobbyScene";
		public static readonly string SCENE_GAME = "3_GameScene";

		public static readonly string SCENE_TRANSITION = "_TransitionScene";

		/** Key-Value **/
		public static readonly string KEY_GAMENAME = "GameName";
		public static readonly string VALUE_GAMENAME = "MeowTruck";
	}
}
