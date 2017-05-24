using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SunShine_Hotel.Data
{

	public static class Database
	{
		//public DataGridView MyDGVGuests { get; set; }
		//public DataGridView MyDGVRooms { get; set; }

		public static DataGridView Load_DGVGuests()
		{

			using (var context = new SunShine_HotelEntities())
			{
				DataGridView dgv = new DataGridView();

				//var guests = context.Guests.Select(g => new
				//{
				//	g.GuestID,
				//	g.BookingIDFK,
				//	g.Name,
				//	g.Address,
				//	g.Phone,
				//	g.Guests,
				//	g.Room,
				//	g.CheckIn,
				//	g.CheckOut,
				//	g.BookingDate
				//}).OrderByDescending(g => g.GuestID);
				var guests = from g in context.Guests
					join b in context.Billings on g.GuestID equals b.GuestIDFK
					orderby g.GuestID descending
					select new
					{
						g.GuestID,
						g.BookingIDFK,
						g.Name,
						g.Address,
						g.Phone,
						g.Guests,
						g.Room,
						g.CheckIn,
						g.CheckOut,
						g.BookingDate,
						b.BillingID,
						b.BarCharge,
						b.TeleChrage,
						b.RoomCharge,
						b.WiFiCharges


					};

				dgv.DataSource = guests.ToList();
				return dgv;
			}


		}

		public static DataGridView Load_DGVRooms()
		{
			using (var context = new SunShine_HotelEntities())
			{
				DataGridView dgv = new DataGridView();

				var Allrooms = from r in context.Rooms.OrderByDescending(r => r.RoomID)
					//because you are selecting more than 1 field you must wrap it in new { }
					select new
					{
						r.RoomID,
						r.SingleBeds,
						r.DoubleBeds,
						r.TariffSinglePerson,
						r.TariffDoublePerson,
						r.TariffExtraPerson
					};
				//send it out to your datagrid view

				dgv.DataSource = Allrooms.ToList();

				return dgv;

			}

		}

		public static DataGridView Load_DGVBookings()
		{
			using (var context = new SunShine_HotelEntities())
			{
				DataGridView dgv = new DataGridView();

				var rooms = from b in context.Bookings.OrderByDescending(b => b.BookingID)
					//because you are selecting more than 1 field you must wrap it in new { }
					select new
					{
						b.BookingID,
						b.RoomIDFK,
						b.BookingFrom,
						b.BookingTo,
						b.RoomCost

					};
				//send it out to your datagrid view
				dgv.DataSource = rooms.ToList();

				return dgv;

			}

		}

		public static DataGridView Load_DGVBilling()
		{
			using (var context = new SunShine_HotelEntities())
			{
				DataGridView dgv = new DataGridView();

				var billing = from b in context.Billings.OrderByDescending(b => b.BillingID)
					//because you are selecting more than 1 field you must wrap it in new { }
					select new
					{
						b.BillingID,
						b.GuestIDFK,
						b.BarCharge,
						b.TeleChrage,
						b.RoomCharge,
						b.WiFiCharges
					};
				//send it out to your datagrid view
				dgv.DataSource = billing.ToList();

				return dgv;

			}

		}

		public static IEnumerable<Room> GetFreeRoomsAndStuff(IEnumerable<int> allRoomsFree)
		{
			List<Room> FreeRooms = new List<Room>();

			using (var context = new SunShine_HotelEntities())
			{

				var AllFreerooms =  context.Rooms.Where(r => allRoomsFree.Contains(r.RoomID)).ToList();

				//output the other details to a list
				FreeRooms = AllFreerooms;
			}
			//returm must be outside the using statement
			return FreeRooms;
		}

		public static List<Room> RoomClickedDetails(IEnumerable<int> allRoomsFree)

		{
			//var RoomDetail = new DataGridView();
			using (var context = new SunShine_HotelEntities())
			{
				var Allrooms =  context.Rooms.Where(r => allRoomsFree.Contains(r.RoomID)).ToList();
              return     Allrooms;
		    
			}
		}

		internal static DataGridView ListAllBookingsAfterToday()
		{
			using (var context = new SunShine_HotelEntities())
			{
				DataGridView dgv = new DataGridView();

				var allBookings =
						   context.Bookings.Where(b => b.BookingFrom >= DateTime.Today.Date)
							   .Select(b => new { b.RoomIDFK, b.BookingFrom, b.BookingTo })
							   .OrderBy(b => b.RoomIDFK)
							   .ThenBy(b => b.BookingFrom);
				//send it out to your datagrid view
				dgv.DataSource = allBookings.ToList();

				return dgv;
			}
		}
	}
}
