namespace AutoAsync
{
	/// <summary>
	/// Provides method form creating <c>IAsyncProxy</c> instances
	/// </summary>
	/// <typeparam name="TInterface">Service interface type</typeparam>
	public interface IAsyncProxyCreator<TInterface> where TInterface : class
	{
		/// <summary>
		/// Creates new <c>IAsyncProxy</c> instance
		/// </summary>
		/// <returns><c>IAsyncProxy</c> instance</returns>
		IAsyncProxy<TInterface> Get();
	}
}
