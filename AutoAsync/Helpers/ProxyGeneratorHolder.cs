using Castle.DynamicProxy;

namespace AutoAsync.Helpers
{
	/// <summary>
	/// Simple static helper holding an instance of ProxyGenerator
	/// </summary>
	static class ProxyGeneratorHolder
	{
		/// <summary>
		/// DynamicProxy's proxy generator
		/// </summary>
		public static ProxyGenerator Generator = new ProxyGenerator();
	}
}
