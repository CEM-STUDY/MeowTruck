namespace MeowTruck.Manager
{
	public class InputManager
	{
		public PlayerControl Control { get; private set; }

		public void Init()
		{
			Control = new();
			Control.Enable();
		}
	}
}
