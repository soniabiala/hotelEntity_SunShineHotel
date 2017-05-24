using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using SunShine_Hotel.Businees_Layer;
using SunShine_Hotel.Data;

//using CSharp_Entity_Hotel_Assessment_2013.BusinessLayer.RoomBookingClasses;
//using SunShine_Hotel.Businees;
//using SunShine_Hotel.Data_Layer;

namespace SunShine_Hotel.Businees
{
public	class Bookings
{
	private readonly Dictionary<int, decimal> _roomCostDict = new Dictionary<int, decimal>();
	 Billings myBilling = new Billings();
	Guests myguest = new Guests();



	public List<string> OutputAllRoomCosts = new List<string>();

	public List<string> OutputAllRoomOverflow = new List<string>();
	
		private double _daysBooked { get; set; }
		private bool _isRoomConflict { get; set; }

		public string BookedConfirmationMessage { get; set; }

		
		public int SingleBeds { get; set; }
		public int DoubleBeds { get; set; }
		
		public static decimal roomCost { get; set; }
		public int RowIdSelected { get; set; }
		public int RoomIdfk { get; set; } //selected room from dgv
		public DataGridView ChooseRoomForBookingDgv = new DataGridView();


		public IEnumerable AllBooking()
		{// get all the bookings from the booking table
			using (var context = new SunShine_HotelEntities())
			{
				var bookings = from b in context.Bookings.OrderByDescending(b => b.BookingID)
					select new 
					{
						b.BookingID,
						b.RoomIDFK,
						b.BookingFrom,
						b.BookingTo,
						b.RoomCost

					};
				return bookings.ToList();
			}
		}
		public void NewBooking()
		{   // adding new booking to the booking table
			
			using (var context = new SunShine_HotelEntities())
			{
				var AddBooking = new Booking();
				{
					AddBooking.RoomIDFK = BookingDetailsDTO.roomID;
					AddBooking.BookingFrom = BookingDetailsDTO.BookingFrom;
				    AddBooking.BookingTo = BookingDetailsDTO.BookingTo;
					AddBooking.RoomCost = BookingDetailsDTO.roomCost;
				}
				if (AddBooking.RoomIDFK== 0 && AddBooking.RoomCost == 0)
				{
					MessageBox.Show(" Room No and Room Cost not selected");
				}
				else
				{
				context.Bookings.Add(AddBooking);
			
					MessageBox.Show("Room : " + BookingDetailsDTO.roomID + " " + "has been booked Between  : " 
				 + "\n"  + BookingDetailsDTO.BookingFrom + " - " + BookingDetailsDTO.BookingTo
				  + "\n" + " At total cost of : $" + BookingDetailsDTO.roomCost);

					//save the changes
					context.SaveChanges();
					myguest.NewGuest();
					myBilling.addBilling();
				}
			}
		}
		//get the price of a single room that the person has clicked on to save it to RoomCost in the database from the dictionary
		public decimal RoomBookedPrice()
		{
			decimal cost;
			_roomCostDict.TryGetValue(RoomIdfk, out cost);
			return cost;
		}

		private double DaysBooked
		{
			get
			{
				_daysBooked = (BookingDetailsDTO.BookingTo - BookingDetailsDTO.BookingFrom).TotalDays;
				return _daysBooked;
			}
		}
		private static string RoomCalc(int doublebeds, int Guestcountdown, Room room, int singlebeds, string roomFullMessage)
		{
			AllBeds.GuestCountdown = Guestcountdown;
			if (doublebeds > 0)
			{
				var tuple = AllBeds.BedCalc(doublebeds, room.TariffDoublePerson.Value, "Doublebed");
				roomCost += tuple.Item1;
				doublebeds = tuple.Item2;
			}
			if (singlebeds > 0)
			{
				var tuple = AllBeds.BedCalc(singlebeds, room.TariffSinglePerson.Value, "SingleBed");
				roomCost += tuple.Item1;
				singlebeds = tuple.Item2;
			}

			int extrabeds = 2;
			var tupleExtraBeds = AllBeds.BedCalc(extrabeds, room.TariffExtraPerson.Value, "ExtraBed");
			roomCost += tupleExtraBeds.Item1;
			extrabeds = tupleExtraBeds.Item2;

			return AllBeds.OverflowMessage(roomFullMessage, doublebeds, singlebeds, extrabeds);

		}
		public void ExtractRowCells()
		{
			//an entire row at that index for the free rooms
			var row = ChooseRoomForBookingDgv.Rows[RowIdSelected];
			RoomIdfk = (int)row.Cells[0].Value;
			// FreeRoomCost = (int) row.Cells[1].Value;
			SingleBeds = (int)row.Cells[1].Value;
			DoubleBeds = (int)row.Cells[2].Value;
		}
		///     This returns back a list of the rooms and the prices for those rooms that are available between the dates chosen from the calender
		///     
		/// </summary>
		

