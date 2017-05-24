//using SunShine_Hotel.Data_Layer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using SunShine_Hotel.Businees;
using SunShine_Hotel.Businees_Layer;
using System.Collections;
using SunShine_Hotel.Data;

namespace SunShine_Hotel
{
	public partial class Form1 : Form
	{  //create an instance of the database class
	
		Guests myGuests = new Guests();
	//	Database myDatabase = new Database();
		Rooms myRoom = new Rooms();
		Billings myBilling = new Billings();
		Bookings mybooking = new Bookings();

		int guestID, roomID, bookingID, billingID;
		//public  DateTime bookingFrom, bookingTo;
		string RoomID= "";
	
		private string TabIndex = "";
		
		public Form1()
		{
			InitializeComponent();
		}

		
		private void Form1_Load(object sender, EventArgs e)
		{
			FormReset();
			DGVAvailableRooms.DataSource = null;

		}
		
		

		
		private void Change_CheckOutDate()
		{
			DateTime checkIN = Convert.ToDateTime(DTPCheckIn.Text);
			DateTime checkOut = checkIN.AddDays(1);
			
			DTPCheckOut.Text = checkOut.ToString();
		}
		//private void Change_CheckInDate()
		//{
		//	DateTime checkOut = Convert.ToDateTime(DTPCheckOut.Text);
		//	DateTime checkIn = checkOut.AddDays(-1);

		//	DTPCheckIn.Text = checkIn.ToString();
		//}
		private void FormReset()
		{
			LoadAllDGV();
			Change_CheckOutDate();
			txtBarCharges.Text = "0.00";
			txtWiFiCharges.Text = "0.00";
			txtPhonecharges.Text = "0.00";
			txtRoomcharges.Text = "0.00";
			txtTotal.Text = "0.00";
			txtName.Text = "";
			txtAddress.Text = "";
			txtPhoneNo.Text = "";
			txtNumOfGuests.Text = "";

		}
		private void btnReservation_Click(object sender, EventArgs e)
		{ 
			//check for missing info
			if (txtName.Text != String.Empty && txtAddress.Text != String.Empty && txtPhoneNo.Text != String.Empty && txtNumOfGuests.Text!= String.Empty)
			{ //adding a new booking
				AddNewBooking();
			}
			else
			{
				MessageBox.Show("Fill the empty Fields with Guest details !!!!");
			}
			
		}

		private void AddNewBooking()
		{
			BookingDetailsDTO.BookingFrom = Convert.ToDateTime(DTPCheckIn.Value).Date;
			BookingDetailsDTO.BookingTo = Convert.ToDateTime(DTPCheckOut.Value).Date;
			BookingDetailsDTO.guestName = txtName.Text;
			BookingDetailsDTO.guestAddress = txtAddress.Text;
			BookingDetailsDTO.GuestPhoneNo = Convert.ToInt32(txtPhoneNo.Text);
			mybooking.NewBooking();
			FormReset();

		}

		private void LoadAllDGV()
		{//load all the data grid views with data from tables
			DGVGuests.DataSource = Database.Load_DGVGuests().DataSource;
			DGVGuests.Columns[0].Visible = false;
			DGVGuests.Columns[1].Visible = false;
			DGVGuests.Columns[10].Visible = false;
			DGVBookings.DataSource = Database.Load_DGVBookings().DataSource;
			DGVBookings.Columns[0].Visible = false;
			DGVRooms.DataSource = Database.Load_DGVRooms().DataSource;
			
			DGVBilling.DataSource = Database.Load_DGVBilling().DataSource;
			DGVBilling.Columns[0].Visible = false;
			//DGVBilling.Columns[1].Visible = false;
			GBRoom.Visible = false;
			GBGuests.Visible = true;

		}

