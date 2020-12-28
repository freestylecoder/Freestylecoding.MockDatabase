using System;

namespace Freestylecoding.MockableDatabase.Tests {
	internal class EvilValueException : Exception {
		public const int EvilValue = 666;
	}
}
