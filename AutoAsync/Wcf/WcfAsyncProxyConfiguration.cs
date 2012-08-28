using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace AutoAsync.Wcf
{
	/// <summary>
	/// Represents extended configuration for WCF-based <c>AsyncProxy</c> service
	/// </summary>
	public class WcfAsyncProxyConfiguration
	{
		/// <summary>
		/// Initializes a new instance of the <c>WcfAsyncProxyConfiguration</c> class
		/// </summary>
		/// <param name="binding">The Binding used to configure the endpoint</param>
		/// <param name="remoteAddress">The address that provides the location of the service</param>
		public WcfAsyncProxyConfiguration(Binding binding, string remoteAddress)
			: this(binding, new EndpointAddress(remoteAddress))
		{ }

		/// <summary>
		/// Initializes a new instance of the <c>WcfAsyncProxyConfiguration</c> class
		/// </summary>
		/// <param name="endpointConfigurationName">The configuration name used for the endpoint.</param>
		/// <param name="remoteAddress">The <c>EndpointAddress</c> that provides the location of the service</param>
		public WcfAsyncProxyConfiguration(string endpointConfigurationName, EndpointAddress remoteAddress)
		{
			EndpointConfigurationName = endpointConfigurationName;
			RemoteAddress = remoteAddress;
		}

		/// <summary>
		/// Initializes a new instance of the <c>WcfAsyncProxyConfiguration</c> class
		/// </summary>
		/// <param name="binding">The Binding used to configure the endpoint</param>
		/// <param name="remoteAddress">The <c>EndpointAddress</c> that provides the location of the service</param>
		public WcfAsyncProxyConfiguration(Binding binding, EndpointAddress remoteAddress)
		{
			Binding = binding;
			RemoteAddress = remoteAddress;
		}

		/// <summary>
		/// Initializes a new instance of the <c>WcfAsyncProxyConfiguration</c> class
		/// </summary>
		/// <param name="endpointConfigurationName">The configuration name used for the endpoint.</param>
		public WcfAsyncProxyConfiguration(string endpointConfigurationName)
		{
			EndpointConfigurationName = endpointConfigurationName;
		}



		/// <summary>
		/// Should events use synchronization context
		/// </summary>
		public bool UseSynchronizationContext { get; set; }

		/// <summary>
		/// Timeout on proxy disposal
		/// </summary>
		public TimeSpan? DisposeTimeout { get; set; }



		/// <summary>
		/// The Binding used to configure the endpoint
		/// </summary>
		public Binding Binding { get; private set; }

		/// <summary>
		/// The address that provides the location of the service
		/// </summary>
		public EndpointAddress RemoteAddress { get; private set; }

		/// <summary>
		/// The configuration name used for the endpoint
		/// </summary>
		public string EndpointConfigurationName { get; private set; }



		/// <summary>
		/// Just clones:)
		/// </summary>
		public object Clone()
		{ return MemberwiseClone(); }
	}
}