	public DataGridView FindRooms()
	{
			if (BookingDetailsDTO.GuestNumbers.Equals(null))
			{
				MessageBox.Show("Please enter the number of Guests");
				return null;
			}
			//get the free rooms
			var allRoomsFree = GetFreeRooms();
			//calculate the cost of each room based on the number of guests

			// a room goes at the cost of 2 people,  a single person is charged as 2 people unless they are in the room with only single beds - bunk room
			//any extra beds in a room are charged with TariffExtraPerson to 2 extra bed limit

			//clear old entries before the foreach loop runs
			_roomCostDict.Clear();
			return GetFreeRoomTariffDetails(allRoomsFree, BookingDetailsDTO.GuestNumbers);
		}

	private DataGridView GetFreeRoomTariffDetails(IEnumerable<int> roomsThatAreFree, int GuestNumbersCountDown)
	{

		List<string> OutputAllRoomCosts = new List<string>();
		List<string> OutputAllRoomOverflow = new List<string>();
		//import the guestnumbers get the tariffs for the rooms
		var roomTariffDetails = Database.RoomClickedDetails(roomsThatAreFree);
		var roomFullMessage = "";
		foreach (Room EachRoom in roomTariffDetails)
			//calculate the cost for each room as it loops through each available room
		{
			roomCost = 0;
			roomFullMessage = ""; //reset to empty
			bool IsTwoOrLessPeople = false;

				var doublebeds = Convert.ToInt32(EachRoom.DoubleBeds); //how many double beds
			var singlebeds = Convert.ToInt32(EachRoom.SingleBeds); //how many single beds 
			//var extrabeds = 2; // only 2 extra beds in a room count down from 2 allowed

			////pass the guest number to a local variable that is decremented and refreshed for each new room
			//if there is one or 2 person in an ordinary room they pay the double rate ie: the price of the room
			if (GuestNumbersCountDown <= 2 && doublebeds > 0)
			{
				roomCost = EachRoom.TariffDoublePerson.Value;
				OutputAllRoomCosts.Add(string.Format("{0:C}", roomCost));
				OutputAllRoomOverflow.Add("No Overflow"); //no overflow
				IsTwoOrLessPeople = true;
					
			}

				//if one person is in a room with no double beds they pay a single rate - its a dorm
				if (GuestNumbersCountDown == 1 && doublebeds == 0)
				{
					roomCost = EachRoom.TariffSinglePerson.Value;
					OutputAllRoomCosts.Add(string.Format("{0:C}", roomCost));
					OutputAllRoomOverflow.Add("No Overflow");  //no overflow
					IsTwoOrLessPeople = true;
				}
				//if we have 2 or more people in a room

				int Guestcountdown = GuestNumbersCountDown;

				if (GuestNumbersCountDown > 1 && IsTwoOrLessPeople == false) //so its 2 or more
				{
					roomFullMessage = RoomCalc(doublebeds, GuestNumbersCountDown, EachRoom, singlebeds, roomFullMessage);

					//final roomcost
					roomCost += Convert.ToDecimal(DaysBooked) * roomCost;

					//add the rooms to a dictionary with the room number and the room cost. This is all becuase I can't add the roomcost to the dgv (grr) so i have to pass it to a list and a dict to hold the data. The dict is used when we add the room cost to the database once the room is selected.
					_roomCostDict.Add(EachRoom.RoomID,roomCost);
					OutputAllRoomCosts.Add(string.Format("{0:C}", roomCost));
					OutputAllRoomOverflow.Add(roomFullMessage);
				}
			}
			DataGridView mydgv = new DataGridView();

			mydgv.DataSource = OutputRoomBookings(OutputAllRoomCosts, OutputAllRoomOverflow).DataSource;




			return mydgv;
		}

		private static DataGridView OutputRoomBookings(List<string> OutputAllRoomCosts, List<string> OutputAllRoomOverflow)
		{        //get how many rows there are to count through
			var rowCount = OutputAllRoomCosts.Count;
			try
			{//get all the data for the free rooms
				var allRoomsFree = GetFreeRooms();
				List<Room> RoomDetails = new List<Room>(Database.GetFreeRoomsAndStuff(allRoomsFree));
				BindingList<RoomStringDTO> AllDetailsOutput = new BindingList<RoomStringDTO>();
				for (var i = 0; i < rowCount; i++)
				{
					RoomStringDTO newListRow = new RoomStringDTO();
					{
						newListRow.RoomID = RoomDetails[i].RoomID.ToString();
						newListRow.RoomCost = OutputAllRoomCosts[i];
						newListRow.SingleBeds = RoomDetails[i].SingleBeds.ToString();
						newListRow.DoubleBeds = RoomDetails[i].DoubleBeds.ToString();
						newListRow.OverflowMsg = OutputAllRoomOverflow[i];
					}
					AllDetailsOutput.Add(newListRow);

				}
				DataGridView mydgv = new DataGridView { DataSource = AllDetailsOutput };


				return mydgv;

			}
			catch (Exception)
			{
				MessageBox.Show("No rooms free ... or DGV not filling");
				return null;
			}
		}

