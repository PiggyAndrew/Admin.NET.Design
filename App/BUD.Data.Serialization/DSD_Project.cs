using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DSD.Data.Serialization;

/// <summary>
/// Defined a class to represent project
/// </summary>
public class DSD_Project
{
    /// <summary>
    /// Create a project instance
    /// </summary>
    public DSD_Project()
    {
        Pipes = new List<DSD_Pipe>();
        Manholes = new List<DSD_Manhole>();
    }

    /// <summary>
    /// The project information
    /// </summary>
    public DSD_ProjectInformation Information { get; set; }

    /// <summary>
    /// The project pipes
    /// </summary>
    public List<DSD_Pipe> Pipes { get; set; }

    /// <summary>
    /// The project manholes
    /// </summary>
    public List<DSD_Manhole> Manholes { get; set; }
}
