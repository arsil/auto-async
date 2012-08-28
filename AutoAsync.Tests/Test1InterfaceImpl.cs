using System;

namespace AutoAsync.Tests
{
	class Test1InterfaceImpl : ITest1Interface
	{
		public int GetStringLength(string text)
		{
			return text.Length;
		}

		public string GetStringUpperCaseAndSomeLengths(string text, ref int bida, out float kasza)
		{
			bida = bida + text.Length;
			kasza = (text.Length / 100.0f);

			return text.ToUpper();
		}

		public void GetStringLengthAsLong(string text, out long stringLength)
		{
			stringLength = text.Length;
		}
	}
}
