using System.ServiceModel;

namespace AutoAsync.Tests
{
	[ServiceContract]
	public interface ITest1Interface
	{
		[OperationContract]
		int GetStringLength(string text);

		[OperationContract]
		string GetStringUpperCaseAndSomeLengths(
			string text, ref int bida, out float kasza);

		[OperationContract]
		void GetStringLengthAsLong(string text, out long stringLength);
	}
}
