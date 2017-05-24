using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SunShine_Hotel.Businees;

namespace SunShine_Hotel.Businees_Layer
{
public	class Guests
{
//public DateTime checkInDate { get; set; }

		//get everything about guests from the database and display
		public IEnumerable AllGuests()
		{
			using (var context = new SunShine_HotelEntities())
			{
				var allGuests = from g in context.Guests
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
						g.BookingDate
					};
				return allGuests.ToList();
			}
			
		}

		public void NewGuest()
		{   //get the current booking date
			DateTime BookingDate;
			BookingDate = DateTime.Now;
			//get the last booking ID from the new booking, really difficult 
			
			using ( var context = new SunShine_HotelEntities())
			{ //get the last booking ID from the new booking, really difficult 
				var bookingId =
				Convert.ToInt16(
					context.Bookings.OrderByDescending(b => b.BookingID).Select(c => c.BookingID).First());
				//var RoomID = Convert.ToInt16(
				//	context.Bookings.OrderByDescending(b => b.BookingID).Select(c => c.RoomIDFK).First());
				var AddGuest = new Guest();

				AddGuest.Name = BookingDetailsDTO.guestName;
				AddGuest.Address = BookingDetailsDTO.guestAddress;
				AddGuest.Guests = BookingDetailsDTO.GuestNumbers;
				AddGuest.Phone = BookingDetailsDTO.GuestPhoneNo;
				AddGuest.Room = BookingDetailsDTO.roomID;
				AddGuest.BookingDate = BookingDate;
				AddGuest.BookingIDFK = bookingId;
				context.Guests.Add(AddGuest);
				context.SaveChanges();
			}
		}

	public DateTime checkInDate(int bookingID)
	{
		using (var context = new SunShine_HotelEntities())
		{
			DateTime Date = Convert.ToDateTime(
				context.Bookings.FirstOrDefault(b => b.BookingID == bookingID).BookingFrom
				);
			return Date;
		}
	}

	//confirm guest check in
		public void CheckIn(int bookingId)
		{

			using (var context = new SunShine_HotelEntities())
			{
				//checkInDate = Convert.ToDateTime(
				//context.Bookings.Where(b => b.BookingID == bookingId).Select(b => b.BookingFrom));


				var confirm = context.Guests.FirstOrDefault(b => b.BookingIDFK == bookingId);
				confirm.CheckIn = DateTime.Now;



				context.SaveChanges();
			}
		}

		public void DeleteGuest(int guestID)
		{
			using (var context = new SunShine_HotelEntities())
			{
				var deleteGuest = context.Guests.FirstOrDefault(g => g.GuestID == guestID);


				context.Guests.Remove(deleteGuest);
				context.SaveChanges();
                MessageBox.Show("Guest has been deleted successfully ....");
			}
		}

		public void updateGuest(int guestId)
		{
			using (var context = new SunShine_HotelEntities())
			{
				var update = context.Guests.FirstOrDefault(g => g.GuestID == guestId);
				update.Name = BookingDetailsDTO.guestName;
				update.Address = BookingDetailsDTO.guestAddress;
				update.Phone = BookingDetailsDTO.GuestPhoneNo;
				update.Guests = BookingDetailsDTO.GuestNumbers;
				update.Room = BookingDetailsDTO.roomID;
				context.SaveChanges();
			}
		}

	public void GuestBookingInfo(int bookingID)
	{
		

		using (var context = new SunShine_HotelEntities())
		{
				int roomID = 0;
				DateTime bookingFrom = new DateTime(), BookingTo = new DateTime();
				decimal roomCost=0;
				{
					var loadGuestBooking =
					   context.Bookings.Where(b => b.BookingID == bookingID)
						   .Select(b => new { b.BookingID, b.RoomIDFK, b.BookingFrom, b.BookingTo, b.RoomCost})
						   .SingleOrDefault();
					
					roomID = loadGuestBooking.RoomIDFK;
					bookingFrom = loadGuestBooking.BookingFrom.Value;
					BookingTo = loadGuestBooking.BookingTo.Value;
					roomCost = loadGuestBooking.RoomCost.Value;
					//return bookingInfo.ToList();
				}
				
				MessageBox.Show("Room :" + roomID+ " " + "is booked from " + bookingFrom + " - " + BookingTo 
					+"\n" +" At the Cost of : $"+ roomCost+ " " + " For this Guest.");
			}
	}

	public IEnumerable GuestsWithoutCheckIN()
	{  //get all the guests data who need to check in
			using (var context = new SunShine_HotelEntities())
			{
				var CheckInEmpty = from g in context.Guests
								   join b in context.Billings on g.GuestID equals b.GuestIDFK
								   where g.CheckIn == null && g.CheckOut == null orderby  g.GuestID descending 
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
				return CheckInEmpty.ToList();

			}
		}
		public IEnumerable GuestsWithoutCheckOut()
		{//get the guest data who need to check Out 
			using (var context = new SunShine_HotelEntities())
			{
				var CheckOutEmpty = from g in context.Guests
									join b in context.Billings on g.GuestID equals b.GuestIDFK
									where g.CheckIn != null && g.CheckOut==null
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
				return CheckOutEmpty.ToList();

			}
		}

	public void GuestsCheckOut(int guestID, decimal total)
		{
			

			using (var context = new SunShine_HotelEntities())
		    { //check if the guest checked In or not
			    var currentGuest = context.Guests.SingleOrDefault(g => g.GuestID == guestID);
			    if (currentGuest.CheckIn == null)
			    {
				    MessageBox.Show(" Guest hasn't checked in yet");

			    }
			    else
			    {
					MessageBox.Show("Guest is ready to check out" + "\n" + "With Total Expenses to pay :  $" + total);
					currentGuest.CheckOut = DateTime.Now.Date;

				 context.SaveChanges();
			    }
			}
			
		}
}
}
