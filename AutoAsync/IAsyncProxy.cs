using System;

namespace AutoAsync
{
	/// <summary>
	/// Defines generic asynchronous call paradigm
	/// </summary>
	/// <typeparam name="TInterface">Synchronous interface that we want to use asynchronously</typeparam>
	public interface IAsyncProxy<TInterface> : IDisposable where TInterface : class
	{
		/// <summary>
		/// Begins asynchronous call to the <paramref name="callAction"/> method which does not return value
		/// </summary>
		/// <param name="callAction">Method to call</param>
		/// <param name="callback">Optional callback method</param>
		/// <returns>Representation of the asynchronous call without return value. This object must be passed to the appropriate </returns>
		IExtendedAsyncResult BeginCall(
			Action<TInterface> callAction, Action<IExtendedAsyncResult> callback);

		/// <summary>
		/// Begins asynchronous call to the <paramref name="callAction"/> method which does not return value
		/// </summary>
		/// <param name="callAction">Method to call</param>
		/// <param name="callback">Optional callback method</param>
		/// <param name="userState">Optional user state</param>
		/// <returns>Representation of the asynchronous call without return value. This object must be passed to the appropriate </returns>
		IExtendedAsyncResult BeginCall(
			Action<TInterface> callAction, Action<IExtendedAsyncResult> callback, object userState);

		/// <summary>
		/// Begins asynchronous call to the <paramref name="callAction"/> method which returns <typeparamref name="TRes"/> value
		/// </summary>
		/// <param name="callAction">Method to call</param>
		/// <param name="callback">Optional callback method</param>
		/// <param name="userState">Optional user state</param>
		/// <returns>Representation of the asynchronous call with <typeparamref name="TRes"/> return value</returns>
		/// <typeparam name="TRes">Return value type</typeparam>
		IExtendedAsyncResult<TRes> BeginCall<TRes>(
			Func<TInterface, TRes> callAction, Action<IExtendedAsyncResult<TRes>> callback, object userState);

		/// <summary>
		/// Begins asynchronous call to the <paramref name="callAction"/> method which returns <typeparamref name="TRes"/> value
		/// </summary>
		/// <param name="callAction">Method to call</param>
		/// <param name="callback">Optional callback method</param>
		/// <returns>Representation of the asynchronous call with <typeparamref name="TRes"/> return value</returns>
		/// <typeparam name="TRes">Return value type</typeparam>
		IExtendedAsyncResult<TRes> BeginCall<TRes>(
			Func<TInterface, TRes> callAction, Action<IExtendedAsyncResult<TRes>> callback);



		/// <summary>
		/// Ends a pending asynchronous call.
		/// </summary>
		/// <param name="result"><see cref="IExtendedAsyncResult"/> returned by a call to the <c>BeginCall</c> method</param>
		void EndCall(IExtendedAsyncResult result);

		/// <summary>
		/// Ends a pending asynchronous call.
		/// </summary>
		/// <typeparam name="TOut1">Type of the 1st result value</typeparam>
		/// <param name="out1">Returned 1st value</param>
		/// <param name="result"><see cref="IExtendedAsyncResult"/> returned by a call to the <c>BeginCall</c> method</param>
		void EndCall<TOut1>(out TOut1 out1, IExtendedAsyncResult result);

		/// <summary>
		/// Ends a pending asynchronous call
		/// </summary>
		/// <typeparam name="TOut1">Type of the 1st result value</typeparam>
		/// <typeparam name="TOut2">Type of the 2nd result value</typeparam>
		/// <param name="out1">Returned 1st value</param>
		/// <param name="out2">Returned 2nd value</param>
		/// <param name="result"><see cref="IExtendedAsyncResult"/> returned by a call to the <c>BeginCall</c> method</param>
		void EndCall<TOut1, TOut2>(out TOut1 out1, out TOut2 out2, IExtendedAsyncResult result);



		/// <summary>
		/// Ends a pending asynchronous call
		/// </summary>
		/// <typeparam name="TRes">Type of the return value</typeparam>
		/// <param name="result"><see cref="IExtendedAsyncResult"/> returned by a call to the <c>BeginCall</c> method</param>
		/// <returns>Return value</returns>
		TRes EndCall<TRes>(IExtendedAsyncResult<TRes> result);

		/// <summary>
		/// Ends a pending asynchronous call
		/// </summary>
		/// <typeparam name="TRes">Type of the return value</typeparam>
		/// <typeparam name="TOut1">Type of the 1st additional result value</typeparam>
		/// <param name="out1">Returned 1st additional value</param>
		/// <param name="result"><see cref="IExtendedAsyncResult"/> returned by a call to the <c>BeginCall</c> method</param>
		/// <returns>Return value</returns>
		TRes EndCall<TRes, TOut1>(out TOut1 out1, IExtendedAsyncResult<TRes> result);

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
		TRes EndCall<TRes, TOut1, TOut2>(out TOut1 out1, out TOut2 out2, IExtendedAsyncResult<TRes> result);
	}
}
