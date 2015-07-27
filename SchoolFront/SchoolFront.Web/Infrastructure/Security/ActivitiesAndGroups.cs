using System;

namespace SchoolFront.Web.Infrastructure.Security
{
    /// <summary>
    /// These are activities that can be performed in the system.
    /// These activities are then mapped to AD groups to drive
    /// authorization decisions.
    /// </summary>
    [Serializable]
    [Flags]
    public enum Activities
    {
        /// <summary>
        /// Business use flag
        /// </summary>
        BusinessUse = 1 << 0,           // = 00000001 = 1
        
        /// <summary>
        /// View system settings flag
        /// </summary>
        ViewSystemSettings = 1 << 1,    // = 00000010 = 2

        /// <summary>
        /// Edit system settings flag
        /// </summary>
        EditSystemSettings = 1 << 2     // = 00000100 = 4
    }

    /*
     * A note about bitwise operations and enums...
     * 
     * In order for bitwise operations to work correctly,
     * the enum values must be powers of 2 
     * (that is, 1, 2, 4, 8, 16, 32, ...
     * or 2^0 (=1), 2^1 (=2), 2^2 (=4), 2^3 (=8), 2^4 (=16), 2^5 (=32), ...).
     * 
     * That's what the bitwise shift left operator (<<) is doing.
     */

    /// <summary>
    /// The authorization groups.
    /// </summary>
    [Serializable]
    [Flags]
    public enum Groups
    {
        /// <summary>
        /// The user group
        /// </summary>
        User = 1 << 0,          // = 00000001 = 1
        
        /// <summary>
        /// The admin group
        /// </summary>
        Admin = 1 << 1,         // = 00000010 = 2
        
        /// <summary>
        /// The super user group
        /// </summary>
        SuperUser = 1 << 2,     // = 00000100 = 4
        
        /// <summary>
        /// The manager group
        /// </summary>
        Manager = 1 << 3,       // = 00001000 = 8
    }
}