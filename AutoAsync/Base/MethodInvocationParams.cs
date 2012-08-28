using System;
using System.Reflection;

namespace AutoAsync.Base
{
	/// <summary>
	/// Defines parameters of the method invocation
	/// </summary>
	public class MethodInvocationParams
	{
		/// <summary>
		/// Initializes a new instance of the MethodInvocationParams class
		/// </summary>
		/// <param name="method"><c>MethodInfo</c> of the call</param>
		/// <param name="arguments">Arguments of the call</param>
		/// <param name="genericArguments">Generic arguments of the call</param>
		public MethodInvocationParams(
			MethodInfo method, object[] arguments, Type[] genericArguments)
		{
			Method = method;
			Arguments = arguments;
			GenericArguments = genericArguments;
		}

		/// <summary>
		/// <c>MethodInfo</c> of the call
		/// </summary>
		public MethodInfo Method { get; private set; }

		/// <summary>
		/// Arguments of the call
		/// </summary>
		public object[] Arguments { get; private set; }

		/// <summary>
		/// Generic arguments of the call
		/// </summary>
		public Type[] GenericArguments { get; private set; }
	}
}
