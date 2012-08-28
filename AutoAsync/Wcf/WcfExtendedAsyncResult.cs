using System;

namespace AutoAsync.Wcf
{
	class WcfExtendedAsyncResult<TRes> : WcfExtendedAsyncResultBase, IExtendedAsyncResult<TRes>
	{
		public WcfExtendedAsyncResult(
			object service,
			Action<IExtendedAsyncResult<TRes>> onCompleted,
			Func<object, WcfExtendedAsyncResult<TRes>, object[]> endCall)
		{
			_service = service;
			_onCompleted = onCompleted;
			_endCall = endCall;
		}

		public TRes End()
		{
			var result = _endCall(_service, this);

			return (TRes)result[0];
		}

		public TRes End<TOut1>(out TOut1 out1)
		{
			var result = _endCall(_service, this);
			out1 = (TOut1) result[1];

			return (TRes)result[0];
		}

		public TRes End<TOut1, TOut2>(out TOut1 out1, out TOut2 out2)
		{
			var result = _endCall(_service, this);
			out1 = (TOut1)result[1];
			out2 = (TOut2)result[2];

			return (TRes)result[0];
		}

		protected override void OnInternalWcfCallCompleted()
		{
			if (_onCompleted != null)
				_onCompleted(this);
		}

		private readonly object _service;
		private readonly Func<object, WcfExtendedAsyncResult<TRes>, object[]> _endCall;
		private readonly Action<IExtendedAsyncResult<TRes>> _onCompleted;
	}

	class WcfExtendedAsyncResult : WcfExtendedAsyncResultBase, IExtendedAsyncResult
	{
		public WcfExtendedAsyncResult(
			object service,
			Action<IExtendedAsyncResult> onCompleted,
			Func<object, WcfExtendedAsyncResult, object[]> endCall)
		{
			_service = service;
			_onCompleted = onCompleted;
			_endCall = endCall;
		}

		public void End()
		{
			_endCall(_service, this);
		}

		public void End<TOut1>(out TOut1 out1)
		{
			var result = _endCall(_service, this);
			out1 = (TOut1)result[0];
		}

		public void End<TOut1, TOut2>(out TOut1 out1, out TOut2 out2)
		{
			var result = _endCall(_service, this);
			out1 = (TOut1)result[0];
			out2 = (TOut2)result[1];
		}

		protected override void OnInternalWcfCallCompleted()
		{
			if (_onCompleted != null)
				_onCompleted(this);
		}

		private readonly object _service;
		private readonly Func<object, WcfExtendedAsyncResult, object[]> _endCall;
		private readonly Action<IExtendedAsyncResult> _onCompleted;
	}
}
