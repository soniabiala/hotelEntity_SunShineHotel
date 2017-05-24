using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SunShine_Hotel;
using SunShine_Hotel.Data_Layer;

namespace CSharp_Entity_Hotel_Assessment_2013.BusinessLayer.RoomBookingClasses
{

	/// <summary>
	///  TODO  NEED TO FINISH OFF THE BOOKING PART OF THE PROGRAM
	/// </summary>
	public class RoomBooking
	{
		private readonly Dictionary<int, decimal> _roomCostDict = new Dictionary<int, decimal>();
		//private AllGuests _allGuests;
		//   private int _bookingId { get; set; }
		private double _daysBooked { get; set; }
		private bool _isRoomConflict { get; set; }

		public string BookedConfirmationMessage { get; set; }
		public DataGridView CostForBookingDgv = new DataGridView();
		public int GuestNumbers { get; set; }
		public int SingleBeds { get; set; }
		public int DoubleBeds { get; set; }


		private readonly Database myModelCalls = new Database();
		public List<string> OutputAllRoomCosts = new List<string>();
		private List<string> OutputAllRoomOverflow = new List<string>();
		public decimal roomCost { get; set; }
		public int RowIdSelected { get; set; }


		public int RoomIdfk { get; set; } //selected room from dgv
		public DateTime BookingFrom { get; set; }
		public DateTime BookingTo { get; set; }

		public DataGridView ChooseRoomForBookingDgv { get; set; }

