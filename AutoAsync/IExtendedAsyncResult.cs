using System;

namespace AutoAsync
{
	/// <summary>
	/// Represents extended status of an asynchronous operation
	/// </summary>
	public interface IExtendedAsyncResult : IAsyncResult
	{
		/// <summary>
		/// Ends a pending asynchronous call.
		/// </summary>
		void End();

		/// <summary>
		/// Ends a pending asynchronous call.
		/// </summary>
		/// <typeparam name="TOut1">Type of the 1st result value</typeparam>
		/// <param name="out1">Returned 1st value</param>
		void End<TOut1>(out TOut1 out1);

		/// <summary>
		/// Ends a pending asynchronous call
		/// </summary>
		/// <typeparam name="TOut1">Type of the 1st result value</typeparam>
		/// <typeparam name="TOut2">Type of the 2nd result value</typeparam>
		/// <param name="out1">Returned 1st value</param>
		/// <param name="out2">Returned 2nd value</param>
		void End<TOut1, TOut2>(out TOut1 out1, out TOut2 out2);
	}

	/// <summary>
	/// Represents extended status of an asynchronous operation
	/// </summary>
	/// <typeparam name="TRes">Type of the operation's return value</typeparam>
	public interface IExtendedAsyncResult<TRes> : IAsyncResult
	{
		/// <summary>
		/// Ends a pending asynchronous call
		/// </summary>
		/// <returns>Return value</returns>
		TRes End();

		/// <summary>
		/// Ends a pending asynchronous call
		/// </summary>
		/// <typeparam name="TOut1">Type of the 1st additional result value</typeparam>
		/// <param name="out1">Returned 1st additional value</param>
		/// <returns>Return value</returns>
		TRes End<TOut1>(out TOut1 out1);

		/// <summary>
		/// Ends a pending asynchronous call
		/// </summary>
		/// <typeparam name="TOut1">Type of the 1st additional result value</typeparam>
		/// <typeparam name="TOut2">Type of the 2nd additional result value</typeparam>
		/// <param name="out1">Returned 1st additional value</param>
		/// <param name="out2">Returned 2nd additional value</param>
		/// <returns>Return value</returns>
		TRes End<TOut1, TOut2>(out TOut1 out1, out TOut2 out2);
	}
}
