using System;
using System.Collections.Generic;
using System.Reflection;

namespace AutoAsync.Wcf
{
	/// <summary>
	/// Cache holding generated async interfaces
	/// </summary>
	static class WcfAsyncProxyCache
	{
		public static void GetAsyncInterface(
			Type orgInterfaceType,
			out Type asyncInterfaceType, 
			out Func<MethodInfo, WcfAsyncMethodsHolder> orgMethodToAsyncFunc)
		{
			AsyncTypeInfo result;

			lock (_syncRoot)
			{
				// checking cache....
				if (!_syncInterfaceToAsyncInfoMap.TryGetValue(orgInterfaceType, out result))
				{
					// element not found - have to generate new type!
					Type tmpType;
					var tmpMap = new Dictionary<MethodInfo, WcfAsyncMethodsHolder>();

					WcfAsyncInterfaceCreator.GenerateAsyncInterface(
						orgInterfaceType, out tmpType, tmpMap);

					result = new AsyncTypeInfo(tmpType, tmpMap);
					_syncInterfaceToAsyncInfoMap.Add(orgInterfaceType, result);
				}
			}

			asyncInterfaceType = result.AsyncInterfaceType;
			orgMethodToAsyncFunc = result.GetMethod;
		}


		private static IDictionary<Type, AsyncTypeInfo> _syncInterfaceToAsyncInfoMap
			= new Dictionary<Type, AsyncTypeInfo>();
		private readonly static object _syncRoot = new object();

		/// <summary>
		/// Helper type holding info about async interface
		/// </summary>
		class AsyncTypeInfo
		{
			public AsyncTypeInfo(
				Type asyncInterfaceType, 
				IDictionary<MethodInfo, WcfAsyncMethodsHolder> orgMethodToAsyncMap)
			{
				AsyncInterfaceType = asyncInterfaceType;
				_orgMethodToAsyncMap = orgMethodToAsyncMap;
			}

			public WcfAsyncMethodsHolder GetMethod(MethodInfo mi)
			{ return _orgMethodToAsyncMap[mi]; }

			public Type AsyncInterfaceType { get; private set; }

			private readonly IDictionary<MethodInfo, WcfAsyncMethodsHolder> _orgMethodToAsyncMap;
		}



	}
}