		private void btnCheckIn_Click(object sender, EventArgs e)
		{// check if the guest is on time for his check In
			if (guestID == 0)
			{
				MessageBox.Show("Select the guest to Check In..");
			}
			else
			{
				DateTime checkinDate  = myGuests.checkInDate(bookingID);
				if (checkinDate.Date > DateTime.Today.Date)
			    {
				MessageBox.Show("Too early for check in, please come back again on " + checkinDate.Date);
			    }

		       else	if (checkinDate.Date < DateTime.Today.Date)
		  	   {
				MessageBox.Show("You are late for you check in, you were meant to show up on " + checkinDate.Date);

			   }
			    else 
			    {
				var guestCheckIn = new Guests();
				guestCheckIn.CheckIn(bookingID);
				MessageBox.Show("All Good .... Guest has been checked in.");
				DGVGuests.DataSource = Database.Load_DGVGuests().DataSource;
					FormReset();
				}
			}
		}

		private void btnUpdateGuest_Click(object sender, EventArgs e)
		{
			if (guestID == 0)
			{
				MessageBox.Show("Select the Guest to Update !!!");
			}
			
			else
			{
				BookingDetailsDTO.guestName = txtName.Text;
				BookingDetailsDTO.guestAddress = txtAddress.Text;
				BookingDetailsDTO.GuestPhoneNo = Convert.ToInt32(txtPhoneNo.Text);
				BookingDetailsDTO.GuestNumbers = Convert.ToInt32(txtNumOfGuests.Text);
				myGuests.updateGuest(guestID );
				MessageBox.Show("Details for" +" " +txtName.Text.ToUpper() + " "+ "Has been updated !!!");
			}

			DGVGuests.DataSource = Database.Load_DGVGuests().DataSource;
			FormReset();
		}


		private void DGVGuests_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			
			guestID = Convert.ToInt32(DGVGuests.Rows[e.RowIndex].Cells[0].Value.ToString());
			bookingID = Convert.ToInt32(DGVGuests.Rows[e.RowIndex].Cells[1].Value.ToString());
			billingID = Convert.ToInt32(DGVGuests.Rows[e.RowIndex].Cells[10].Value.ToString());
			txtTotal.Text = "";
			
			
			txtName.Text = DGVGuests.Rows[e.RowIndex].Cells[2].Value.ToString();
			txtAddress.Text = DGVGuests.Rows[e.RowIndex].Cells[3].Value.ToString();
			txtPhoneNo.Text = DGVGuests.Rows[e.RowIndex].Cells[4].Value.ToString();
			txtNumOfGuests.Text = DGVGuests.Rows[e.RowIndex].Cells[5].Value.ToString();

			//load expenses details to  group box expenses
			txtBarCharges.Text = DGVGuests.Rows[e.RowIndex].Cells[11].Value.ToString();
			txtPhonecharges.Text = DGVGuests.Rows[e.RowIndex].Cells[12].Value.ToString();
			txtRoomcharges.Text = DGVGuests.Rows[e.RowIndex].Cells[13].Value.ToString();
			txtWiFiCharges.Text = DGVGuests.Rows[e.RowIndex].Cells[14].Value.ToString();

		}
		

		private void btnDeleteGuest_Click(object sender, EventArgs e)
		{ //delete the guest using guest ID

			if (guestID != 0)
			{
				
				myGuests.DeleteGuest(guestID);
			}
			else
			{
				MessageBox.Show("Select the Guest to delete !!!");
			}

			DGVGuests.DataSource = Database.Load_DGVGuests().DataSource;
			FormReset();
		}

		private void DTPCheckIn_ValueChanged(object sender, EventArgs e)
		{
			DateTime checkIN = Convert.ToDateTime(DTPCheckIn.Text);
			if (checkIN < DateTime.Today)
			{// check for a valid date . Can not book a book for date before today  

				MessageBox.Show("Check In Date is not Correct");
				DTPCheckIn.Text = DateTime.Today.ToString();
			}
			else
			{
			Change_CheckOutDate();
			//DGVAvailableRooms.DataSource = mybooking.FindRooms().DataSource;
				
			}

		}

