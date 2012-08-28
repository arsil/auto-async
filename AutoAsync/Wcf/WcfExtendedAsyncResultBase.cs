using System;
using System.Threading;

namespace AutoAsync.Wcf
{
	    // todo: shouldn't this class be a base class!!!! isn't it suitable for general usage!!!!????  !? !? ! ?! ?! ?! ?! ?! ? !? ?! ?!? !? !? ?! ?! ?? ?! !? ?! ?! ? !? !?
	    // todo: PROBABLY NOT, because it uses OrgAsyncResult... which is probably not needed in other desingns???
	    // todo: describe what is it for... how to use it... why such a shamefull design:)

	internal abstract class WcfExtendedAsyncResultBase : IAsyncResult
	{
		public bool IsCompleted
		{
			get { return OrgAsyncResult.IsCompleted; }
		}

		public WaitHandle AsyncWaitHandle
		{
			get { return OrgAsyncResult.AsyncWaitHandle; }
		}

		public object AsyncState
		{
			get { return OrgAsyncResult.AsyncState; }
		}

		public bool CompletedSynchronously
		{
			get { return OrgAsyncResult.CompletedSynchronously; }
		}

		protected abstract void OnInternalWcfCallCompleted();

		internal void FireOnInternalWcfCallCompleted(IAsyncResult ar)
		{
			    // todo: find out if we need lock here. we are changing OrgAsyncResult!!! this could happen from the other thread!! so probably YES!
			    // todo: if so, we need also lock on propery change!!!!
			if (OrgAsyncResult == null)
				OrgAsyncResult = ar;

			OnInternalWcfCallCompleted();
		}

		    // todo: see previous comment! we need probably lock here!!! --------------------------------------------------0234 -2394 02 3-49 349 -02394- 0293-4 92-349 -02934 -02934092-349 -02394 -09234 923904 
		    // todo: also think about desing - why internal? can we do something about it????---------------------------- -2-042 349-23 94 -9249 -023923 94-0239 29 49 2349 -02394 9 023-
		    // todo: this mutablility sucks!!!!
		internal IAsyncResult OrgAsyncResult { get; set; }
	}
}