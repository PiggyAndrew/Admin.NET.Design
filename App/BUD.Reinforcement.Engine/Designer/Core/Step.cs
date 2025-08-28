using DSD.Reinforcement.Engine.Designer.Output;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSD.Reinforcement.Engine.Designer.Core;

public class Step : INode
{
    public Step(string header)
    {
        Header = header;
        Log = new List<string>();
    }

    public string Header { get; }

    public NodeStatus NodeStatus { get; private set; }

    public List<string> Log { get; set; }

    public void Add(string message) => Log.Add(message);

    public void Add(string message, string arg)
    {
        Log.Add(string.Format(message, arg));
    }

    public override string ToString()
    {
        return base.ToString();
    }

    public Step Fail(string message)
    {
        this.Add(message);
        this.Add("");
        this.NodeStatus = NodeStatus.Failed;
        return this;
    }

    public Step Success(string message)
    {
        this.Add(message);
        this.NodeStatus = NodeStatus.Successed;
        return this;
    }
}
