//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SunShine_Hotel
{
    using System;
    using System.Collections.Generic;
    
    public partial class Guest
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Guest()
        {
            this.Billings = new HashSet<Billing>();
        }
    
        public int GuestID { get; set; }
        public int BookingIDFK { get; set; }
        public string Name { get; set; }
        public Nullable<int> Phone { get; set; }
        public string Address { get; set; }
        public Nullable<int> Guests { get; set; }
        public Nullable<int> Room { get; set; }
        public Nullable<System.DateTime> CheckIn { get; set; }
        public Nullable<System.DateTime> CheckOut { get; set; }
        public Nullable<System.DateTime> BookingDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Billing> Billings { get; set; }
        public virtual Booking Booking { get; set; }
    }
}
