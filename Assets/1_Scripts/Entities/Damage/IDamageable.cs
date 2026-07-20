namespace MeowTruck.Entities
{
	/// <summary>
	/// 
	/// Damage를 받을 수 있는 함수들을 정의합니다.
	/// 
	/// </summary>
	public interface IDamageable
	{
		public void TakeDamage(float damage);
	}
}
