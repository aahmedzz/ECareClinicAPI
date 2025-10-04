using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECareClinic.Core.Enums
{
	[Flags]
	public enum VisitType
	{
		InPerson = 1,
		VideoCall = 2
	}
	public static class VisitTypeExtensions
	{
		public static List<string> ToListOfStrings(this VisitType visitTypes)
		{
			return Enum.GetValues(typeof(VisitType))
				.Cast<VisitType>()
				.Where(v => visitTypes.HasFlag(v))
				.Select(v => v.ToString())
				.ToList();
		}
	}
}