		private double DaysBooked
		{
			get
			{
				_daysBooked = (BookingTo - BookingFrom).TotalDays;
				return _daysBooked;
			}
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


		/// <summary>
		///     Adds a new booking if there isn't a conflict
		///     this runs from the BookRoomClick method above if there is no date conflict
		/// </summary>
		public void AddNewBooking()
		{
			//make a new instance and pass that to the class
			var myRB = new Booking()
			{
				RoomIDFK = RoomIdfk,
				BookingFrom = BookingFrom,
				BookingTo = BookingTo,
				RoomCost = RoomBookedPrice()
			};
			if (myRB.RoomIDFK != 0 && myRB.RoomCost != 0)
			{
				using (var context = new SunShine_HotelEntities())
				{
					context.Bookings.Add(myRB);
				   context.SaveChanges();
				}
				
			}
			else
			{
				MessageBox.Show(myRB.RoomIDFK + " RoomIDFK is null " + myRB.RoomCost + " Roomcost is null");
			}
		}

		//get the price of a single room that the person has clicked on to save it to RoomCost in the database from the dictionary
		public decimal RoomBookedPrice()
		{
			decimal cost;
			_roomCostDict.TryGetValue(RoomIdfk, out cost);
			return cost;
		}

		/// <summary>
		///     This returns back a list of the rooms and the prices for those rooms that are available between the dates chosen from the calender
		///     
		/// </summary>
		public void FindRoomFromDates()
		{
			if (GuestNumbers.Equals(null))
			{
				MessageBox.Show("Please enter the number of Guests");
				return;
			}

			//get the free rooms
			var allRoomsFree = GetFreeRooms();

			//get the free rooms for the DGV to show

			ChooseRoomForBookingDgv.DataSource = myModelCalls.GetFreeRoomsAndStuff(allRoomsFree).DataSource;
			//calculate the cost of each room based on the number of guests

			// a room goes at the cost of 2 people,  a single person is charged as 2 people unless they are in the room with only single beds - bunk room
			//any extra beds in a room are charged with TariffExtraPerson to 2 extra bed limit

			//clear old entries before the foreach loop runs
			OutputAllRoomCosts.Clear();
			_roomCostDict.Clear();

			//import the guestnumbers get the tariffs for the rooms
			var roomTariffDetails = myModelCalls.RoomClickedDetails(allRoomsFree);
			//calculate the cost for each room as it loops through each available room
			foreach (var room in roomTariffDetails)
			{
				roomCost = 0;
				var roomFullMessage = ""; //if there are more than 2 extra beds then the room is full

				var doublebeds = Convert.ToInt32(room.TariffDoublePerson); //how many double beds
				var singlebeds = Convert.ToInt32(room.TariffSinglePerson); //how many single beds 
				var extrabeds = 2; // only 2 extra beds in a room count down from 2 allowed

				////pass the guest number to a local variable that is decremented and refreshed for each new room
				var guestNumbersForForeach = GuestNumbers;

				if (guestNumbersForForeach <= 2 && doublebeds > 0)
				{
					//if there is one or 2 person in an ordinay room they pay the double rate ie: the price of the room
					roomCost = room.TariffDoublePerson.Value;
					break;
				}

				if (guestNumbersForForeach == 1 && doublebeds == 0)
				{
					//if one person is in a room with no double beds they pay a single rate - its a dorm
					roomCost = room.TariffSinglePerson.Value;
					break;
				}

				int Guestcountdown = guestNumbersForForeach;

				if (guestNumbersForForeach > 1) //so its 2 or more
				{
					roomFullMessage = FillBedsForEachRoom(doublebeds, Guestcountdown, room, singlebeds,
						extrabeds, roomFullMessage);

					roomCost += Convert.ToDecimal(DaysBooked) * roomCost;

					//add the rooms to a dictionary with the room number and the room cost. This is all becuase I can't add the roomcost to the dgv (grr) so i have to pass it to a list and a dict to hold the data. The dict is used when we add the room cost to the database once the room is selected.
					_roomCostDict.Add(room.RoomID, roomCost);

					//   var text = room.RoomID + ": " + string.Format("{0:C}", _roomCost) + " " +RoomFullMessage;

					var text = string.Format("{0:C}", roomCost); // + " " + RoomFullMessage;

					//output the data to a listview to show on the form
					OutputAllRoomCosts.Add(text);
					OutputAllRoomOverflow.Add(roomFullMessage);
					// CostForBooking.Rows.Add(text);
				}

				OutputToDgvWithList();

			}
		}

		private string FillBedsForEachRoom(int doublebeds, int Guestcountdown, Room room, int singlebeds, int extrabeds, string roomFullMessage)
		{
			roomCost = AllBeds.BedCalc(ref doublebeds, ref Guestcountdown, room.TariffDoublePerson.Value, "Doublebed");

			roomCost += AllBeds.BedCalc(ref singlebeds, ref Guestcountdown, room.TariffSinglePerson.Value, "SingleBed");

			roomCost += AllBeds.BedCalc(ref extrabeds, ref Guestcountdown, room.TariffExtraPerson.Value, "ExtraBed");

			return AllBeds.OverflowMessage(extrabeds, roomFullMessage, Guestcountdown, doublebeds, singlebeds);

			//do //loop through the options until the guest number = 0
			//{
			//you need a spare bed AND a guest
			//while (doublebeds > 0) //only ever 1 double bed, we are not a place for kinky stuff
			//{
			//    //counts down guests
			//    if (guestNumbersForForeach == 0) {
			//        //probably never triggered as 2 people and one double bed is covered above
			//        break;
			//    }
			//    //if the room has double beds count through them
			//    roomCost = room.Tariff2People.Value; // increase the room cost by amount
			//    guestNumbersForForeach -= 2; //reduce guest numbers by 2
			//    doublebeds -= 1; //remove bed from the list
			//}

			//while (singlebeds > 0) {
			//    //counts down guests
			//    if (guestNumbersForForeach == 0) {
			//        break;
			//    }
			//    //if no doubles then count through single beds
			//    roomCost += room.TariffSinglePerson.Value; // $150
			//    guestNumbersForForeach -= 1;
			//    singlebeds -= 1;
			//}

			//while (extrabeds > 0) {
			//    //if no double or single beds left then count through extra beds until there are 2 end if the guests run out
			//    if (guestNumbersForForeach == 0) {
			//        break;
			//    }
			//    roomCost += room.TariffExtraPerson.Value; // $20
			//    extrabeds -= 1;
			//    guestNumbersForForeach -= 1;
			//}
			//message to show how many extra beds used.




			switch (extrabeds)
			{
				case 0:
					roomFullMessage += "Inc 2 Extra beds. ";
					break;
				case 1:
					roomFullMessage += "Inc 1 Extra bed. ";
					break;
				default:
					roomFullMessage += "";
					break;
			}
			//overflow message
			if (Guestcountdown >= 1 && doublebeds == 0 && singlebeds == 0 && extrabeds == 0)
			{
				roomFullMessage += Guestcountdown + " Guests must be in another room.";
				//  break;
			}
			//overflow message
			if (Guestcountdown == 0) // && doublebeds == 0 && singlebeds == 0 && extrabeds == 0) {
			{
				roomFullMessage += " No Overflow ";
				//  break;
			}

			return roomFullMessage;
		}

		private void OutputToDgvWithList()
		{
			// RunOnceOnly = true;
			CostForBookingDgv.ColumnCount = 5;
			CostForBookingDgv.Columns[0].Name = "Room";
			CostForBookingDgv.Columns[1].Name = "Cost";
			CostForBookingDgv.Columns[2].Name = "Single Bed";
			CostForBookingDgv.Columns[3].Name = "Double Bed";
			CostForBookingDgv.Columns[4].Name = "Overflow";

			var rowCount = OutputAllRoomCosts.Count; //outside becuase it keeps wanting to change
			CostForBookingDgv.Rows.Clear();
			// add all list entries to a dgv  OutputAllRoomCosts.Count

			try
			{
				for (var i = 0; i < rowCount; i++)
				{
					//string array holds all the cells
					string[] newRow =
					{
						ChooseRoomForBookingDgv.Rows[i].Cells[0].Value.ToString(), OutputAllRoomCosts[i],
						ChooseRoomForBookingDgv.Rows[i].Cells[1].Value.ToString(),
						ChooseRoomForBookingDgv.Rows[i].Cells[2].Value.ToString(), OutputAllRoomOverflow[i]
					};

					CostForBookingDgv.Rows.Add(newRow);
				}
			}
			catch (Exception)
			{
				MessageBox.Show("No rooms free ... maybe");
			}
		}

		/// <summary>
		///     Get the rooms that are free
		/// </summary>
		/// <returns>List of the free rooms</returns>
		public List<int> GetFreeRooms()
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
					if (DoTheDatesOverlap(BookingFrom, BookingTo, bookedRoom.BookingFrom, bookedRoom.BookingTo))
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

		/// <summary>
		///     Return a bool if the room dates overlap or not
		/// </summary>
		private bool DoTheDatesOverlap(DateTime start1, DateTime end1, DateTime? start2, DateTime? end2)
		{
			return (end1 >= start2) && (end2 >= start1);
		}

		#region "Not used"

		/// <summary>
		///     Get the last Booking ID
		/// </summary>
		/// <returns></returns>
		public int LastBookingId()
		{
			using (var context = new SunShine_HotelEntities())
			{
				//get all the bookings after today
				var lastBookedRooms = context.Bookings.Select(b => b.BookingID).LastOrDefault();

				return lastBookedRooms;
			}
		}


		public void LoadBookings()
		{
			//list all bookings after today
			//    using (var context = new Sunshine_HotelEntities())
			//    {
			//        var allBookings =
			//            context.Bookings.Where(b => b.BookingFrom >= DateTime.Today.Date)
			//                .Select(b => new {b.RoomIDFK, b.BookingFrom, b.BookingTo})
			//                .OrderBy(b => b.RoomIDFK)
			//                .ThenBy(b => b.BookingFrom);

			//        ChooseRoomForBookingDgv.DataSource = allBookings.ToList();

			//}

			ChooseRoomForBookingDgv = myModelCalls.ListAllBookingsAfterToday();
			//    _runOnceOnly = false;
			//list free rooms to combobox
		}

		/// <summary>
		///     Checks to see if the room is free before booking it
		///     IS THIS NECESSARY? NOT USED
		/// </summary>
		public void BookRoomClick()
		{
			//Book rooms between dates
			_isRoomConflict = false; //set the conflict 
									 //get all the dates for that room to compare them with the dates entered
			using (var context = new SunShine_HotelEntities())
			{
				var roomBookingConflict =
					context.Bookings.Where(b => b.RoomIDFK == RoomIdfk && b.BookingFrom >= DateTime.Today.Date)
						.Select(b => new { b.BookingFrom, b.BookingTo })
						.ToList();

				foreach (var bdate in roomBookingConflict)
				{
					//if the new booked start date is bigger than the existing start date and less than the existing end date or the new booked end date is bigger than existing start date and before the existing end date - conflict!
					if (BookingFrom.Date >= bdate.BookingFrom && BookingFrom.Date < bdate.BookingTo ||
						BookingTo.Date >= bdate.BookingFrom && BookingTo.Date < bdate.BookingTo)
					{
						//throw a message
						MessageBox.Show("The room is already booked between " + BookingFrom.Date + " and " +
										BookingTo.Date);

						//exit out if there is a conflict
						_isRoomConflict = true;
						return;
					}
				} //end the foreach

				//add new booking if there is no conflict
				if (_isRoomConflict == false)
				{
					AddNewBooking();
					//must book the room before you book the guest
					//_allGuests.AddGuest();
				}
			}
		}

		#endregion
	}
}