		// add a new room 
		private void btnAddRoom_Click(object sender, EventArgs e)
		{
			//check for missing info
			if (txtSingleBed.Text != String.Empty && txtDoubleBed.Text != String.Empty && txtTariffSingle.Text != String.Empty && txtTariffDouble.Text != String.Empty && txtTarifExtra.Text != String.Empty)
			{
				//adding a new room to the database
				int singleBed = Convert.ToInt32(txtSingleBed.Text);
				int doubleBed = Convert.ToInt32(txtDoubleBed.Text);
				decimal tariffSingle = Convert.ToDecimal(txtTariffSingle.Text);
				decimal tariffDouble = Convert.ToDecimal(txtTariffDouble.Text);
				decimal tariffExtra = Convert.ToDecimal(txtTarifExtra.Text);

				myRoom.addRoom(singleBed, doubleBed, tariffSingle, tariffDouble, tariffExtra);

				MessageBox.Show("New Room has been added");

				LoadAllDGV();
				GBRoom.Visible = true;
				txtSingleBed.Text = "";
				txtDoubleBed.Text = "";
				txtTariffSingle.Text = "";
				txtTariffDouble.Text = "";
				txtTarifExtra.Text = "";
			}
			else
			{
				MessageBox.Show("Fill the empty Fields !!");
			}
			//update DGV Room with the new booking
			DGVRooms.DataSource = Database.Load_DGVRooms().DataSource;
		}

		private void DGVBookings_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{ //Get the booking ID
			bookingID = Convert.ToInt32(DGVBookings.Rows[e.RowIndex].Cells[0].Value.ToString());
		}

		//find the available room according to selected dates
		private void btnCheck_Click(object sender, EventArgs e)
		{
			AvailableRooms();
		}

		private void AvailableRooms()
		{
			//find available rooms for selected date
			BookingDetailsDTO.BookingFrom = Convert.ToDateTime(DTPCheckIn.Value);
			BookingDetailsDTO.BookingTo = Convert.ToDateTime(DTPCheckOut.Value);
			try
			{
				if (txtNumOfGuests.Text != "")
				{
					BookingDetailsDTO.GuestNumbers = Convert.ToInt32(txtNumOfGuests.Text);

				}
			}
			catch (Exception)
			{
				MessageBox.Show("Enter no of guests for stay...");

			}

			//DGVRooms.DataSource = Database.Load_DGVRooms().DataSource;
			mybooking.FindRooms();
			DGVAvailableRooms.Visible = true;
			//load DGV with available rooms
			DGVAvailableRooms.DataSource = mybooking.FindRooms().DataSource;
		}

		//update guest expenses if there is any changes
		private void btnUpdateExpenses_Click(object sender, EventArgs e)
		{
			if (guestID != 0)
			{
				decimal barCharge = Convert.ToDecimal(txtBarCharges.Text);
				decimal wifiCharge = Convert.ToDecimal(txtWiFiCharges.Text);
				decimal phonecharge = Convert.ToDecimal(txtPhonecharges.Text);
				decimal roomcharge = Convert.ToDecimal(txtRoomcharges.Text);
				myBilling.updateBilling(billingID, barCharge, phonecharge, roomcharge, wifiCharge);
				decimal total = barCharge + wifiCharge + phonecharge + roomcharge;
				txtTotal.Text = total.ToString();
				DGVBilling.DataSource = Database.Load_DGVBilling().DataSource;
				DGVGuests.DataSource = Database.Load_DGVGuests().DataSource;
			}
			else
			{
				MessageBox.Show("Select the guest to make any changes !!");
			}
			FormReset();
		}

		

		private void DGVBilling_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{// get the billingID 

			//billingID = Convert.ToInt32(DGVBilling.Rows[e.RowIndex].Cells[0].Value.ToString());
		}

		//display selected room info to txt boxes for update
		private void DGVRooms_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			roomID = Convert.ToInt32(DGVRooms.Rows[e.RowIndex].Cells[0].Value.ToString());
			roomNo.Text = DGVRooms.Rows[e.RowIndex].Cells[0].Value.ToString();
			txtSingleBed.Text = DGVRooms.Rows[e.RowIndex].Cells[1].Value.ToString();
			txtDoubleBed.Text = DGVRooms.Rows[e.RowIndex].Cells[2].Value.ToString();

