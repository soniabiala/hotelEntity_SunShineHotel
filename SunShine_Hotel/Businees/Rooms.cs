using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SunShine_Hotel.Businees_Layer
{
	class Rooms
	{
		public IEnumerable AllRooms(List<int> RoomID)
		{// get all the bookings from the booking table
			using (var context = new SunShine_HotelEntities())
			{
				var bookings = from r in context.Rooms
							   select new
							   {
								   r.RoomID,
								   r.SingleBeds,
								   r.DoubleBeds,
								   r.TariffSinglePerson,
								   r.TariffDoublePerson,
								   r.TariffExtraPerson
								   
								 

							   };
				return bookings.ToList();
			}
		}

		public void updateRoom(int roomID, int singleBed, int doublrBed, decimal tariffSingle, decimal tariffdouble, decimal tariffExtraPerson)
		{
			using (var context = new SunShine_HotelEntities())
			{
				var update = context.Rooms.FirstOrDefault(r => r.RoomID== roomID);
				update.SingleBeds=singleBed;
				update.DoubleBeds=doublrBed;
				update.TariffSinglePerson = tariffSingle;
				update.TariffDoublePerson= tariffdouble;
				update.TariffExtraPerson = tariffExtraPerson;
				
				context.SaveChanges();
			}

		}

		public void addRoom(int singleBed, int doubleBed, decimal tariffSingle, decimal tariffDouble, decimal tariffExtra)
		{
			using (var context = new SunShine_HotelEntities())
			{
				
				var AddRoom = new Room();
				AddRoom.SingleBeds = singleBed;
				AddRoom.DoubleBeds = doubleBed;
				AddRoom.TariffSinglePerson = tariffSingle;
				AddRoom.TariffDoublePerson = tariffDouble;
				AddRoom.TariffExtraPerson = tariffExtra;
				
				context.Rooms.Add(AddRoom);
				context.SaveChanges();
			}
		}

		internal void DeleteRoom(int roomID)
		{
			using (var context = new SunShine_HotelEntities())
			{

				var roomBooked = context.Bookings.FirstOrDefault(b => b.RoomIDFK == roomID);
				if (roomBooked.BookingFrom > DateTime.Today.Date)
				{
					MessageBox.Show("Can't delete the room : " + roomID + " as its booked on : " + roomBooked.BookingFrom);
				}
				else
				{
				var deleteRoom = context.Rooms.FirstOrDefault(r => r.RoomID == roomID);

				context.Rooms.Remove(deleteRoom);
				context.SaveChanges();
				MessageBox.Show("Room" + " " + roomID + " " + " has been deleted successfully ....");	
				}
				
				
			}
		}
	}
}
