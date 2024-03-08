using System;
using System.Diagnostics;
namespace TASStudio.Entities {
	//.load C:\Windows\Microsoft.NET\Framework\v4.0.30319\SOS.dll
	public class GameMemory {
		private static ProgramPointer TAS = new ProgramPointer(AutoDeref.Single, new ProgramSignature(PointerVersion.Steam, "C745F8F1DEBC9AC745FC785634128D4DF8", 24));
		public Process Program { get; set; }
		private DateTime lastHooked;

		public GameMemory() {
			lastHooked = DateTime.MinValue;
		}
    }
}