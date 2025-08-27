using Autodesk.Revit.DB;
using BUD.Revit.Framework;
using DSD.Toolkit.Logger;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace BUD.Drainage.Revit.Addin.Services;

internal class CustomFailureProcessor : IFailuresProcessor
{
    public void Dismiss(Document document)
    {


    }

    public FailureProcessingResult ProcessFailures(FailuresAccessor data)
    {
        Document document = data.GetDocument();

        if (!data.IsActive())
        {
            return FailureProcessingResult.Continue;
        }

        IList<FailureMessageAccessor> failureMessages = data.GetFailureMessages();
        if (failureMessages.Count == 0)
        {
            return FailureProcessingResult.Continue;
        }

        HostApplication.Current.Logger.Information(failureMessages.Count);
        for (int i = 0; i < failureMessages.Count; i++)
        {
            FailureMessageAccessor messageAccessor = failureMessages[i];
            FailureSeverity failureSeverity = messageAccessor.GetSeverity();
            string description = messageAccessor.GetDescriptionText();
            string caption = messageAccessor.GetDefaultResolutionCaption();

            ICollection<ElementId> ids = messageAccessor.GetFailingElementIds();
            List<ElementId> idss = new List<ElementId>(ids);


            HostApplication.Current.Logger.Information(i + 1);
            HostApplication.Current.Logger.Information($"caption:{caption}");
            HostApplication.Current.Logger.Information($"description:{description}");
            HostApplication.Current.Logger.Information($"failureSeverity:{failureSeverity}");


            if (data.IsElementsDeletionPermitted(idss, out string reson))
            {
                HostApplication.Current.Logger.Information($"reson:{reson}");

                data.DeleteElements(idss);
                data.DeleteWarning(messageAccessor);
                continue;
            }

            data.DeleteWarning(messageAccessor);
        }


        return FailureProcessingResult.ProceedWithCommit;
    }
}
