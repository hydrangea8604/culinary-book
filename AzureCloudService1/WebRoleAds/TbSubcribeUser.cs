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
    
    public partial class TbSubcribeUser
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SubcribedUserId { get; set; }
        public System.DateTime SubcribeDate { get; set; }
    
        public virtual TbUser TbUser { get; set; }
        public virtual TbUser TbUser1 { get; set; }
    }
}
