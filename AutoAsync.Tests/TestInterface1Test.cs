using System;
using System.ServiceModel;
using AutoAsync.Wcf;
using NUnit.Framework;

namespace AutoAsync.Tests
{
	[TestFixture]
	public class TestInterface1Test
	{
		[Test]
		public void SimpleAsyncCallTest()
		{
			var baseAddress = new Uri("http://127.0.0.1:9080");

			// Create the ServiceHost.
			var host = new ServiceHost(typeof(Test1InterfaceImpl), baseAddress);
			host.AddServiceEndpoint(
				typeof(ITest1Interface),
				new NetNamedPipeBinding(NetNamedPipeSecurityMode.None),
				"net.pipe://localhost/ITestInterface1");
			host.Open();

			var endPoint = new EndpointAddress("net.pipe://localhost/ITestInterface1");
			var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
				new BasicHttpBinding();

			var factory = new WcfAsyncProxyCreator<ITest1Interface>(binding, endPoint);
			using (var proxy = factory.Get())
			{
				var asyncResult = proxy.BeginCall(i => i.GetStringLength("01234567"), null, null);
				Assert.AreEqual(8, asyncResult.End());

				long dummy;
				var res2 = proxy.BeginCall(i => i.GetStringLengthAsLong("0123", out dummy), c =>
					{
						long result;
						c.End(out result);
						Assert.AreEqual(4, result);
					}
					, null);
				res2.AsyncWaitHandle.WaitOne(10000);

				int dummy1 = 9;
				float dummy2;

				var res3 = proxy.BeginCall(i =>
					i.GetStringUpperCaseAndSomeLengths("To ja sem", ref dummy1, out dummy2),
					c =>
						{
							int result1;
							float result2;

							var uppered = c.End(out result1, out result2);

							Assert.AreEqual("TO JA SEM", uppered);
							Assert.AreEqual(18, result1);
							Assert.AreEqual((9 / 100.0f), result2);

						}, 6);
				res3.AsyncWaitHandle.WaitOne(10000);

				asyncResult = proxy.BeginCall(i => i.GetStringLength("0"), null, null);
				Assert.AreEqual(1, asyncResult.End());
			}

			host.Close();
		}

		[Test]
		public void MultipleFactoriesTest()
		{
			var baseAddress = new Uri("http://127.0.0.1:9080");

			// Create the ServiceHost.
			var host = new ServiceHost(typeof(Test1InterfaceImpl), baseAddress);
			host.AddServiceEndpoint(
				typeof(ITest1Interface),
				new NetNamedPipeBinding(NetNamedPipeSecurityMode.None),
				"net.pipe://localhost/ITestInterface1");
			host.Open();

			var endPoint = new EndpointAddress("net.pipe://localhost/ITestInterface1");
			var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
			new BasicHttpBinding();

			var factory1 = new WcfAsyncProxyCreator<ITest1Interface>(binding, endPoint);
			using (var proxy1 = factory1.Get())
			{
				var asyncResult = proxy1.BeginCall(i => i.GetStringLength("01234567"), null, null);
				Assert.AreEqual(8, asyncResult.End());

				var factory2 = new WcfAsyncProxyCreator<ITest1Interface>(binding, endPoint);
				using (var proxy2 = factory2.Get())
				{
					var asyncResult2 = proxy2.BeginCall(i => i.GetStringLength("01234567"), null, null);
					Assert.AreEqual(8, asyncResult2.End());
				}
			}

			host.Close();
		}
	}
}