			txtTariffSingle.Text = DGVRooms.Rows[e.RowIndex].Cells[3].Value.ToString();
			txtTariffDouble.Text = DGVRooms.Rows[e.RowIndex].Cells[4].Value.ToString();
			txtTarifExtra.Text = DGVRooms.Rows[e.RowIndex].Cells[5].Value.ToString();
		}

		//method to delete a room
		private void btnDeleteRoom_Click(object sender, EventArgs e)
		{

			if (roomID != 0)
			{
			
				myRoom.DeleteRoom(roomID);
			}
			else
			{
				MessageBox.Show("Select the Room to delete !!!");
			}

			DGVRooms.DataSource = Database.Load_DGVRooms().DataSource;
		}

		private void btnAllRooms_Click(object sender, EventArgs e)
		{
			DGVRooms.Visible = true;
			//LBRooms.Visible = false;
			DGVAvailableRooms.Visible = false;
		}

		private void btnAvailableRooms_Click(object sender, EventArgs e)
		{
			DGVRooms.Visible = false;
			DGVAvailableRooms.Visible = true;
			AvailableRooms();
		}

		private void btnDeleteBilling_Click(object sender, EventArgs e)
		{
			if (billingID == 0)
			{
				MessageBox.Show("Select the billing from the list to delete..");
			}
			else
			{
				myBilling.deleteBilling(billingID);
			}
			DGVBilling.DataSource = Database.Load_DGVBilling().DataSource;
		}

		private void btnDeleteBooking_Click(object sender, EventArgs e)
		{
			if (bookingID != 0)
			{
				mybooking.deleteBooking(bookingID);
			}
			else
			{
				MessageBox.Show("Select the booking from the list to delete..");
			}
			DGVBookings.DataSource = Database.Load_DGVBookings().DataSource;
		}

		private void btnCheckInGuest_Click(object sender, EventArgs e)
		{
			DGVGuests.DataSource = myGuests.GuestsWithoutCheckIN();
		}

		private void btnCheckOutGuest_Click(object sender, EventArgs e)
		{
			DGVGuests.DataSource = myGuests.GuestsWithoutCheckOut();
			DGVGuests.Columns[0].Visible = false;
			DGVGuests.Columns[1].Visible = false;
			DGVGuests.Columns[10].Visible = false;
		}

		private void btnAllGuests_Click(object sender, EventArgs e)
		{
			DGVGuests.DataSource = Database.Load_DGVGuests().DataSource;
			DGVGuests.Columns[0].Visible = false;
			DGVGuests.Columns[1].Visible = false;
			DGVGuests.Columns[10].Visible = false;
		}

		private void btnCheckOut_Click(object sender, EventArgs e)
		{
			// Reset the bool to false
			
			if (guestID != 0)
			{
				decimal barCharge = Convert.ToDecimal(txtBarCharges.Text);
				decimal wifiCharge = Convert.ToDecimal(txtWiFiCharges.Text);
				decimal phonecharge = Convert.ToDecimal(txtPhonecharges.Text);
				decimal roomcharge = Convert.ToDecimal(txtRoomcharges.Text);

				decimal total = barCharge + wifiCharge + phonecharge + roomcharge;
				
				myGuests.GuestsCheckOut(guestID,total);
				DGVGuests.DataSource = Database.Load_DGVGuests().DataSource;
				FormReset();

			}
			else
			{
				MessageBox.Show("Select the guest for check out ..");
			}
		}

		

		private void DTPCheckOut_ValueChanged(object sender, EventArgs e)
		{
			DateTime checkOut = Convert.ToDateTime(DTPCheckOut.Text);
			DateTime checkIn = Convert.ToDateTime(DTPCheckIn.Text);
			if (checkOut < DateTime.Today)
			{
				MessageBox.Show("Check Out Date is not Correct");
				DTPCheckOut.Text = DateTime.Today.ToString();
			}
			else if (checkOut < checkIn)
			{
				MessageBox.Show("Check Out Date is not Correct");
				DTPCheckOut.Text = checkIn.AddDays(1).ToString();
			}
			//else
			//{
			//	DGVAvailableRooms.DataSource = mybooking.FindRooms().DataSource;
					
			//}
			
		}

		private void DGVAvailableRooms_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			BookingDetailsDTO.roomID = Convert.ToInt32(DGVAvailableRooms.Rows[e.RowIndex].Cells[0].Value.ToString());
			BookingDetailsDTO.roomCost = RemoveDollarSign(DGVAvailableRooms.Rows[e.RowIndex].Cells[3].Value.ToString());
		}
		private Decimal RemoveDollarSign(string moneytext)
		{
			return Convert.ToDecimal(moneytext.Replace("$", ""));
		}

		private void GBRoom_Enter(object sender, EventArgs e)
		{

		}

	

		private void TabControl_Selected(object sender, TabControlEventArgs e)
		{
			TabIndex = e.TabPageIndex.ToString();
			if (e.TabPage == guests)
			{ 
				GBGuests.Visible = true;
				GBRoom.Visible = false;

			}
			else if (e.TabPage == Rooms)
			{
				GBGuests.Visible = false;
				GBRoom.Visible = true;
				DGVRooms.Visible = false;
				//LBRooms.Visible = true;
				DGVAvailableRooms.Visible = true;

			}
		}

		private void btnExpenses_Click(object sender, EventArgs e)
		{

			if (guestID !=0)
			{


				decimal barCharge = Convert.ToDecimal(txtBarCharges.Text);
				decimal wifiCharge = Convert.ToDecimal(txtWiFiCharges.Text);
				decimal phonecharge = Convert.ToDecimal(txtPhonecharges.Text);
				decimal roomcharge = Convert.ToDecimal(txtRoomcharges.Text);
				
				decimal total = barCharge + wifiCharge + phonecharge + roomcharge;

				txtTotal.Text = total.ToString();
				//myBilling.addCharges(guestID, barCharge, phonecharge, roomcharge, wifiCharge);
				DGVBilling.DataSource = Database.Load_DGVBilling().DataSource;
			}

			else
			{

				MessageBox.Show("Select the guest for total expenses !!!" );
				
			}
          DGVBilling.DataSource = Database.Load_DGVBilling().DataSource;

		}

		private void btnUpdateRoom_Click(object sender, EventArgs e)
		{

			if (roomID != 0)
			{
				myRoom.updateRoom(roomID, Convert.ToInt32(txtSingleBed.Text), Convert.ToInt32(txtDoubleBed.Text), Convert.ToDecimal(txtTariffSingle.Text), Convert.ToDecimal(txtTariffDouble.Text), Convert.ToDecimal(txtTarifExtra.Text));
				MessageBox.Show("Room" + " " + roomID + " " + "Has been updated !!!");
			}
			else
			{
				MessageBox.Show("Select the Room to Update !!!");
			}

			DGVRooms.DataSource = Database.Load_DGVRooms().DataSource;
			txtSingleBed.Text = "";
			txtDoubleBed.Text = "";

			txtTariffSingle.Text = "";
			txtTariffDouble.Text = "";
			txtTarifExtra.Text = "";
		}

		private void txtPhoneNo_TextChanged(object sender, EventArgs e)
		{
			if (System.Text.RegularExpressions.Regex.IsMatch(txtPhoneNo.Text, "[^0-9]"))
			{
				MessageBox.Show("Please enter only numbers.");
				txtPhoneNo.Text = txtPhoneNo.Text.Remove(txtPhoneNo.Text.Length - 1);
				txtPhoneNo.Focus();
			}

		}

		private void GuestNo_TextChanged(object sender, EventArgs e)
		{
			if (System.Text.RegularExpressions.Regex.IsMatch(txtNumOfGuests.Text, "[^1-9]"))
			{
				MessageBox.Show("Please enter valid No of Guests only.");
				txtNumOfGuests.Text = txtNumOfGuests.Text.Remove(txtNumOfGuests.Text.Length - 1);
				txtNumOfGuests.Focus();
			}
		}
	}
}
