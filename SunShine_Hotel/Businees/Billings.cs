using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SunShine_Hotel.Businees
{
	class Billings
	{
		public void addCharges(int gId, decimal barCharge, decimal phonecharge, decimal roomcharge, decimal wifiCharge)
		{
			using (var context = new SunShine_HotelEntities())
			{
				if ((from b in context.Billings where b.GuestIDFK == gId select  b).Count()>0)
				{
					MessageBox.Show("Expenses already added for this guest....");
				}
				else
				{
				var AddBilling = new Billing();

				AddBilling.BarCharge = barCharge;
				AddBilling.GuestIDFK = gId;
				AddBilling.TeleChrage = phonecharge;
				AddBilling.WiFiCharges = wifiCharge;
				AddBilling.RoomCharge = roomcharge;
				context.Billings.Add(AddBilling);
				context.SaveChanges();	
				}

				
			}
		}

		public void updateBilling(int billingID, decimal barCharge, decimal phonecharge, decimal roomcharge, decimal wifiCharge)
		{
			using (var context = new SunShine_HotelEntities())
			{
				var update = context.Billings.FirstOrDefault(b => b.BillingID == billingID);
				update.BarCharge = barCharge;
				update.WiFiCharges = wifiCharge;
				update.TeleChrage = phonecharge;
				update.RoomCharge = roomcharge;
				context.SaveChanges();
				MessageBox.Show(" Expenses has been updated for the selected guest !!");
			}
		}

		public IEnumerable getBillingDetail(int gID)
		{
			using (var context = new SunShine_HotelEntities())
			{
				var billingINFO = from b in context.Billings
								   where b.GuestIDFK == gID
								   select new
								   {
									  
									   b.BarCharge,
									   b.TeleChrage,
									   b.RoomCharge,
									   b.WiFiCharges
								   };
				return billingINFO.ToList();
			}
		}

		public void deleteBilling(int billingId)
		{
			using (var context = new SunShine_HotelEntities())
			{
				var delete = context.Billings.FirstOrDefault(b => b.BillingID == billingId);


				context.Billings.Remove(delete);
				context.SaveChanges();
				MessageBox.Show("Selected billing has been deleted successfully ....");
				

			}
		}

		//adding the room charges to billing
		public void addBilling()
		{
			using (var context = new SunShine_HotelEntities())
			{
				var guestId =
				Convert.ToInt16(
					context.Guests.OrderByDescending(g => g.GuestID).Select(c => c.GuestID).First());
				var newBilling = new Billing();
				newBilling.GuestIDFK = guestId;
				newBilling.RoomCharge = BookingDetailsDTO.roomCost;
				newBilling.BarCharge = Convert.ToDecimal("0.00");
				newBilling.TeleChrage = Convert.ToDecimal("0.00");
				newBilling.WiFiCharges = Convert.ToDecimal("0.00");

				context.Billings.Add(newBilling);
				context.SaveChanges();
			}
		}
	}
}
