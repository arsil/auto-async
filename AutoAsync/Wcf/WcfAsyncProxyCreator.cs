using System;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace AutoAsync.Wcf
{
	/// <summary>
	/// Defines creation method for WCF based <c>IAsyncProxy</c> communication.
	/// </summary>
	/// <typeparam name="TInterface">Synchronous interface that we want to use asynchronously</typeparam>
	public class WcfAsyncProxyCreator<TInterface> : IAsyncProxyCreator<TInterface> where TInterface : class
	{
		/// <summary>
		/// Initializes a new instance of the <c>WcfAsyncProxyCreator</c> class
		/// </summary>
		/// <param name="binding">The Binding used to configure the endpoint</param>
		/// <param name="remoteAddress">The address that provides the location of the service</param>
		public WcfAsyncProxyCreator(Binding binding, string remoteAddress)
			: this(new WcfAsyncProxyConfiguration(binding, remoteAddress))
		{ }

		/// <summary>
		/// Initializes a new instance of the <c>WcfAsyncProxyCreator</c> class
		/// </summary>
		/// <param name="binding">The Binding used to configure the endpoint</param>
		/// <param name="remoteAddress">The <c>EndpointAddress</c> that provides the location of the service</param>
		public WcfAsyncProxyCreator(Binding binding, EndpointAddress remoteAddress)
			: this(new WcfAsyncProxyConfiguration(binding, remoteAddress))
		{ }

		/// <summary>
		/// Initializes a new instance of the <c>WcfAsyncProxyCreator</c> class
		/// </summary>
		/// <param name="configuration">Extended configuration</param>
		public WcfAsyncProxyCreator(WcfAsyncProxyConfiguration configuration)
		{
			WcfAsyncProxyCache.GetAsyncInterface(
				typeof(TInterface), out _asyncInterfaceType, out _methodInfoToAsyncFunc);

			var genericFactoryType = typeof(ChannelFactory<>);
			var factoryType = genericFactoryType.MakeGenericType(_asyncInterfaceType); // here we have async interface!

			if (configuration.RemoteAddress != null && configuration.Binding != null)
				_channelFactory = Activator.CreateInstance(factoryType, new object[] { configuration.Binding, configuration.RemoteAddress });
			else if (configuration.RemoteAddress != null && configuration.Binding == null)
				_channelFactory = Activator.CreateInstance(factoryType, new object[] { configuration.EndpointConfigurationName, configuration.RemoteAddress });
			else
				_channelFactory = Activator.CreateInstance(factoryType, new object[] { configuration.EndpointConfigurationName });

			_configuration = (WcfAsyncProxyConfiguration)configuration.Clone();
			_channelCreator = factoryType.GetMethod("CreateChannel", new Type[] { });
		}

		/// <summary>
		/// Creates new <c>IAsyncProxy</c> instance
		/// </summary>
		/// <returns><c>IAsyncProxy</c> instance</returns>
		public IAsyncProxy<TInterface> Get()
		{
			object service = _channelCreator.Invoke(_channelFactory, new object[] { });

			return new WcfAsyncProxy<TInterface>(service, GetAsyncMethods, _configuration);
		}

		private WcfAsyncMethodsHolder GetAsyncMethods(MethodInfo mi)
		{
			return _methodInfoToAsyncFunc(mi);
		}

		private readonly Type _asyncInterfaceType;
		private readonly Func<MethodInfo, WcfAsyncMethodsHolder> _methodInfoToAsyncFunc;

		private readonly object _channelFactory;
		private readonly MethodInfo _channelCreator;
		private readonly WcfAsyncProxyConfiguration _configuration;
	}
}
