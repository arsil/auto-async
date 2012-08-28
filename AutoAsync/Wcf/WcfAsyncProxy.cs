using System;
using System.Reflection;
using System.ServiceModel;
using System.Threading;

using AutoAsync.Base;

namespace AutoAsync.Wcf
{
	class WcfAsyncProxy<TInterface> : BaseAsyncProxy<TInterface> where TInterface : class
	{
		public WcfAsyncProxy(object service, Func<MethodInfo, WcfAsyncMethodsHolder> getAsyncMethods,
			WcfAsyncProxyConfiguration configuration)
		{
			_service = service;
			_getAsyncMethods = getAsyncMethods;
			_configuration = configuration;
		}

		protected override IExtendedAsyncResult ProcessBeginCall(
			MethodInvocationParams mip, Action<IExtendedAsyncResult> callBack, object userState)
		{
			var asyncMethods = _getAsyncMethods(mip.Method);

			var callResult = new WcfExtendedAsyncResult(_service, callBack, asyncMethods.EndCall);

			// obserwacja: jeśli poleci callback, to będzie miał oryginalne iasyncresult => nie potrzebujemy więc go w WcfExtendedAsyncResult w takim przypadku - tłumacz może go nam wyprodukować!
			// obserwacja: potrzebujemy gotowe WcfExtendedAsyncResult tylko gdy wychodziny stąd! czyli możemy to gówno ustawić przy wychodzeniu!
			// todo: straszna to wioska jest, ale cóż.... 
			callResult.OrgAsyncResult = asyncMethods.BeginCall(
				_service,
				mip.Arguments,
				WrapSynchonizationContextIfNeeded(callResult.FireOnInternalWcfCallCompleted),
				userState);

			return callResult;
		}

		protected override IExtendedAsyncResult<TRes> ProcessBeginCall<TRes>(
			MethodInvocationParams mip, Action<IExtendedAsyncResult<TRes>> callBack, object userState)
		{
			var asyncMethods = _getAsyncMethods(mip.Method);

			var callResult = new WcfExtendedAsyncResult<TRes>(_service, callBack, asyncMethods.EndCall);

			// obserwacja: jeśli poleci callback, to będzie miał oryginalne iasyncresult => nie potrzebujemy więc go w WcfExtendedAsyncResult w takim przypadku - tłumacz może go nam wyprodukować!
			// obserwacja: potrzebujemy gotowe WcfExtendedAsyncResult tylko gdy wychodziny stąd! czyli możemy to gówno ustawić przy wychodzeniu!
			// todo: straszna to wioska jest, ale cóż.... 
			callResult.OrgAsyncResult = asyncMethods.BeginCall(
				_service,
				mip.Arguments,
				WrapSynchonizationContextIfNeeded(callResult.FireOnInternalWcfCallCompleted),
				userState);

			return callResult;
		}

		 // todo: check why two factories for the same type produce exceptions!----------------------------------------------------------------------&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&

		public override void Dispose()
		{
			var disp = (IClientChannel)_service;

			try
			{
				if (disp.State == CommunicationState.Faulted)
				{
					disp.Abort();
				}
				else
				{
					if (_configuration.DisposeTimeout != null)
						disp.Close(_configuration.DisposeTimeout.Value);
					else
						disp.Close();
				}
			}
			catch (TimeoutException)
			{
				disp.Abort();
			}
			catch (CommunicationException)
			{
				disp.Abort();
			}
			catch (Exception)
			{
				 // todo: do we really wana throw this????
				disp.Abort();
				throw;
			}

			try
			{
				// just in case...
				disp.Dispose();
			}
			// ReSharper disable EmptyGeneralCatchClause
			catch (Exception)
			{
			}
			// ReSharper restore EmptyGeneralCatchClause
		}

		private AsyncCallback WrapSynchonizationContextIfNeeded(AsyncCallback callBack)
		{
			if (_configuration.UseSynchronizationContext && callBack != null)
			{
				var currentContext = SynchronizationContext.Current;
				if (currentContext != null)
					return a => currentContext.Post(
						r2 => callBack(a), null);
			}

			return callBack;
		}

		private readonly Func<MethodInfo, WcfAsyncMethodsHolder> _getAsyncMethods;
		private readonly object _service;
		private readonly WcfAsyncProxyConfiguration _configuration;
	}
}
