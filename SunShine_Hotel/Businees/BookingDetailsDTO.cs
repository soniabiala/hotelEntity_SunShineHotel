using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace SunShine_Hotel.Businees
{
static	class  BookingDetailsDTO
	{
		public static DateTime BookingFrom { get; set; }
		public static DateTime BookingTo { get; set; }
		public static int GuestNumbers { get; set; }
		public static int roomID { get; set; }
		public static decimal roomCost { get; set; }
		public  static string guestName { get; set; }
		public static string guestAddress { get; set; }
		public static int GuestPhoneNo { get; set; }


	}
}
