//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebRoleAds
{
    using System;
    using System.Collections.Generic;
    
    public partial class TbRating
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PostId { get; set; }
        public int Point { get; set; }
    
        public virtual TbPost TbPost { get; set; }
        public virtual TbUser TbUser { get; set; }
    }
}
