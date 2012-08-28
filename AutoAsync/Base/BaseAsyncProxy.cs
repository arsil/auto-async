using System;

using AutoAsync.Helpers;

namespace AutoAsync.Base
{
	/// <summary>
	/// Helps derived classes by implementing some <c>IAsyncProxy</c> functionalities
	/// </summary>
	/// <typeparam name="TInterface">Synchronous interface that we want to use asynchronously</typeparam>
	public abstract class BaseAsyncProxy<TInterface> : IAsyncProxy<TInterface> where TInterface : class
	{
		/// <summary>
		/// Begins asynchronous call to the <paramref name="callAction"/> method which does not return value
		/// </summary>
		/// <param name="callAction">Method to call</param>
		/// <param name="callback">Optional callback method</param>
		/// <returns>Representation of the asynchronous call without return value. This object must be passed to the appropriate </returns>
		public IExtendedAsyncResult BeginCall(Action<TInterface> callAction, Action<IExtendedAsyncResult> callback)
		{
			return BeginCall(callAction, callback, null);
		}

		/// <summary>
		/// Begins asynchronous call to the <paramref name="callAction"/> method which does not return value
		/// </summary>
		/// <param name="callAction">Method to call</param>
		/// <param name="callback">Optional callback method</param>
		/// <param name="userState">Optional user state</param>
		/// <returns>Representation of the asynchronous call without return value. This object must be passed to the appropriate </returns>
		public IExtendedAsyncResult BeginCall(
			Action<TInterface> callAction, Action<IExtendedAsyncResult> callback, object userState)
		{
			var mip = MakeCallAndGetParams(() => callAction(Proxy));
			return ProcessBeginCall(mip, callback, userState);
		}

		/// <summary>
		/// Begins asynchronous call to the <paramref name="callAction"/> method which returns <typeparamref name="TRes"/> value
		/// </summary>
		/// <param name="callAction">Method to call</param>
		/// <param name="callback">Optional callback method</param>
		/// <param name="userState">Optional user state</param>
		/// <returns>Representation of the asynchronous call with <typeparamref name="TRes"/> return value</returns>
		/// <typeparam name="TRes">Return value type</typeparam>
		public IExtendedAsyncResult<TRes> BeginCall<TRes>(
			Func<TInterface, TRes> callAction, Action<IExtendedAsyncResult<TRes>> callback, object userState)
		{
			var mip = MakeCallAndGetParams(() => callAction(Proxy));
			return ProcessBeginCall(mip, callback, userState);
		}

		/// <summary>
		/// Begins asynchronous call to the <paramref name="callAction"/> method which returns <typeparamref name="TRes"/> value
		/// </summary>
		/// <param name="callAction">Method to call</param>
		/// <param name="callback">Optional callback method</param>
		/// <returns>Representation of the asynchronous call with <typeparamref name="TRes"/> return value</returns>
		/// <typeparam name="TRes">Return value type</typeparam>
		public IExtendedAsyncResult<TRes> BeginCall<TRes>(Func<TInterface, TRes> callAction, Action<IExtendedAsyncResult<TRes>> callback)
		{
			return BeginCall(callAction, callback, null);
		}


		/// <summary>
		/// Ends a pending asynchronous call.
		/// </summary>
		/// <param name="result"><see cref="IExtendedAsyncResult"/> returned by a call
		/// to the <c>BeginCall</c> method</param>
		public void EndCall(IExtendedAsyncResult result)
		{
			result.End();
		}

		/// <summary>
		/// Ends a pending asynchronous call.
		/// </summary>
		/// <typeparam name="TOut1">Type of the result value</typeparam>
		/// <param name="out1">Returned TOut1 value</param>
		/// <param name="result"><see cref="IExtendedAsyncResult"/> returned by a call to the <c>BeginCall</c> method</param>
		public void EndCall<TOut1>(out TOut1 out1, IExtendedAsyncResult result)
		{
			result.End(out out1);
		}

