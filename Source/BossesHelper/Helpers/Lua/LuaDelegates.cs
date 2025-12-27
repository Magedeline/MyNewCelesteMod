using NLua;

namespace DesoloZantas.Core.BossesHelper.Helpers.Lua
{
	internal static class LuaDelegates
	{
		#region To Celeste Delegates
		public static Collision ToCollision(this LuaFunction f)
			=> data => f.Call(data);

		public static Ease.Easer ToEaser(this LuaFunction f)
			=> t => (float)f.Call(t)[0];

		public static DashCollision ToDashCollision(this LuaFunction f)
			=> (player, direction) => (DashCollisionResults)f.Call(player, direction)[0];
		#endregion

		#region To Action
		public static Action ToAction(this LuaFunction function) =>
			() => function.Call();

		public static Action<T> ToAction<T>(this LuaFunction function) =>
			(t) => function.Call(t);

		public static Action<T1, T2> ToAction<T1, T2>(this LuaFunction function) =>
			(t1, t2) => function.Call(t1, t2);

		public static Action<T1, T2, T3> ToAction<T1, T2, T3>(this LuaFunction function) =>
			(t1, t2, t3) => function.Call(t1, t2, t3);

		public static Action<T1, T2, T3, T4> ToAction<T1, T2, T3, T4>(this LuaFunction function) =>
			(t1, t2, t3, t4) => function.Call(t1, t2, t3, t4);

		public static Action<T1, T2, T3, T4, T5> ToAction<T1, T2, T3, T4, T5>(this LuaFunction function) =>
			(t1, t2, t3, t4, t5) => function.Call(t1, t2, t3, t4, t5);

		public static Action<T1, T2, T3, T4, T5, T6> ToAction<T1, T2, T3, T4, T5, T6>(this LuaFunction function) =>
			(t1, t2, t3, t4, t5, t6) => function.Call(t1, t2, t3, t4, t5, t6);
		#endregion

		#region To Func
		public static Func<TOut> ToFunc<TOut>(this LuaFunction function) =>
			() => (TOut)function.Call().FirstOrDefault();

		public static Func<T, TOut> ToFunc<T, TOut>(this LuaFunction function) =>
			(t) => (TOut)function.Call(t).FirstOrDefault();

		public static Func<T1, T2, TOut> ToFunc<T1, T2, TOut>(this LuaFunction function) =>
			(t1, t2) => (TOut)function.Call(t1, t2).FirstOrDefault();

		public static Func<T1, T2, T3, TOut> ToFunc<T1, T2, T3, TOut>(this LuaFunction function) =>
			(t1, t2, t3) => (TOut)function.Call(t1, t2, t3).FirstOrDefault();

		public static Func<T1, T2, T3, T4, TOut> ToFunc<T1, T2, T3, T4, TOut>(this LuaFunction function) =>
			(t1, t2, t3, t4) => (TOut)function.Call(t1, t2, t3, t4).FirstOrDefault();

		public static Func<T1, T2, T3, T4, T5, TOut> ToFunc<T1, T2, T3, T4, T5, TOut>(this LuaFunction function) =>
			(t1, t2, t3, t4, t5) => (TOut)function.Call(t1, t2, t3, t4, t5).FirstOrDefault();

		public static Func<T1, T2, T3, T4, T5, T6, TOut> ToFunc<T1, T2, T3, T4, T5, T6, TOut>(this LuaFunction function) =>
			(t1, t2, t3, t4, t5, t6) => (TOut)function.Call(t1, t2, t3, t4, t5, t6).FirstOrDefault();
		#endregion
	}
}




