#if PORTABLE
using System.Resources;
#endif
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

[assembly: AssemblyTitle("MicroFlow")]
[assembly: AssemblyDescription("Lightweight framework to create workflows")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyProduct("MicroFlow")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

#if PORTABLE
[assembly: NeutralResourcesLanguage("en")]
#endif

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.

[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM

#if !PORTABLE

[assembly: Guid("261d091d-3793-4492-b6a8-41e6c0a4dc8b")]
#endif

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]

[assembly: InternalsVisibleTo("MicroFlow.Test")]
[assembly: InternalsVisibleTo("MicroFlow.Graph")]
[assembly: InternalsVisibleTo("MicroFlow.Meta")]