		/// <summary>
		/// Ends a pending asynchronous call
		/// </summary>
		/// <typeparam name="TOut1">Type of the 1st result value</typeparam>
		/// <typeparam name="TOut2">Type of the 2nd result value</typeparam>
		/// <param name="out1">Returned 1st value</param>
		/// <param name="out2">Returned 2nd value</param>
		/// <param name="result"><see cref="IExtendedAsyncResult"/> returned by a call to the <c>BeginCall</c> method</param>
		public void EndCall<TOut1, TOut2>(out TOut1 out1, out TOut2 out2, IExtendedAsyncResult result)
		{
			result.End(out out1, out out2);
		}

		/// <summary>
		/// Ends a pending asynchronous call
		/// </summary>
		/// <typeparam name="TRes">Type of the return value</typeparam>
		/// <param name="result"><see cref="IExtendedAsyncResult"/> returned by a call to the <c>BeginCall</c> method</param>
		/// <returns>Return value</returns>
		public TRes EndCall<TRes>(IExtendedAsyncResult<TRes> result)
		{
			return result.End();
		}

		/// <summary>
		/// Ends a pending asynchronous call
		/// </summary>
		/// <typeparam name="TRes">Type of the return value</typeparam>
		/// <typeparam name="TOut1">Type of the 1st additional result value</typeparam>
		/// <param name="out1">Returned 1st additional value</param>
		/// <param name="result"><see cref="IExtendedAsyncResult"/> returned by a call to the <c>BeginCall</c> method</param>
		/// <returns>Return value</returns>
		public TRes EndCall<TRes, TOut1>(out TOut1 out1, IExtendedAsyncResult<TRes> result)
		{
			return result.End(out out1);
		}

		/// <summary>
		/// Ends a pending asynchronous call
		/// </summary>
		/// <typeparam name="TRes">Type of the return value</typeparam>
		/// <typeparam name="TOut1">Type of the 1st additional result value</typeparam>
		/// <typeparam name="TOut2">Type of the 2nd additional result value</typeparam>
		/// <param name="out1">Returned 1st additional value</param>
		/// <param name="out2">Returned 2nd additional value</param>
		/// <param name="result"><see cref="IExtendedAsyncResult"/> returned by a call to the <c>BeginCall</c> method</param>
		/// <returns>Return value</returns>
		public TRes EndCall<TRes, TOut1, TOut2>(out TOut1 out1, out TOut2 out2, IExtendedAsyncResult<TRes> result)
		{
			return result.End(out out1, out out2);
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public virtual void Dispose()
		{
		}

		/// <summary>
		/// Derived classes must implement this method in order to process begin call request
		/// </summary>
		/// <param name="mip">Method invocation parameters</param>
		/// <param name="callBack">Callback method</param>
		/// <param name="userState">User state</param>
		/// <returns>Representation of the asynchronous call without return value. This object must be passed to the appropriate </returns>
		protected abstract IExtendedAsyncResult ProcessBeginCall(
			MethodInvocationParams mip, Action<IExtendedAsyncResult> callBack, object userState);

		/// <summary>
		/// Derived classes must implement this method in order to process begin call request
		/// </summary>
		/// <param name="mip">Method invocation parameters</param>
		/// <param name="callBack">Callback method</param>
		/// <param name="userState">User state</param>
		/// <returns>Representation of the asynchronous call without return value. This object must be passed to the appropriate </returns>
		protected abstract IExtendedAsyncResult<TRes> ProcessBeginCall<TRes>(
			MethodInvocationParams mip, Action<IExtendedAsyncResult<TRes>> callBack, object userState);

		#region Static hacks

		/*
		These hacks save some memory at cost of speed (lock).
		Only parameters deduction is in critical section - so this should not be a problem
		*/

		private static MethodInvocationParams MakeCallAndGetParams(Action action)
		{
			MethodInvocationParams result;

			lock (SyncRoot)
			{
				action();
				result = InvocationHelper.MethodInvocationParams;
			}

			return result;
		}

		static class InvocationHelper
		{
			public static void SetMethodInvocationParams(MethodInvocationParams mip)
			{
				MethodInvocationParams = mip;
			}

			public static MethodInvocationParams MethodInvocationParams;
		}

		private readonly static TInterface Proxy = ProxyGeneratorHolder.Generator
			.CreateInterfaceProxyWithoutTarget<TInterface>(
				new MethodInvocationParamsInterceptor(InvocationHelper.SetMethodInvocationParams));

		private static readonly object SyncRoot = new object();

		#endregion
	}
}
