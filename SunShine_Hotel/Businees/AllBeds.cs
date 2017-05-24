using System;

namespace SunShine_Hotel.Businees
{
	public static class AllBeds
	{/// <summary>
	 /// Generates the cost per room
	 /// </summary>
		public static decimal roomCost = 0;
		public static int GuestCountdown = 0;

		private static int Doublebed = 2;
		private static int SingleBed = 2;
		//  public static int guestNumbersForForeach;
		//reference parameters change the underlying value of the input
		public static Tuple<decimal,int> BedCalc( int beds, decimal roomValue, string Bedtype)
		{//  Loop through the number of beds that each room has adding on the room cost each time.
			//Count down the guests until you either run out of beds, or guests.
			roomCost = 0;
			while (GuestCountdown > 0 && beds > 0)
			{
				
				if (Bedtype =="Doublebed" && GuestCountdown != 0)
				{
					roomCost += roomValue; // increase the room cost by amount   
					GuestCountdown -= 2; //reduce guest numbers by 2
					beds -= 1; //remove bed from the list
				}

				if (Bedtype == "SingleBed" && GuestCountdown != 0)
				{
					roomCost += roomValue; // increase the room cost by amount   
					GuestCountdown -= 1; //reduce guest numbers by 1m cost by amount   
					beds -= 1; //remove bed from the list
				}
				if (Bedtype == "ExtraBed" && GuestCountdown != 0)
				{
					roomCost += roomValue; // increase the room cost by amount   
					GuestCountdown -= 1; //reduce guest numbers by 1
					beds -= 1; //remove bed from the list
				}
				//if the room has double beds count through them

				//beds -= 1; //remove bed from the list
			}
			return Tuple.Create(roomCost, beds);


		}

		public static string OverflowMessage( string roomFullMessage, int doublebeds, int singlebeds, int extrabeds)
		{
			switch (extrabeds)
			{
				case 0:
					roomFullMessage += "Inc 2 Extra beds. ";
					break;
				case 1:
					roomFullMessage += "Inc 1 Extra bed. ";
					break;
				default:
					roomFullMessage += "No Extra beds";
					break;
			}
			//overflow message
			if (GuestCountdown >= 1 && doublebeds == 0 && singlebeds == 0 && extrabeds == 0)
			{
				roomFullMessage += GuestCountdown + " Guests must be in another room.";
				//  break;
			}
			//overflow message
			if (GuestCountdown == 0) // && doublebeds == 0 && singlebeds == 0 && extrabeds == 0) {
			{
				roomFullMessage += " No Overflow ";
				//  break;
			}
			//} while (guestNumbersForForeach > 0);
			return roomFullMessage;

		}
	}
}
