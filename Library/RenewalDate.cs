//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Library
{
    using System;
    using System.Collections.Generic;
    
    public partial class RenewalDate
    {
        public int CDInteractionID { get; set; }
        public int RenewalDateID { get; set; }
        public System.DateTime RenewalDate1 { get; set; }
    
        public virtual CustomerDocumentInteraction CustomerDocumentInteraction { get; set; }
    }
}
