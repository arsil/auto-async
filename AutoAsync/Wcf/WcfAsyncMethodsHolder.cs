using System;
using System.Collections.Generic;
using System.Reflection;

namespace AutoAsync.Wcf
{
	class WcfAsyncMethodsHolder
	{
		public WcfAsyncMethodsHolder(Type channelType, 
			MethodInfo beginMethod, Type[] beginMethodTypes, MethodInfo endMethod, Type[] endMethodTypes)
		{
			_channelType = channelType;
			_beginMethod = beginMethod;
			_beginMethodTypes = beginMethodTypes;
			_endMethod = endMethod;
			_endMethodTypes = endMethodTypes;
		}

		public IAsyncResult BeginCall(object service, object[] parameters, AsyncCallback callBack, object userState)
		{
			// wywołanie właściwe!
			var beginMethodParameters = new List<object>(parameters);
			beginMethodParameters.Add(callBack);
			beginMethodParameters.Add(userState);

			return (IAsyncResult)_beginMethod.Invoke(service, beginMethodParameters.ToArray());
		}

		public object[] EndCall(object service, WcfExtendedAsyncResult asyncResult)
		{
			var endParameters = new object[_endMethodTypes.Length];
			endParameters[endParameters.Length - 1] = asyncResult.OrgAsyncResult;

			_endMethod.Invoke(service, endParameters);

			var result = new object[_endMethodTypes.Length - 1];
			Array.Copy(endParameters, 0, result, 0, _endMethodTypes.Length - 1);

			return result;
		}

// todo: problem z typem generycznym!!!!
		public object[] EndCall<TRes>(object service, WcfExtendedAsyncResult<TRes> asyncResult)
		{
			var endParameters = new object[_endMethodTypes.Length];
			endParameters[endParameters.Length - 1] = asyncResult.OrgAsyncResult;

			var returnResult = (TRes)_endMethod.Invoke(service, endParameters);

			var result = new object[_endMethodTypes.Length];
			result[0] = returnResult;
			Array.Copy(endParameters, 0, result, 1, _endMethodTypes.Length - 1);

			return result;
		}

		private readonly Type _channelType;
		private readonly MethodInfo _beginMethod;
		private readonly Type[] _beginMethodTypes;
		private readonly MethodInfo _endMethod;
		private readonly Type[] _endMethodTypes;
	}
}
