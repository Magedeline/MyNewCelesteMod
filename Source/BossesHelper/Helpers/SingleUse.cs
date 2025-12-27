namespace DesoloZantas.Core.BossesHelper.Helpers
{
	public struct SingleUse<T>(T val) where T : struct
	{
		private T? value = val;

		public readonly bool HasValue => value.HasValue;

		public T? Value
		{
			get
			{
				T? val = value;
				value = null;
				return val;
			}
		}

		public static implicit operator SingleUse<T>(T? v) => v.HasValue ? new(v.Value) : new();

		public static implicit operator T?(SingleUse<T> s) => s.Value;
	}
}




