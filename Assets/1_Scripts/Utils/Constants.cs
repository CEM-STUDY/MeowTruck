using UnityEngine;

namespace MeowTruck
{
	public static class Constants
	{
		/** Network Params **/
		public static readonly int MAX_PLAYERS = 4;

		/** Layers **/
		public static readonly LayerMask LAYER_PLAYER = 1 << 10;
		public static readonly LayerMask LAYER_INTERACT = 1 << 11;
		public static readonly LayerMask LAYER_ITEM = 1 << 12;

		/** Scene Names **/
		public static readonly string SCENE_INIT = "0_InitScene";
		public static readonly string SCENE_MAIN = "1_MainScene";
		public static readonly string SCENE_LOBBY = "2_LobbyScene";
		public static readonly string SCENE_VILLAGE = "3_VillageScene";
		public static readonly string SCENE_FOOD_TRUCK = "4_FoodTruckScene";
		public static readonly string SCENE_FIELD = "5_FieldScene";

		public static readonly string SCENE_TRANSITION = "_TransitionScene";

		/** Key-Value **/
		public static readonly string KEY_GAMENAME = "GameName";
		public static readonly string VALUE_GAMENAME = "MeowTruck";
	}
}