		public static List<int> GetFreeRooms()
		{
			List<int> allRoomsFreeId;
			using (var context = new SunShine_HotelEntities())
			{
				//get all the bookings after today
				var allBookedRooms =
					context.Bookings.Where(b => b.BookingFrom >= DateTime.Today)
						.Select(b => new { b.RoomIDFK, b.BookingFrom, b.BookingTo })
						.OrderBy(b => b.RoomIDFK)
						.ThenBy(b => b.BookingFrom);


				//get all the rooms in total 
				var allRooms = context.Rooms.Select(r => r.RoomID);

				//make a list of all the room numbers (maybe i could leave it as allrooms but whatever)
				allRoomsFreeId = new List<int>(allRooms.ToList());

				//loop through all the booked rooms 
				foreach (var bookedRoom in allBookedRooms)
				{
					//find if the booking dates are inside the room dates and remove the rooms from the list if they conflict
					if (DoTheDatesOverlap(BookingDetailsDTO.BookingFrom, BookingDetailsDTO.BookingTo, bookedRoom.BookingFrom, bookedRoom.BookingTo))
					{
						if (allRoomsFreeId.Contains(Convert.ToInt32(bookedRoom.RoomIDFK)))
						{
							//remove rooms from the list of all rooms that are in conflict
							allRoomsFreeId.Remove(Convert.ToInt32(bookedRoom.RoomIDFK));
						}
					}
				}
			}
			return allRoomsFreeId;
		}

		private static bool DoTheDatesOverlap(DateTime checkIn, DateTime checkOut, DateTime? bookingFrom, DateTime? bookingTo)
		{
			return (checkOut >= bookingFrom) && (bookingTo >= checkIn);
		}

		public int LastBookingId()
		{
			using (var context = new SunShine_HotelEntities())
			{
				//get all the bookings after today
				var lastBookedRooms = context.Bookings.Select(b => b.BookingID).LastOrDefault();

				return lastBookedRooms;
			}
		}
		//public void LoadBookings()
		//{
		//	ChooseRoomForBookingDgv = Database.ListAllBookingsAfterToday();
			
		//}
		//public void BookRoomClick()
		//{
		//	//Book rooms between dates
		//	_isRoomConflict = false; //set the conflict 
		//							 //get all the dates for that room to compare them with the dates entered
		//	using (var context = new SunShine_HotelEntities())
		//	{
		//		var roomBookingConflict =
		//			context.Bookings.Where(b => b.RoomIDFK == RoomIdfk && b.BookingFrom >= DateTime.Today.Date)
		//				.Select(b => new { b.BookingFrom, b.BookingTo })
		//				.ToList();

		//		foreach (var bdate in roomBookingConflict)
		//		{
		//			//if the new booked start date is bigger than the existing start date and less than the existing end date or the new booked end date is bigger than existing start date and before the existing end date - conflict!
		//			if (BookingDetailsDTO.BookingTo.Date >= bdate.BookingFrom && BookingDetailsDTO.BookingFrom.Date < bdate.BookingTo ||
		//				BookingDetailsDTO.BookingTo.Date >= bdate.BookingFrom && BookingDetailsDTO.BookingTo.Date < bdate.BookingTo)
		//			{
		//				//throw a message
		//				MessageBox.Show("The room is already booked between " + BookingDetailsDTO.BookingFrom.Date + " and " +
		//								BookingDetailsDTO.BookingTo.Date);

		//				//exit out if there is a conflict
		//				_isRoomConflict = true;
		//				return;
		//			}
		//		} //end the foreach

		//		//add new booking if there is no conflict
		//		if (_isRoomConflict == false)
		//		{
		//			NewBooking();
		//			//must book the room before you book the guest
		//			//_allGuests.AddGuest();
		//		}
		//	}
		//}
		public void deleteBooking(int bookingId)
	    {
			using (var context = new SunShine_HotelEntities())
			{
				var delete = context.Bookings.FirstOrDefault(b => b.BookingID == bookingId);


				context.Bookings.Remove(delete);
				context.SaveChanges();
				MessageBox.Show(" Selected booking has been deleted successfully ....");
				

			}
		}
}
}
