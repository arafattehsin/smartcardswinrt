using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Security.Principal;
using System.Threading.Tasks;


namespace SmartCardLibrary
{
    public sealed class SmartcardContextSafeHandle : SafeHandle
    {
        public SmartcardContextSafeHandle()
            : base(IntPtr.Zero, true)
        {
        }

        //The default constructor will be called by P/Invoke smart 
        //marshalling when returning MySafeHandle in a method call.
        public override bool IsInvalid
        {
            //[SecurityPermission(SecurityAction.LinkDemand,
            //    UnmanagedCode = true)]
            get { return (this.handle == IntPtr.Zero); }
        }

        //We should not provide a finalizer. SafeHandle's critical 
        //finalizer will call ReleaseHandle for us.
        protected override bool ReleaseHandle()
        {

            SmartcardErrorCode result =
                (SmartcardErrorCode)UnsafeNativeMethods.ReleaseContext(handle);
            return (result == SmartcardErrorCode.None);
        }
    }
}
