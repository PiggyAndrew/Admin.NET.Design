using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUD.Tuna.Revit.Extensions.DataSchema;

public static class SchemaExtensions
{
    public static Schema CreateSchema(this UIApplication application, Guid schemaGUID)
    {
        AddInId addin = application.ActiveAddInId;

        Schema schema = Schema.Lookup(schemaGUID);
        if (schema != null)
        {
            return schema;
        }

        using SchemaBuilder schemaBuilder = new SchemaBuilder(schemaGUID);
        schemaBuilder.SetSchemaName("Structural");
        schemaBuilder.SetApplicationGUID(addin.GetGUID());
        schemaBuilder.SetVendorId("Vircon");
        schemaBuilder.SetDocumentation("Use to defined support ");

        schemaBuilder.SetWriteAccessLevel(AccessLevel.Public);
        schemaBuilder.SetReadAccessLevel(AccessLevel.Public);

        schemaBuilder.AddSimpleField("string", typeof(string));

        return schemaBuilder.Finish();
    }
}
