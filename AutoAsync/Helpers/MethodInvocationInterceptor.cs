using System;
using Castle.DynamicProxy;

using AutoAsync.Base;

namespace AutoAsync.Helpers
{
	class MethodInvocationParamsInterceptor : IInterceptor
	{
		public MethodInvocationParamsInterceptor(Action<MethodInvocationParams> setInvoicationParams)
		{
			_setInvoicationParams = setInvoicationParams;
		}

		public void Intercept(IInvocation invocation)
		{
			// todo: generic method handling - not applicable to Wcf, but usefull!
//			MethodInfo mi = invocation.Method.IsGenericMethod
//				? invocation.Method.GetGenericMethodDefinition()
//				: invocation.Method;
			//invocation.Method
			//invocation.ReturnValue
			//invocation.GenericArguments
			//invocation.Arguments
			var mip = new MethodInvocationParams(
				invocation.Method,
				invocation.Arguments,
				invocation.GenericArguments);

			_setInvoicationParams(mip);

			var retType = invocation.Method.ReturnType;
			if (retType.IsValueType && retType != typeof(void))
				invocation.ReturnValue = Activator.CreateInstance(invocation.Method.ReturnType);
		}

		private readonly Action<MethodInvocationParams> _setInvoicationParams;
	}
}
